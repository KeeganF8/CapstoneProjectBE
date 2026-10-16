using CapstoneProjectBE.Data;
using CapstoneProjectBE.DTOs.Auth;
using CapstoneProjectBE.ExternalServices.Email;
using CapstoneProjectBE.Models;
using CapstoneProjectBE.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.Json;

namespace CapstoneProjectBE.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IVerificationCodeService _verificationCodeService;
        private readonly IEmailService _emailService;
        private readonly IJwtTokenService _jwtTokenService;

        public AuthService(AppDbContext context, IPasswordHasher<User> passwordHasher, IVerificationCodeService verificationCodeService, IEmailService emailService, IJwtTokenService jwtTokenService)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _verificationCodeService = verificationCodeService;
            _emailService = emailService;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<RegisterResponse?> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken)
        {
            string email = request.Email.Trim().ToLowerInvariant();
            bool emailExists = await _context.Users.AnyAsync(user => user.Email == email, cancellationToken);

            if (emailExists)
            {
                return null;
            }

            DateTime now = DateTime.UtcNow;

            var user = new User
            {
                UserID = Guid.NewGuid(),
                Email = email,
                CreatedAt = now,
                IsActive = false
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);
            string verificationCode = _verificationCodeService.GenerateCode();

            var emailVerification = new EmailVerification
            {
                EmailVerificationID = Guid.NewGuid(),
                UserID = user.UserID,
                User = user,
                ExpiresAt = now.AddMinutes(15),
                LastSentAt = now,
                FailedAttempts = 0
            };

            emailVerification.CodeHash = _verificationCodeService.HashVerificationCode(emailVerification, verificationCode);
            user.EmailVerification = emailVerification;
            _context.Users.Add(user);

            await _context.SaveChangesAsync(cancellationToken);
            await _emailService.SendVerificationCodeAsync(email, verificationCode, cancellationToken);

            return new RegisterResponse
            {
                UserID = user.UserID,
                Email = user.Email,
                IsActive = user.IsActive,
                Token = null
            };
        }

        public async Task<VerifyEmailResponse?> VerifyEmailAsync(VerifyEmailRequest request, CancellationToken cancellationToken)
        {
            string email = request.Email.Trim().ToLowerInvariant();

            User? user = await _context.Users.Include(user => user.EmailVerification).SingleOrDefaultAsync(user => user.Email == email, cancellationToken);

            if (user is null || user.IsActive || user.EmailVerification is null)
            {
                return null;
            }

            EmailVerification verification = user.EmailVerification;

            if (verification.ExpiresAt <= DateTime.UtcNow || verification.FailedAttempts >= 5)
            {
                return null;
            }

            bool codeValid = _verificationCodeService.VerifyVerificationCode(verification, verification.CodeHash, request.VerificationCode);

            if (!codeValid)
            {
                verification.FailedAttempts++;

                await _context.SaveChangesAsync(cancellationToken);
                return null;
            }

            user.IsActive = true;

            _context.EmailVerifications.Remove(verification);

            await _context.SaveChangesAsync(cancellationToken);

            string token = _jwtTokenService.GenerateToken(user);

            return new VerifyEmailResponse
            {
                Message = "Successful email verification.",
                Token = token
            };
        }

        public async Task ResendVerificationAsync(ResendVerificationRequest request, CancellationToken cancellationToken)
        {
            string email = request.Email.Trim().ToLowerInvariant();

            User? user = await _context.Users.Include(user => user.EmailVerification).SingleOrDefaultAsync(user => user.Email == email, cancellationToken);

            if (user is null || user.IsActive)
            {
                return;
            }

            DateTime now = DateTime.UtcNow;

            if (user.EmailVerification is not null && user.EmailVerification.LastSentAt > now.AddMinutes(-1))
            {
                return;
            }

            string verificationCode = _verificationCodeService.GenerateCode();

            EmailVerification verification;

            if (user.EmailVerification is null)
            {
                verification = new EmailVerification
                {
                    EmailVerificationID = Guid.NewGuid(),
                    UserID = user.UserID,
                    User = user
                };

                user.EmailVerification = verification;
                _context.EmailVerifications.Add(verification);
            }

            else
            {
                verification = user.EmailVerification;
            }

            verification.CodeHash = _verificationCodeService.HashVerificationCode(verification, verificationCode);

            verification.ExpiresAt = now.AddMinutes(15);
            verification.LastSentAt = now;
            verification.FailedAttempts = 0;

            await _context.SaveChangesAsync(cancellationToken);
            await _emailService.SendVerificationCodeAsync(email, verificationCode, cancellationToken);
        }

        public async Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
        {
            string email = request.Email.Trim().ToLowerInvariant();

            User? user = await _context.Users.Include(user => user.UserProfile).SingleOrDefaultAsync(user => user.Email == email, cancellationToken);

            if (user is null || !user.IsActive)
            {
                return null;
            }

            PasswordVerificationResult passwordResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);

            if(passwordResult == PasswordVerificationResult.Failed)
            {
                return null;
            }

            if (passwordResult == PasswordVerificationResult.SuccessRehashNeeded)
            {
                user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

                await _context.SaveChangesAsync(cancellationToken);
            }

            string token = _jwtTokenService.GenerateToken(user);

            return new LoginResponse
            {
                Token = token,
                User = new AuthUserDto
                {
                    UserID = user.UserID,
                    Email = user.Email,
                    IsActive = user.IsActive
                },
                ProfileCompleted = user.UserProfile?.ProfileCompleted ?? false
            };
        }

        public async Task<AuthUserDto?> GetCurrentUserAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await _context.Users.AsNoTracking().Where(user => user.UserID == userId && user.IsActive).Select(user => new AuthUserDto
            {
                UserID = user.UserID,
                Email = user.Email,
                IsActive = user.IsActive
            }).SingleOrDefaultAsync(cancellationToken);
        }
    }
}

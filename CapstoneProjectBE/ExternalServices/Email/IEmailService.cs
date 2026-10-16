namespace CapstoneProjectBE.ExternalServices.Email
{
    public interface IEmailService
    {
        Task SendVerificationCodeAsync(string recipientEmail, string verificationCode, CancellationToken cancellationToken);
    }
}

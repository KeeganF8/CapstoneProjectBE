using CapstoneProjectBE.Services.Interfaces;
using CapstoneProjectBE.Data;
using CapstoneProjectBE.DTOs.Careers;
using Microsoft.EntityFrameworkCore;

namespace CapstoneProjectBE.Services
{
    public class CareerService : ICareerService
    {
        private readonly AppDbContext _context;

        public CareerService(AppDbContext context)
        {
            _context = context;
        }
        
        public async Task<IReadOnlyList<CareerDto>> GetAllAsync(CancellationToken cancellationToken)
        {
            //AsNoTracking useful for read-only queries
            return await _context.Careers.AsNoTracking().OrderBy(career => career.CareerName).Select(career => new CareerDto
            {
                CareerID = career.CareerID,
                CareerName = career.CareerName,
                Description = career.Description,
                Industry = career.Industry,
                EducationRequirements = career.EducationRequirements
            }).ToListAsync(cancellationToken);
        }

        public async Task<CareerDto?> GetByIdAsync( Guid careerId, CancellationToken cancellationToken)
        {
            return await _context.Careers.AsNoTracking().Where(career => career.CareerID == careerId).Select(career => new CareerDto
            {
                CareerID = career.CareerID,
                CareerName = career.CareerName,
                Description = career.Description,
                Industry = career.Industry,
                EducationRequirements = career.EducationRequirements
            }).SingleOrDefaultAsync(cancellationToken);
        }
    }
}

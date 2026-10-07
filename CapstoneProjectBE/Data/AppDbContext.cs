using CapstoneProjectBE.Models;
using Microsoft.EntityFrameworkCore;

namespace CapstoneProjectBE.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public  DbSet<Career> Careers  { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<CareerSkill> CareerSkills { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Career>(entity =>
            {
                entity.HasKey(career => career.CareerID);
                entity.Property(career => career.CareerName).HasMaxLength(100).IsRequired();
                entity.HasIndex(career => career.CareerName).IsUnique();
            });

            modelBuilder.Entity<Skill>(entity =>
            {
                entity.HasKey(skill => skill.SkillID);
                entity.Property(skill => skill.SkillName).HasMaxLength(100).IsRequired();
                entity.Property(skill => skill.Category).HasMaxLength(100).IsRequired();
                entity.HasIndex(skill => skill.SkillName).IsUnique();
            });

            modelBuilder.Entity<CareerSkill>(entity =>
            {
                entity.HasKey(careerSkill => careerSkill.CareerSkillID);
                entity.HasIndex(careerSkill => new {careerSkill.CareerID, careerSkill.SkillID}).IsUnique();
                entity.HasOne(careerSkill => careerSkill.Career).WithMany(career => career.CareerSkills).HasForeignKey(careerSkill => careerSkill.CareerID);
                entity.HasOne(careerSkill => careerSkill.Skill).WithMany(skill => skill.CareerSkills).HasForeignKey(careerSkill => careerSkill.SkillID);
            });

            //temp data
            var softwareDeveloperId =
                Guid.Parse("10000000-0000-0000-0000-000000000001");

            var csharpId =
                Guid.Parse("20000000-0000-0000-0000-000000000001");

            var sqlId =
                Guid.Parse("20000000-0000-0000-0000-000000000002");

            var restApiId =
                Guid.Parse("20000000-0000-0000-0000-000000000003");

            modelBuilder.Entity<Career>().HasData(
                new Career
                {
                    CareerID = softwareDeveloperId,
                    CareerName = "Software Developer",
                    Description = "Designs, develops and maintains software applications.",
                    Industry = "Information Technology",
                    EducationRequirements = "Relevant qualification or equivalent practical experience."
                });

            modelBuilder.Entity<Skill>().HasData(
                new Skill
                {
                    SkillID = csharpId,
                    SkillName = "C#",
                    Category = "Technical",
                    Description = "Programming using the C# language."
                },
                new Skill
                {
                    SkillID = sqlId,
                    SkillName = "SQL",
                    Category = "Technical",
                    Description = "Querying and managing relational databases."
                },
                new Skill
                {
                    SkillID = restApiId,
                    SkillName = "REST APIs",
                    Category = "Technical",
                    Description = "Designing and consuming RESTful APIs."
                });

            modelBuilder.Entity<CareerSkill>().HasData(
                new CareerSkill
                {
                    CareerSkillID = Guid.Parse("30000000-0000-0000-0000-000000000001"),
                    CareerID = softwareDeveloperId,
                    SkillID = csharpId,
                    ImportanceLevel = 5,
                    IsCoreSkill = true
                },
                new CareerSkill
                {
                    CareerSkillID = Guid.Parse("30000000-0000-0000-0000-000000000002"),
                    CareerID = softwareDeveloperId,
                    SkillID = sqlId,
                    ImportanceLevel = 4,
                    IsCoreSkill = true
                },
                new CareerSkill
                {
                    CareerSkillID = Guid.Parse("30000000-0000-0000-0000-000000000003"),
                    CareerID = softwareDeveloperId,
                    SkillID = restApiId,
                    ImportanceLevel = 4,
                    IsCoreSkill = true
                });
        }
    }
}

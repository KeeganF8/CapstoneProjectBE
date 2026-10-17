using CapstoneProjectBE.Models;
using Microsoft.EntityFrameworkCore;

namespace CapstoneProjectBE.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Career> Careers { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<CareerSkill> CareerSkills { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<EmailVerification> EmailVerifications { get; set; }

        public DbSet<LearningResource> LearningResources { get; set; }
        public DbSet<LearningRoadmap> LearningRoadmaps { get; set; }
        public DbSet<RoadmapItem> RoadmapItems { get; set; }
        public DbSet<RoadmapItemResource> RoadmapItemResources { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Career>(entity =>
            {
                entity.ToTable("careers");

                entity.HasKey(career => career.CareerID)
                    .HasName("careers_pkey");

                entity.Property(career => career.CareerID)
                    .HasColumnName("career_id")
                    .HasDefaultValueSql("gen_random_uuid()");

                entity.Property(career => career.CareerName)
                    .HasColumnName("career_name")
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(career => career.Description)
                    .HasColumnName("description")
                    .IsRequired();

                entity.Property(career => career.Industry)
                    .HasColumnName("industry")
                    .IsRequired();

                entity.Property(career => career.EducationRequirements)
                    .HasColumnName("education_requirements")
                    .IsRequired();

                entity.HasIndex(career => career.CareerName)
                    .IsUnique()
                    .HasDatabaseName("careers_career_name_key");
            });

            modelBuilder.Entity<Skill>(entity =>
            {
                entity.ToTable("skills");

                entity.HasKey(skill => skill.SkillID)
                    .HasName("skills_pkey");

                entity.Property(skill => skill.SkillID)
                    .HasColumnName("skill_id")
                    .HasDefaultValueSql("gen_random_uuid()");

                entity.Property(skill => skill.SkillName)
                    .HasColumnName("skill_name")
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(skill => skill.Category)
                    .HasColumnName("category")
                    .IsRequired();

                entity.Property(skill => skill.Description)
                    .HasColumnName("description")
                    .IsRequired();

                entity.HasIndex(skill => skill.SkillName)
                    .IsUnique()
                    .HasDatabaseName("skills_skill_name_key");
            });

            modelBuilder.Entity<CareerSkill>(entity =>
            {
                entity.ToTable("career_skills");

                entity.HasKey(careerSkill => careerSkill.CareerSkillID)
                    .HasName("career_skills_pkey");

                entity.Property(careerSkill => careerSkill.CareerSkillID)
                    .HasColumnName("career_skill_id")
                    .HasDefaultValueSql("gen_random_uuid()");

                entity.Property(careerSkill => careerSkill.CareerID)
                    .HasColumnName("career_id");

                entity.Property(careerSkill => careerSkill.SkillID)
                    .HasColumnName("skill_id");

                entity.Property(careerSkill => careerSkill.ImportanceLevel)
                    .HasColumnName("importance_level");

                entity.Property(careerSkill => careerSkill.IsCoreSkill)
                    .HasColumnName("is_core_skill")
                    .HasDefaultValue(false);

                entity.HasIndex(careerSkill => new
                {
                    careerSkill.CareerID,
                    careerSkill.SkillID
                })
                    .IsUnique()
                    .HasDatabaseName(
                        "career_skills_career_id_skill_id_key");

                entity.HasIndex(careerSkill => careerSkill.SkillID)
                    .HasDatabaseName("career_skills_skill_id_idx");

                entity.HasOne(careerSkill => careerSkill.Career)
                    .WithMany(career => career.CareerSkills)
                    .HasForeignKey(careerSkill => careerSkill.CareerID)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("career_skills_career_id_fkey");

                entity.HasOne(careerSkill => careerSkill.Skill)
                    .WithMany(skill => skill.CareerSkills)
                    .HasForeignKey(careerSkill => careerSkill.SkillID)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("career_skills_skill_id_fkey");
            });

            modelBuilder.Entity<LearningRoadmap>(entity =>
            {

                entity.ToTable("learning_roadmaps");

                entity.HasKey(roadmap => roadmap.RoadmapID);

                entity.Property(roadmap => roadmap.RoadmapID)
                .HasColumnName("roadmap_id")
                .HasDefaultValueSql("gen_random_uuid()");

                entity.Property(roadmap => roadmap.UserID)
                .HasColumnName("user_id");

                entity.Property(roadmap => roadmap.CareerID)
                .HasColumnName("career_id");

                entity.Property(roadmap => roadmap.Title)
                .HasColumnName("title")
                .HasMaxLength(150)
                .IsRequired();

                entity.Property(roadmap => roadmap.ProgressPercentage)
                .HasColumnName("progress_percentage")
                .HasPrecision(5,2);

                entity.Property(roadmap => roadmap.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("timestamp with time zone")
                .HasDefaultValueSql("now()");

                entity.Property(roadmap => roadmap.UpdatedAt)
                .HasColumnName("updated_at")
                .HasColumnType("timestamp with time zone")
                .HasDefaultValueSql("now()");

                entity.HasOne(roadmap => roadmap.Career)
                .WithMany()
                .HasForeignKey(roadmap => roadmap.CareerID);

                entity.HasOne(roadmap => roadmap.User)
                .WithMany()
                .HasForeignKey(roadmap => roadmap.UserID)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("learning_roadmaps_user_id_fkey");

            });


            modelBuilder.Entity<RoadmapItem>(entity =>
            {
                entity.ToTable("roadmap_items");
                entity.HasKey(item => item.RoadmapItemID);

                entity.Property(item => item.RoadmapItemID)
                .HasColumnName("roadmap_item_id")
                .HasDefaultValueSql("gen_random_uuid()");

                entity.Property(item => item.RoadmapID)
                .HasColumnName("roadmap_id");

                entity.Property(item => item.SkillID)
                .HasColumnName("skill_id");

                entity.Property(item => item.Title)
                .HasColumnName("title")
                .HasMaxLength(150)
                .IsRequired();

                entity.Property(item => item.Description)
                .HasColumnName("description");

                entity.Property(item => item.StepOrder)
                .HasColumnName("step_order");

                entity.Property(item => item.Priority)
                .HasColumnName("priority");

                entity.Property(item => item.Status)
                .HasColumnName("status");

                entity.Property(item => item.TargetDate)
                .HasColumnName("target_date")
                .HasColumnType("date");

                entity.Property(item => item.CompletedAt)
                .HasColumnName("completed_at")
                .HasColumnType("timestamp with time zone");

                entity.HasIndex(item => new { item.RoadmapID, item.StepOrder})
                .IsUnique();

                entity.HasOne(item => item.Roadmap)
                .WithMany(roadmap => roadmap.Items)
                .HasForeignKey(item => item.RoadmapID)
                .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(item => item.Skill)
                .WithMany()
                .HasForeignKey(item => item.SkillID);

            
                
            });

            modelBuilder.Entity<LearningResource>(entity =>
            {
                entity.ToTable("learning_resources");

                entity.HasKey(resource => resource.ResourceID);

                entity.Property(resource => resource.ResourceID)
                .HasColumnName("resource_id")
                .HasDefaultValueSql("gen_random_uuid()");

                entity.Property(resource => resource.SkillID)
                .HasColumnName("skill_id");

                entity.Property(resource => resource.ResourceName)
                .HasColumnName("resource_name")
                .HasMaxLength(200)
                .IsRequired();
                
                entity.Property(resource => resource.ResourceType)
                .HasColumnName("resource_type");

                entity.Property(resource => resource.Url)
                .HasColumnName("url")
                .HasMaxLength(500)
                .IsRequired();

                entity.Property(resource => resource.Provider)
                .HasColumnName("provider");

                entity.Property(resource => resource.DifficultyLevel)
                .HasColumnName("difficulty_level");

                entity.Property(resource => resource.CostType)
                .HasColumnName("cost_type");

                entity.Property(resource => resource.EstDuration)
                .HasColumnName("est_duration");

                entity.Property(resource => resource.Description)
                .HasColumnName("description");

                entity.Property(resource => resource.SourceType)
                .HasColumnName("source_type");

                entity.Property(resource => resource.IsActive)
                .HasColumnName("is_active");

                entity.HasIndex(resource => resource.SkillID);

                entity.HasOne(resource => resource.Skill)
                .WithMany()
                .HasForeignKey(resource => resource.SkillID);

                
            });

            modelBuilder.Entity<RoadmapItemResource>(entity =>
            {
                entity.ToTable("roadmap_item_resources");

                entity.HasKey(link => link.RoadmapItemResourceID);

                entity.Property(link => link.RoadmapItemResourceID)
                .HasColumnName("roadmap_item_resource_id")
                .HasDefaultValueSql("gen_random_uuid()");

               entity.Property(link => link.RoadmapItemID)
               .HasColumnName("roadmap_item_id");

                entity.Property(link => link.ResourceID)
               .HasColumnName("resource_id");

                entity.Property(link => link.IsCompleted)
               .HasColumnName("is_completed");

                entity.Property(link => link.CompletedAt)
               .HasColumnName("completed_at")
               .HasColumnType("timestamp with time zone");

               entity.HasIndex(link => new { link.RoadmapItemID, link.ResourceID})
               .IsUnique();

               entity.HasOne(link => link.RoadmapItem)
               .WithMany(item => item.Resources)
               .HasForeignKey(link => link.RoadmapItemID)
               .OnDelete(DeleteBehavior.Cascade);

               entity.HasOne(link => link.LearningResource)
               .WithMany()
               .HasForeignKey(link => link.ResourceID);
               
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

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");

                entity.HasKey(user => user.UserID)
                    .HasName("users_pkey");

                entity.Property(user => user.UserID)
                    .HasColumnName("user_id")
                    .HasDefaultValueSql("gen_random_uuid()");

                entity.Property(user => user.Email)
                    .HasColumnName("email")
                    .HasMaxLength(254)
                    .IsRequired();

                entity.HasIndex(user => user.Email)
                    .IsUnique()
                    .HasDatabaseName("users_email_key");

                entity.Property(user => user.PasswordHash)
                    .HasColumnName("password_hash")
                    .HasMaxLength(500)
                    .IsRequired();

                entity.Property(user => user.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("timestamp with time zone")
                    .HasDefaultValueSql("now()")
                    .IsRequired();

                entity.Property(user => user.IsActive)
                    .HasColumnName("is_active")
                    .HasDefaultValue(false);

                entity.HasOne(user => user.UserProfile)
                    .WithOne(profile => profile.User)
                    .HasForeignKey<UserProfile>(profile => profile.UserID)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("user_profiles_user_id_fkey");
            });

            modelBuilder.Entity<UserProfile>(entity =>
            {
                entity.ToTable("user_profiles");

                entity.HasKey(profile => profile.ProfileID)
                    .HasName("user_profiles_pkey");

                entity.Property(profile => profile.ProfileID)
                    .HasColumnName("profile_id")
                    .HasDefaultValueSql("gen_random_uuid()");

                entity.Property(profile => profile.UserID)
                    .HasColumnName("user_id");

                entity.Property(profile => profile.TargetCareerID)
                    .HasColumnName("target_career_id");

                entity.Property(profile => profile.FName)
                    .HasColumnName("first_name")
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(profile => profile.LName)
                    .HasColumnName("last_name")
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(profile => profile.Qualification)
                    .HasColumnName("qualification")
                    .HasMaxLength(200)
                    .IsRequired();

                entity.Property(profile => profile.FieldOfStudy)
                    .HasColumnName("field_of_study")
                    .HasMaxLength(150)
                    .IsRequired();

                entity.Property(profile => profile.ExperienceLevel)
                    .HasColumnName("experience_level")
                    .HasMaxLength(100);

                entity.Property(profile => profile.Interests)
                    .HasColumnName("interests");

                entity.Property(profile => profile.ProfileCompleted)
                    .HasColumnName("profile_completed")
                    .HasDefaultValue(false);

                entity.Property(profile => profile.UpdatedAt)
                    .HasColumnName("updated_at")
                    .HasColumnType("timestamp with time zone")
                    .HasDefaultValueSql("now()")
                    .IsRequired();

                entity.HasIndex(profile => profile.UserID)
                    .IsUnique()
                    .HasDatabaseName("user_profiles_user_id_key");

                entity.HasIndex(profile => profile.TargetCareerID)
                    .HasDatabaseName("user_profiles_target_career_id_idx");

                entity.HasOne(profile => profile.TargetCareer)
                    .WithMany()
                    .HasForeignKey(profile => profile.TargetCareerID)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName(
                        "user_profiles_target_career_id_fkey");
            });

            modelBuilder.Entity<EmailVerification>(entity =>
            {
                entity.ToTable("email_verifications");

                entity.HasKey(verification => verification.EmailVerificationID)
                    .HasName("email_verifications_pkey");

                entity.Property(verification => verification.EmailVerificationID)
                    .HasColumnName("email_verification_id")
                    .HasDefaultValueSql("gen_random_uuid()");

                entity.Property(verification => verification.UserID)
                    .HasColumnName("user_id");

                entity.Property(verification => verification.CodeHash)
                    .HasColumnName("code_hash")
                    .HasMaxLength(500)
                    .IsRequired();

                entity.Property(verification => verification.ExpiresAt)
                    .HasColumnName("expires_at")
                    .HasColumnType("timestamp with time zone")
                    .IsRequired();

                entity.Property(verification => verification.LastSentAt)
                    .HasColumnName("last_sent_at")
                    .HasColumnType("timestamp with time zone")
                    .IsRequired();

                entity.Property(verification => verification.FailedAttempts)
                    .HasColumnName("failed_attempts")
                    .HasDefaultValue(0);

                entity.HasIndex(verification => verification.UserID)
                    .IsUnique()
                    .HasDatabaseName("email_verifications_user_id_key");

                entity.HasOne(verification => verification.User)
                    .WithOne(user => user.EmailVerification)
                    .HasForeignKey<EmailVerification>(
                        verification => verification.UserID)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("email_verifications_user_id_fkey");
            });
        }
    }
}

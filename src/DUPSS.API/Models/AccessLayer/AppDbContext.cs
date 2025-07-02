using DUPSS.API.Models.Objects;
using Microsoft.EntityFrameworkCore;

namespace DUPSS.API.Models.AccessLayer
{

    public class AppDbContext : DbContext
    {
        // Existing DbSets
        public DbSet<Role> Role { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<Appointment> Appointment { get; set; }
        public DbSet<Campaign> Campaign { get; set; }
        public DbSet<CourseTopic> CourseTopic { get; set; }
        public DbSet<Course> Course { get; set; }
        public DbSet<CourseEnroll> CourseEnroll { get; set; }
        public DbSet<Blog> Blog { get; set; }
        public DbSet<BlogTopic> BlogTopic { get; set; }
        public DbSet<CampaignRegistration> CampaignRegistrations { get; set; }
        public DbSet<Appointment> Appointments { get; set; } = null!;

        // Add new DbSets for Assessment models
        public DbSet<Assessment> Assessments { get; set; }
        public DbSet<AssessmentVersion> AssessmentVersions { get; set; }
        public DbSet<AssessmentLanguage> AssessmentLanguages { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<QuestionOption> QuestionOptions { get; set; }
        public DbSet<AssessmentResponse> AssessmentResponses { get; set; }
        public DbSet<QuestionResponse> QuestionResponses { get; set; }


        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // It's good practice to call the base method.

            // Existing configurations
            modelBuilder.Entity<Course>().Ignore(c => c.ImageUrl);
            modelBuilder.Entity<Course>().Ignore(c => c.ImageUrl2);
            modelBuilder.Entity<Campaign>().Ignore(c => c.ImageUrl);
            modelBuilder.Entity<User>().Ignore(u => u.ImageUrl);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Member)
                .WithMany(u => u.MemberAppointments)
                .HasForeignKey(a => a.MemberId);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Consultant)
                .WithMany(u => u.ConsultantAppointments)
                .HasForeignKey(a => a.ConsultantId);

            modelBuilder.Entity<Campaign>()
                .HasOne(c => c.Staff)
                .WithMany(u => u.Campaigns)
                .HasForeignKey(c => c.StaffId);

            modelBuilder.Entity<Course>()
                .HasOne(c => c.Topic)
                .WithMany(t => t.Courses)
                .HasForeignKey(c => c.TopicId);

            modelBuilder.Entity<Course>()
                .HasOne(c => c.Staff)
                .WithMany(u => u.Courses)
                .HasForeignKey(c => c.StaffId);

            modelBuilder.Entity<CourseEnroll>()
                .HasOne(ce => ce.Member)
                .WithMany(u => u.Enrollments)
                .HasForeignKey(ce => ce.MemberId);

            modelBuilder.Entity<CourseEnroll>()
                .HasOne(ce => ce.Course)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(ce => ce.CourseId);

            modelBuilder.Entity<Blog>()
                .HasOne(b => b.Staff)
                .WithMany(u => u.Blogs)
                .HasForeignKey(b => b.StaffId);

            modelBuilder.Entity<Blog>()
                .HasOne(b => b.BlogTopic)
                .WithMany(bt => bt.Blogs)
                .HasForeignKey(b => b.BlogTopicId);

            modelBuilder.Entity<CampaignRegistration>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.MemberId);

            modelBuilder.Entity<CampaignRegistration>()
                .HasOne(r => r.Campaign)
                .WithMany()
                .HasForeignKey(r => r.CampaignId);

            // New configurations for Assessment models

            // Assessment -> AssessmentVersion (one-to-many)
            modelBuilder.Entity<AssessmentVersion>()
                .HasOne(v => v.Assessment)
                .WithMany(a => a.Versions)
                .HasForeignKey(v => v.AssessmentId);

            // AssessmentVersion -> AssessmentLanguage (one-to-many)
            modelBuilder.Entity<AssessmentLanguage>()
                .HasOne(l => l.Version)
                .WithMany(v => v.Languages)
                .HasForeignKey(l => l.VersionId);

            // AssessmentLanguage -> Question (one-to-many)
            modelBuilder.Entity<Question>()
                .HasOne(q => q.Language)
                .WithMany(l => l.Questions)
                .HasForeignKey(q => q.LanguageId);

            // Question -> QuestionOption (one-to-many)
            modelBuilder.Entity<QuestionOption>()
                .HasOne(o => o.Question)
                .WithMany(q => q.Options)
                .HasForeignKey(o => o.QuestionId);

            // AssessmentLanguage -> AssessmentResponse (one-to-many)
            modelBuilder.Entity<AssessmentResponse>()
                .HasOne(r => r.Language)
                .WithMany() // No inverse navigation property
                .HasForeignKey(r => r.LanguageId);

            // AssessmentResponse -> QuestionResponse (one-to-many)
            modelBuilder.Entity<QuestionResponse>()
                .HasOne(qr => qr.AssessmentResponse)
                .WithMany(ar => ar.Responses)
                .HasForeignKey(qr => qr.AssessmentResponseId);

            // Relationships for QuestionResponse
            modelBuilder.Entity<QuestionResponse>()
                .HasOne(qr => qr.Question)
                .WithMany() // No inverse navigation
                .HasForeignKey(qr => qr.QuestionId);

            modelBuilder.Entity<QuestionResponse>()
                .HasOne(qr => qr.SelectedOption)
                .WithMany() // No inverse navigation
                .HasForeignKey(qr => qr.SelectedOptionId);

            // Indexes for foreign keys
            modelBuilder.Entity<AssessmentVersion>().HasIndex(v => v.AssessmentId);
            modelBuilder.Entity<AssessmentLanguage>().HasIndex(l => l.VersionId);
            modelBuilder.Entity<Question>().HasIndex(q => q.LanguageId);
            modelBuilder.Entity<QuestionOption>().HasIndex(o => o.QuestionId);
            modelBuilder.Entity<AssessmentResponse>().HasIndex(r => r.LanguageId);
            modelBuilder.Entity<QuestionResponse>().HasIndex(qr => qr.AssessmentResponseId);
            modelBuilder.Entity<QuestionResponse>().HasIndex(qr => qr.QuestionId);
        }
    }
}
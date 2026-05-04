using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using cvapp.BL.Domain;
using cvapp.BL.Domain.ExtraExpertise;

namespace cvapp.DAL.EF;

public class CvAppDbContext : DbContext
{
    public DbSet<Person> Persons => Set<Person>();
    public DbSet<Career> Careers => Set<Career>();
    public DbSet<Certificate> Certificates => Set<Certificate>();
    public DbSet<Client> Clients => Set<Client>();
    public DbSet<Contact> Contacts => Set<Contact>();
    public DbSet<CourseTraining> CoursesTrainings => Set<CourseTraining>();
    public DbSet<CronosEmployer> CronosEmployers => Set<CronosEmployer>();
    public DbSet<CvCertificate> CvCertificates => Set<CvCertificate>();
    public DbSet<CvEducation> CvEducations => Set<CvEducation>();
    public DbSet<CvProject> CvProjects => Set<CvProject>();
    public DbSet<Domain> Domains => Set<Domain>();
    public DbSet<Education> Educations => Set<Education>();
    public DbSet<EducationOptions> EducationOptions => Set<EducationOptions>();
    public DbSet<Employer> Employers => Set<Employer>();
    public DbSet<Exam> Exams => Set<Exam>();
    public DbSet<Expertise> Expertises => Set<Expertise>();
    public DbSet<ExtraExpertise> ExtraExpertises => Set<ExtraExpertise>();
    public DbSet<ExtraTechnology> ExtraTechnologies => Set<ExtraTechnology>();
    public DbSet<InstituteOrSchool> InstitutesOrSchools => Set<InstituteOrSchool>();
   // public DbSet<LanguageSkill> LanguageSkills => Set<LanguageSkill>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<Sector> Sectors => Set<Sector>();
    public DbSet<Technology> Technologies => Set<Technology>();
    public DbSet<ClientDivision> ClientDivisions => Set<ClientDivision>();
    public DbSet<GroupCompany> GroupCompanies => Set<GroupCompany>();
    public DbSet<Language> Languages => Set<Language>();
    
    public CvAppDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite("Data Source=CvAppDb.db");
        }

        if (Debugger.IsAttached)
        {
            optionsBuilder.LogTo(message => Debug.WriteLine(message), LogLevel.Information);
        }
    }



    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ===== KEY DEFINITIONS =====
        modelBuilder.Entity<Person>().HasKey(e => e.Id);
        modelBuilder.Entity<Person>()
            .Property(e => e.Gender)
            .HasConversion<string>();
        modelBuilder.Entity<Person>()
            .Property(e => e.MaritalStatus)
            .HasConversion<string>();
        modelBuilder.Entity<Career>().HasKey(e => e.Id);
        modelBuilder.Entity<Certificate>().HasKey(e => e.Id);
        modelBuilder.Entity<Client>().HasKey(e => e.Id);
        modelBuilder.Entity<Contact>().HasKey(e => e.Id);
        modelBuilder.Entity<CourseTraining>().HasKey(e => e.Id);
        modelBuilder.Entity<CvCertificate>().HasKey(e => e.Id);
        modelBuilder.Entity<CvEducation>().HasKey(e => e.Id);
        modelBuilder.Entity<CvProject>().HasKey(e => e.Id);
        modelBuilder.Entity<Domain>().HasKey(e => e.Id);
        modelBuilder.Entity<Education>().HasKey(e => e.Id);
        modelBuilder.Entity<EducationOptions>().HasKey(e => e.Id);
        modelBuilder.Entity<Employer>().HasKey(e => e.Id);
        modelBuilder.Entity<Exam>().HasKey(e => e.Id);
        modelBuilder.Entity<Expertise>().HasKey(e => e.Id);
        modelBuilder.Entity<ExtraExpertise>().HasKey(e => e.Id);
        modelBuilder.Entity<ExtraTechnology>().HasKey(e => e.Id);
        modelBuilder.Entity<InstituteOrSchool>().HasKey(e => e.Id);
      //  modelBuilder.Entity<LanguageSkill>().HasKey(e => e.Id);
        modelBuilder.Entity<Project>().HasKey(e => e.Id);
        modelBuilder.Entity<Sector>().HasKey(e => e.Id);
        modelBuilder.Entity<Technology>().HasKey(e => e.Id);
        modelBuilder.Entity<CronosEmployer>().HasKey(e => e.Id);
        modelBuilder.Entity<ClientDivision>().HasKey(e => e.Id);
        modelBuilder.Entity<GroupCompany>().HasKey(e => e.Id);
        modelBuilder.Entity<Language>().HasKey(e => e.Id);
        modelBuilder.Entity<Exam>()
            .Property(e => e.Result)
            .HasConversion<string>();
        modelBuilder.Entity<CourseTraining>()
            .Property(e => e.CourseType)
            .HasConversion<string>();
        modelBuilder.Entity<InstituteOrSchool>()
            .Property(i => i.Type)
            .HasConversion<string>();

        // ===== PERSON RELATIONSHIPS =====
        // Person -> Careers (1:M)
        modelBuilder.Entity<Career>()
            .HasOne(e => e.Person)
            .WithMany(e => e.Careers)
            .HasForeignKey(e => e.PersonId)
            .OnDelete(DeleteBehavior.Cascade);

        // Person -> CvCertificates (1:M)
        modelBuilder.Entity<CvCertificate>()
            .HasOne(e => e.Person)
            .WithMany(e => e.Certificates)
            .HasForeignKey(e => e.PersonId)
            .OnDelete(DeleteBehavior.Cascade);

        // Person -> CourseTrainings (1:M)
        modelBuilder.Entity<CourseTraining>()
            .HasOne(e => e.Person)
            .WithMany(e => e.CoursesTrainings)
            .HasForeignKey(e => e.PersonId)
            .OnDelete(DeleteBehavior.Cascade);

        // Person -> CvEducations (1:M)
        modelBuilder.Entity<CvEducation>()
            .HasOne(e => e.Person)
            .WithMany(e => e.Educations)
            .HasForeignKey(e => e.PersonId)
            .OnDelete(DeleteBehavior.Cascade);

        // Person -> ExtraTechnologies (1:M)
        modelBuilder.Entity<ExtraTechnology>()
            .HasOne(e => e.Person)
            .WithMany(e => e.ExtraTechnologies)
            .HasForeignKey(e => e.PersonId)
            .OnDelete(DeleteBehavior.Cascade);

        // Person -> ExtraExpertises (1:M)
        modelBuilder.Entity<ExtraExpertise>()
            .HasOne(e => e.Person)
            .WithMany(e => e.ExtraExpertises)
            .HasForeignKey(e => e.PersonId)
            .OnDelete(DeleteBehavior.Cascade);

        // Person -> LanguageSkills (1:M)
      /*  modelBuilder.Entity<LanguageSkill>()
            .HasOne(e => e.Person)
            .WithMany(e => e.Languages)
            .HasForeignKey(e => e.PersonId)
            .OnDelete(DeleteBehavior.Cascade);*/

        // ===== CAREER RELATIONSHIPS =====
        // Career -> Contacts (1:M)
        modelBuilder.Entity<Contact>()
            .HasOne(e => e.Career)
            .WithMany(e => e.Contacts)
            .HasForeignKey(e => e.CareerId)
            .OnDelete(DeleteBehavior.Cascade);

        // Career -> Projects (1:M)
        modelBuilder.Entity<Project>()
            .HasOne(e => e.Career)
            .WithMany(e => e.Projects)
            .HasForeignKey(e => e.CareerId)
            .OnDelete(DeleteBehavior.Restrict);

        // Career -> Sector (M:1)
        modelBuilder.Entity<Career>()
            .HasOne(e => e.Sector)
            .WithMany(s => s.Careers)
            .HasForeignKey(e => e.SectorId)
            .OnDelete(DeleteBehavior.Restrict);

        // Career -> CronosEmployer (M:1)
        modelBuilder.Entity<Career>()
            .HasOne(e => e.CronosEmployer)
            .WithMany(ce => ce.Careers)
            .HasForeignKey(e => e.ChronosEmployerId)
            .OnDelete(DeleteBehavior.Restrict);

        // Career -> Employer (M:1)
        modelBuilder.Entity<Career>()
            .HasOne(e => e.Employer)
            .WithMany()
            .HasForeignKey(e => e.EmployerId)
            .OnDelete(DeleteBehavior.Restrict);

        // ===== CVPROJECT RELATIONSHIPS =====
        // CvProject -> Person (M:1)
        modelBuilder.Entity<CvProject>()
            .HasOne(e => e.Person)
            .WithMany()
            .HasForeignKey(e => e.PersonId)
            .OnDelete(DeleteBehavior.Cascade);

        // CvProject -> Career (M:1)
        modelBuilder.Entity<CvProject>()
            .HasOne(e => e.Career)
            .WithMany()
            .HasForeignKey(e => e.CareerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Project>()
            .HasOne(p => p.CvProject)
            .WithMany(cp => cp.Projects)
            .HasForeignKey(p => p.CvProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        // CvProject -> Client (M:1)
        modelBuilder.Entity<CvProject>()
            .HasOne(e => e.Client)
            .WithMany()
            .HasForeignKey(e => e.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        // CvProject -> GroupCompany (M:1)
        modelBuilder.Entity<CvProject>()
            .HasOne(e => e.GroupCompany)
            .WithMany(gc => gc.CvProjects)
            .HasForeignKey(e => e.GroupCompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        
       

        // ===== EDUCATION RELATIONSHIPS =====
        // CvEducation -> Education (M:1)
        modelBuilder.Entity<CvEducation>()
            .HasOne(e => e.Major)
            .WithMany(ed => ed.CvEducations)
            .HasForeignKey(e => e.MajorId)
            .OnDelete(DeleteBehavior.Restrict);

        // CvEducation -> InstituteOrSchool (M:1)
        modelBuilder.Entity<CvEducation>()
            .HasOne(e => e.InstituteOrSchool)
            .WithMany(i => i.CvEducations)
            .HasForeignKey(e => e.InstituteOrSchoolId)
            .OnDelete(DeleteBehavior.Restrict);

        // Education -> EducationOptions (M:1)
        modelBuilder.Entity<Education>()
            .HasOne(e => e.EducationLevel)
            .WithMany(el => el.Educations)
            .HasForeignKey(e => e.EducationLevelId)
            .OnDelete(DeleteBehavior.Restrict);

        // ===== CERTIFICATE RELATIONSHIPS =====
        // CvCertificate -> Certificate (M:1)
        modelBuilder.Entity<CvCertificate>()
            .HasOne(e => e.CertificateName)
            .WithMany(c => c.CvCertificates)
            .HasForeignKey(e => e.CertificateId)
            .OnDelete(DeleteBehavior.Restrict);

        // CvCertificate -> InstituteOrSchool (M:1)
        modelBuilder.Entity<CvCertificate>()
            .HasOne(e => e.InstituteOrSchool)
            .WithMany(i => i.CvCertificates)
            .HasForeignKey(e => e.InstituteOrSchoolId)
            .OnDelete(DeleteBehavior.Restrict);

        // ===== COURSE TRAINING RELATIONSHIPS =====
        // CourseTraining -> InstituteOrSchool (M:1)
        modelBuilder.Entity<CourseTraining>()
            .HasOne(e => e.InstituteOrSchool)
            .WithMany(i => i.CourseTrainings)
            .HasForeignKey(e => e.InstituteOrSchoolId)
            .OnDelete(DeleteBehavior.Restrict);

        // CourseTraining -> ExamResult (M:1)
        modelBuilder.Entity<CourseTraining>()
            .HasOne(e => e.Exam)
            .WithMany(r => r.CourseTrainings)
            .HasForeignKey(e => e.ExamId)
            .OnDelete(DeleteBehavior.Restrict);
        
        

        // ===== TECHNOLOGY RELATIONSHIPS =====
        // ExtraTechnology -> Technology (M:1)
        modelBuilder.Entity<ExtraTechnology>()
            .HasOne(e => e.Technology)
            .WithMany(t => t.ExtraTechnologies)
            .HasForeignKey(e => e.TechnologyId)
            .OnDelete(DeleteBehavior.Restrict);

        // Technology -> Domain (M:1)
        modelBuilder.Entity<Technology>()
            .HasOne(e => e.Domain)
            .WithMany(d => d.Technologies)
            .HasForeignKey(e => e.DomainId)
            .OnDelete(DeleteBehavior.Cascade);

        // ===== EXPERTISE RELATIONSHIPS =====
        // ExtraExpertise -> Sector (M:1)
        modelBuilder.Entity<ExtraExpertise>()
            .HasOne(e => e.Sector)
            .WithMany(s => s.ExtraExpertises)
            .HasForeignKey(e => e.SectorId)
            .OnDelete(DeleteBehavior.Restrict);

        // ExtraExpertise -> Expertise (M:1)
        modelBuilder.Entity<ExtraExpertise>()
            .HasOne(e => e.Expertise)
            .WithMany(ex => ex.ExtraExpertises)
            .HasForeignKey(e => e.ExpertiseId)
            .OnDelete(DeleteBehavior.Restrict);

        // ===== PROJECT RELATIONSHIPS =====
        // Project -> Sector (M:1)
        modelBuilder.Entity<Project>()
            .HasOne(e => e.Sector)
            .WithMany(s => s.Projects)
            .HasForeignKey(e => e.SectorId)
            .OnDelete(DeleteBehavior.Restrict);

        // Project -> CronosEmployer (M:1)
        modelBuilder.Entity<Project>()
            .HasOne(e => e.CronosEmployer)
            .WithMany()
            .HasForeignKey(e => e.CronosEmployerId)
            .OnDelete(DeleteBehavior.Restrict);

        // Project -> ClientDivision (M:1)
        modelBuilder.Entity<Project>()
            .HasOne(e => e.ClientDivision)
            .WithMany(cd => cd.Projects)
            .HasForeignKey(e => e.ClientDivisionId)
            .OnDelete(DeleteBehavior.Restrict);
      
        modelBuilder.Entity<Domain>().HasKey(d => d.Id);
        
        modelBuilder.Entity<Domain>()
            .HasMany(d => d.Technologies)
            .WithOne(t => t.Domain)
            .HasForeignKey(t => t.DomainId);
    }
}
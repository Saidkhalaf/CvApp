using cvapp.BL.Domain;
using cvapp.BL.Domain.enums;

namespace cvapp.DAL.EF;

public class CvAppInitializer
{
    public static void Initialize(CvAppDbContext dbContext, bool dropCreateDatabase = false)
    {
        if (dropCreateDatabase)
        {
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();
            Seed(dbContext);
            return;
        }

        if (dbContext.Database.EnsureCreated())
        {
            Seed(dbContext);
        }
    }


    public static void Seed(CvAppDbContext dbContext)
{
    var person = new Person
    {
        Id = AppDefaults.DefaultPersonId,
        FirstName = "Romeo",
        LastName = "Attoyan",
        Street = "Meir",
        HouseNumber = "1",
        PostalCode = "2000",
        City = "Antwerpen",
        Country = "Belgie",
        Email = "romeo.attoyan@example.com",
        Gender = Gender.Male,
        MaritalStatus = MaritalStatus.Single,
        JobTitle = "Software Engineer",
        Summary = "Experienced software engineer with a passion for developing innovative programs that expedite the efficiency and effectiveness of organizational success. Proficient in technology and writing code to create systems that are reliable and user-friendly."
    };
    dbContext.Persons.Add(person);

        dbContext.EducationOptions.AddRange(
            new EducationOptions { EducationLevel = EducationLevel.AcademischGerichteBachelor, EqfLevel = 6 },
            new EducationOptions
                { EducationLevel = EducationLevel.AcademischeGerichteBachelorNaBachelor, EqfLevel = 6 },
            new EducationOptions { EducationLevel = EducationLevel.ArtSecondaryEducation, EqfLevel = 4 },
            new EducationOptions { EducationLevel = EducationLevel.BeroepsSecundairOnderwijs, EqfLevel = 4 },
            new EducationOptions { EducationLevel = EducationLevel.Doctor, EqfLevel = 8 },
            new EducationOptions { EducationLevel = EducationLevel.HogerBeroepsonderwijs, EqfLevel = 5 },
            new EducationOptions { EducationLevel = EducationLevel.Master, EqfLevel = 7 },
            new EducationOptions { EducationLevel = EducationLevel.MasterNaMaster, EqfLevel = 7 }
        );

        dbContext.SaveChanges();

        var bachelorOptionId = dbContext.EducationOptions
            .Single(x => x.EducationLevel == EducationLevel.AcademischGerichteBachelor).Id;
        var masterOptionId = dbContext.EducationOptions
            .Single(x => x.EducationLevel == EducationLevel.Master).Id;

        var appliedInformatics = new Education
        {
            Name = "Applied Informatics",
            Eqf = 6,
            IsQualified = true,
            EducationLevelId = bachelorOptionId,
            CreatedAt = DateTime.UtcNow,
            Total = 0
        };

        var accounting = new Education
        {
            Name = "Accounting",
            Eqf = 7,
            IsQualified = true,
            EducationLevelId = masterOptionId,
            CreatedAt = DateTime.UtcNow,
            Total = 0
        };

        var kdg = new InstituteOrSchool
        {
            Name = "Karel de Grote Hogeschool",
            Type = InstituteType.School,
            Email = "info@kdg.be",
            Website = "https://www.kdg.be",
            TelephoneNumber = "+32 3 613 13 13",
            Fax = "",
            BtwNumber = "BE0404.488.837",
            Comments = "Default seed institute"
        };
        var kuLeuven = new InstituteOrSchool
        {
            Name = "KuLeuven",
            Type = InstituteType.School,
            Email = "info@thomasmoore.be",
            Website = "https://www.thomasmoore.be",
            TelephoneNumber = "+32 3 613 13 13",
            Fax = "",
            BtwNumber = "BE0404.488.837",
            Comments = "Default seed institute"
        };
        var thomasMoore = new InstituteOrSchool
        {
            Name = "Thomas Moore",
            Type = InstituteType.School,
            Email = "info@thomasmoore.be",
            Website = "https://www.thomasmoore.be",
            TelephoneNumber = "+32 3 613 13 13",
            Fax = "",
            BtwNumber = "BE0404.488.837",
            Comments = "Default seed institute"
        };
        var ap = new InstituteOrSchool
        {
            Name = "Ap",
            Type = InstituteType.School,
            Email = "info@ap.be",
            Website = "https://www.ap.be",
            TelephoneNumber = "+32 3 613 13 13",
            Fax = "",
            BtwNumber = "BE0404.488.837",
            Comments = "Default seed institute"
        };

        dbContext.Educations.AddRange(appliedInformatics, accounting);
        dbContext.InstitutesOrSchools.AddRange(kdg, kuLeuven, thomasMoore, ap);


        dbContext.CvEducations.AddRange(
            new CvEducation
            {
                StartYear = 2018,
                EndYear = 2021,
                Major = appliedInformatics,
                InstituteOrSchool = kdg,
                Diploma = null,
                Status = EducationStatus.InProgress,
                Person = person
            },
            new CvEducation
            {
                StartYear = 2021,
                EndYear = 2023,
                Major = accounting,
                InstituteOrSchool = kdg,
                Diploma = null,
                Status = EducationStatus.Unknown,
                Person = person
            }
        );
        var azureFundamentals = new Certificate
        {
            Name = "Microsoft Azure Fundamentals",
            Type = CertificateType.AfleggenExamen
        };

        var scrumMaster = new Certificate
        {
            Name = "Professional Scrum Master I",
            Type = CertificateType.AfleggenTrajectExamens
        };

        var itilFoundation = new Certificate
        {
            Name = "ITIL Foundation",
            Type = CertificateType.DeelNameCursus

        };

        dbContext.Certificates.AddRange(
            azureFundamentals,
            scrumMaster,
            itilFoundation
        );



        dbContext.SaveChanges();

        dbContext.CvCertificates.Add(new CvCertificate(
            2020,
            azureFundamentals.Id,
            kdg.Id,
            "AZ-900",
            string.Empty,
            new DateTime(2025, 6, 15),
            person.Id
        ));

        dbContext.CvCertificates.Add(new CvCertificate(
            2021,
            scrumMaster.Id,
            kuLeuven.Id,
            "PSM-I",
            string.Empty,
            new DateTime(2026, 9, 20),
            person.Id
        ));

        dbContext.CvCertificates.Add(new CvCertificate(
            2022,
            itilFoundation.Id,
            thomasMoore.Id,
            "ITIL-F",
            string.Empty,
            new DateTime(2024, 12, 10),
            person.Id
        ));

        var courseTrainings = new[]
        {
            new CourseTraining
            {
                Name = "Advanced C# Programming",
                Year = 2020,
                InstituteOrSchoolId = kdg.Id,
                CourseType = CourseType.Online,
                PersonId = person.Id
            },
            new CourseTraining
            {
                Name = "Cloud Architecture Fundamentals",
                Year = 2021,
                InstituteOrSchoolId = kuLeuven.Id,
                CourseType = CourseType.SelfStudy,
                Exam = new Exam    
                    {      
                        Name = "Cloud Architecture Fundamentals Exam",
                        Result = ExamResult.Passed
                    },
                PersonId = person.Id
            },
            new CourseTraining
            {
                Name = "DevOps Best Practices",
                Year = 2022,
                InstituteOrSchoolId = thomasMoore.Id,
                CourseType = CourseType.Classroom,
                PersonId = person.Id
            }
        };

        dbContext.CoursesTrainings.AddRange(courseTrainings);
        dbContext.SaveChanges();
    }
}
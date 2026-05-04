using System.ComponentModel.DataAnnotations;
using cvapp.BL.Domain;
using cvapp.BL.Domain.enums;
using cvapp.DAL.Training;

namespace BL.Training;

public class TrainingManager(ITrainingRepository repository)
    : ITrainingManager
{

    public IEnumerable<CourseTraining> GetAllTrainings()
    {
        return repository.ReadAlTrainings();
    }

    public CourseTraining GetTrainingById(long id)
    {
        return repository.ReadTrainingById(id);
    }

    public CourseTraining AddTraining(string name, int year, CourseType courseType, long personId, 
        string instituteName = null, 
        InstituteType? instituteType = null, 
        string email = null,      
        string website = null,    
        string phone = null, 
        string fax = null,
        string btw = null,        
        string comments = null,  
        string examName = null, 
        ExamResult? examResult = null)
    {
        
        if (repository.ReadAlTrainings()
            .Any(t => t.Name == name && t.Year == year && t.PersonId == personId))
        {
            throw new ValidationException("This training already exists.");
        }
        
        // 1. Prepare sub-objects ONLY if they have names
        InstituteOrSchool institute = null;
        if (!string.IsNullOrWhiteSpace(instituteName))
        {
            institute = new InstituteOrSchool 
            { 
                Name = instituteName.Trim(), 
                Type = instituteType ?? InstituteType.Other,
                Email = email,
                Website = website,
                TelephoneNumber = phone,
                Fax = fax,
                BtwNumber = btw,
                Comments = comments
            };
        }

        Exam exam = null;
        if (!string.IsNullOrWhiteSpace(examName))
        {
            exam = new Exam 
            { 
                Name = examName.Trim(), 
                Result = examResult ?? ExamResult.Passed 
            };
        }
        
        // 2. Construct the training object
        var training = new CourseTraining
        {
            Name = name,
            Year = year,
            CourseType = courseType,
            PersonId = personId,
            InstituteOrSchool = institute,
            Exam = exam
        };

        // 3. Validation Logic
        // This is where it fails if the Domain Class (CourseTraining) is too strict
        var context = new ValidationContext(training);
        Validator.ValidateObject(training, context, validateAllProperties: true);

        return repository.CreateTraining(training);
    }
    

    public IEnumerable<InstituteOrSchool> GetAllInstituteOrSchools()
    {
        return repository.ReadAllInstituteOrSchools();
    }

    public IEnumerable<Exam> GetAllExams()
    {
        return repository.ReadAllExams();
    }

    public Exam AddExam(string name, ExamResult result)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ValidationException("Naam van het examen is verplicht.");
        }

        var normalizedName = name.Trim();
        if (normalizedName.Length < 2 || normalizedName.Length > 200)
        {
            throw new ValidationException("Examennaam moet tussen 2 en 200 tekens zijn.");
        }

        var exam = new Exam
        {
            Name = normalizedName,
            Result = result
        };

        return repository.CreateExam(exam);
    }

    public void UpdateTraining(long id,
        string name,
        int year,
        CourseType courseType,
        long? instituteOrSchoolId,
        long? examId, 
        string instName = null, 
        InstituteType? instType = null, 
        string instEmail = null, 
        string instWeb = null, 
        string instPhone = null, 
        string fax = null,
        string instBtw = null, 
        string instComm = null,
        string exName = null, 
        ExamResult? exResult = null)
    {
        // 1. Get the aggregate root with its dependencies included
        var training = repository.ReadTrainingById(id);
        if (training == null) throw new ArgumentException("Training not found.", nameof(id));

        // 2. Update Aggregate Root basic properties
        training.Name = name;
        training.Year = year;
        training.CourseType = courseType;

        // 3. Handle Institute (Aggregate member)
        if (instituteOrSchoolId.HasValue && instituteOrSchoolId.Value > 0)
        {
            if (training.InstituteOrSchoolId != instituteOrSchoolId)
            {
                training.InstituteOrSchoolId = instituteOrSchoolId;
                training.InstituteOrSchool = repository.ReadInstituteOrSchoolById(instituteOrSchoolId.Value);
            }
            
            if (training.InstituteOrSchool != null && !string.IsNullOrWhiteSpace(instName))
            {
                UpdateInstituteFields(training.InstituteOrSchool, instName, instType, instEmail, instWeb, instPhone, fax, instBtw, instComm);
            }
        }
        else if (!string.IsNullOrWhiteSpace(instName))
        {
            if (training.InstituteOrSchool != null)
            {
                UpdateInstituteFields(training.InstituteOrSchool, instName, instType, instEmail, instWeb, instPhone, fax, instBtw, instComm);
            }
            else
            {
                training.InstituteOrSchool = new InstituteOrSchool();
                UpdateInstituteFields(training.InstituteOrSchool, instName, instType, instEmail, instWeb, instPhone, fax,instBtw, instComm);
            }
        }
        else
        {
            training.InstituteOrSchool = null;
            training.InstituteOrSchoolId = null;
        }

        // 4. Handle Exam (Aggregate member)
        if (examId.HasValue && examId.Value > 0)
        {
            if (training.ExamId != examId)
            {
                training.ExamId = examId;
                training.Exam = repository.ReadExamById(examId.Value);
            }

            if (training.Exam != null && !string.IsNullOrWhiteSpace(exName))
            {
                training.Exam.Name = exName;
                training.Exam.Result = exResult ?? training.Exam.Result;
            }
        }
        else if (!string.IsNullOrWhiteSpace(exName))
        {
            if (training.Exam != null)
            {
                training.Exam.Name = exName;
                training.Exam.Result = exResult ?? training.Exam.Result;
            }
            else
            {
                training.Exam = new Exam { Name = exName, Result = exResult ?? ExamResult.Passed };
            }
        }
        else
        {
            training.Exam = null;
            training.ExamId = null;
        }

        // 5. Validate and Persist
        var context = new ValidationContext(training);
        Validator.ValidateObject(training, context, validateAllProperties: true);

        repository.UpdateTraining(training);
    }

    public void DeleteTraining(long id)
    {
        var training = repository.ReadTrainingById(id);
        if (training == null) return;
        repository.DeleteTraining(id);
    }

    private void UpdateInstituteFields(InstituteOrSchool inst, string name, InstituteType? type, string email, string web, string phone, string fax, string btw, string comm)
    {
        inst.Name = name;
        inst.Type = type ?? inst.Type;
        inst.Email = email;
        inst.Website = web;
        inst.TelephoneNumber = phone;
        inst.Fax = fax;
        inst.BtwNumber = btw;
        inst.Comments = comm;
    }
}
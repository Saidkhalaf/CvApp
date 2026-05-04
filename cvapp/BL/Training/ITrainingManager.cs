using cvapp.BL.Domain;
using cvapp.BL.Domain.enums;
using Microsoft.AspNetCore.Http;

namespace BL.Training;

public interface ITrainingManager
{
    IEnumerable<CourseTraining> GetAllTrainings();
    CourseTraining GetTrainingById(long id);

    CourseTraining AddTraining(string name, int year, CourseType courseType, long personId,
        string instituteName = null,
        InstituteType? instituteType = null,
        string email = null,
        string website = null,
        string phone = null,
        string fax = null,
        string btw = null,
        string comments = null,
        string examName = null,
        ExamResult? examResult = null);

    IEnumerable<InstituteOrSchool> GetAllInstituteOrSchools();
    IEnumerable<Exam> GetAllExams();
    Exam AddExam(string name, ExamResult result);
    
    void UpdateTraining(long id,
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
        ExamResult? exResult = null);
    
    void DeleteTraining(long id);
}
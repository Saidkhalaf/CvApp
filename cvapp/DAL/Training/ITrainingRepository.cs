using cvapp.BL.Domain;
using cvapp.BL.Domain.enums;

namespace cvapp.DAL.Training;

public interface ITrainingRepository
{
    CourseTraining CreateTraining(CourseTraining training);
    InstituteOrSchool CreateInstituteOrSchool(InstituteOrSchool instituteOrSchool);
    IEnumerable<CourseTraining>ReadAlTrainings();
    CourseTraining ReadTrainingById(long id);
    InstituteOrSchool ReadInstituteOrSchoolById(long instituteOrSchoolId);
    Exam ReadExamById(long examId);
    Exam CreateExam(Exam exam);
    IEnumerable<InstituteOrSchool> ReadAllInstituteOrSchools();
    IEnumerable<Exam> ReadAllExams();
    void UpdateTraining(CourseTraining training);
    void  DeleteTraining(long id);
}
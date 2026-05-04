using cvapp.BL.Domain.enums;

namespace cvapp.BL.Domain;

public class Exam
{
    public long Id { get; set; }
    public string Name { get; set; }
    public ExamResult Result { get; set; }
    public ICollection<CourseTraining> CourseTrainings { get; set; } = new List<CourseTraining>();
    
}
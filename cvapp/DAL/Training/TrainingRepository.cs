    using cvapp.BL.Domain;
    using cvapp.DAL.EF;
    using Microsoft.EntityFrameworkCore;

    namespace cvapp.DAL.Training;

    public class TrainingRepository(CvAppDbContext context) : ITrainingRepository
    {
        public CourseTraining CreateTraining(CourseTraining training)
        {
            context.CoursesTrainings.Add(training);
            context.SaveChanges();
            return training;
        }

        public IEnumerable<CourseTraining> ReadAlTrainings()
        {
            return context.CoursesTrainings
                .Include(t => t.InstituteOrSchool)
                .Include(t => t.Exam)
                .Include(t => t.Person)
                .AsNoTracking()
                .OrderByDescending(t => t.Id)
                .ToList();
        }

        public CourseTraining ReadTrainingById(long id)
        {
            return context.CoursesTrainings
                .Include(t => t.Person)
                .Include(t => t.Exam)
                .Include(t => t.InstituteOrSchool)
                .SingleOrDefault(t => t.Id == id);
        }

        public InstituteOrSchool ReadInstituteOrSchoolById(long instituteOrSchoolId)
        {
            return context.InstitutesOrSchools.Single(ios => ios.Id == instituteOrSchoolId);
        }

        public InstituteOrSchool CreateInstituteOrSchool(InstituteOrSchool instituteOrSchool)
        {
            context.InstitutesOrSchools.Add(instituteOrSchool);
            context.SaveChanges();
            return instituteOrSchool;
        }

        public Exam ReadExamById(long examId)
        {
            return context.Exams.SingleOrDefault(e=> e.Id == examId);
        }

        public Exam CreateExam(Exam exam)
        {
            context.Exams.Add(exam);
            context.SaveChanges();
            return exam;
        }

        public IEnumerable<InstituteOrSchool> ReadAllInstituteOrSchools()
        {
            return context.InstitutesOrSchools;
        }

        public IEnumerable<Exam> ReadAllExams()
        {
            return context.Exams;
        }

        public void UpdateTraining(CourseTraining training)
        {
            context.CoursesTrainings.Update(training);
            context.SaveChanges();
        }

        public void DeleteTraining(long id)
        {
            var training = context.CoursesTrainings
                .Include(t => t.InstituteOrSchool)
                .Include(t => t.Exam)
                .SingleOrDefault(t => t.Id == id);
            if (training != null)
            {
                // Cascade delete
                if (training.InstituteOrSchool != null) context.InstitutesOrSchools.Remove(training.InstituteOrSchool);
                if (training.Exam != null) context.Exams.Remove(training.Exam);
                
                context.CoursesTrainings.Remove(training);
                context.SaveChanges();
            }
        }
    }
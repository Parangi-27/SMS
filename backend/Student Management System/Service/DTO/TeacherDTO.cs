using Repository.Model;

namespace Service.DTO
{
    public class TeacherDTO
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime DateOfEnrollment { get; set; }
        public int Salary { get; set; }
        public ClassLevels Class { get; set; }
        public int SubjectId { get; set; }
        public string Qualification { get; set; }

    }
}

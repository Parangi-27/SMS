using Repository.Model;

namespace Service.DTO
{
    public class EditStudentDTO
    {
        public string Name { get; set; }
        public int RollNumber { get; set; }
        public ClassLevels classLevels { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime DateOfEnrollment { get; set; }

    }
}

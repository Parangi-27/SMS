using Repository.Model;

namespace Service.DTO
{
    public class GetTeacherDetailsDTO
    {
        public int userId { get; set; }
        public int RollNumber { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime DateOfEnrollment { get; set; }
        public ClassLevels Class { get; set; }
    }
}

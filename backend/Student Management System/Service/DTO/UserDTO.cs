using Repository.Model;

namespace Service.DTO
{
    public class UserDTO
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime DateOfEnrollment { get; set; }
        public RoleTypes Role { get; set; }
    }
}

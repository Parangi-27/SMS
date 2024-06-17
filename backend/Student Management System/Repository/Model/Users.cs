using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Repository.Model
{
    public enum RoleTypes
    {
        Admin = 1,
        Teacher,
        Student
    }
    public class Users
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(50, ErrorMessage = "Name cannot exceed 50 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        [StringLength(50, ErrorMessage = "Email cannot exceed 50 characters.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100)]

        public string Password { get; set; }

        [Required(ErrorMessage = "Date of Birth is required.")]
        [Column(TypeName = "date")]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Date of Enrollment is required.")]
        [Column(TypeName = "date")]
        public DateTime DateOfEnrollment { get; set; }

        [Required(ErrorMessage = "Role is required.")]
        public RoleTypes Role { get; set; }

        [Required(ErrorMessage = "IsActive status is required.")]
        public bool IsActive { get; set; }

        [Required(ErrorMessage = "IsPasswordReset status is required.")]
        public bool IsPasswordReset { get; set; }
    }

}

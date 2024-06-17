using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Repository.Model
{
    public class Student
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "User ID is required.")]
        [ForeignKey("User")]
        public int UserId { get; set; }
        public virtual Users User { get; set; }

        [Required(ErrorMessage = "Roll Number is required.")]
        public int RollNumber { get; set; }
        [Required(ErrorMessage = "Class is required.")]
        public ClassLevels Class { get; set; }

        [Required(ErrorMessage = "Created On date is required.")]
        [Column(TypeName = "date")]
        public DateTime CreatedOn { get; set; }

        [Required(ErrorMessage = "Created By is required.")]
        public int CreatedBy { get; set; }

        [Column(TypeName = "date")]
        public DateTime? ModifiedOn { get; set; }

        public RoleTypes? ModifiedBy { get; set; }
    }
}

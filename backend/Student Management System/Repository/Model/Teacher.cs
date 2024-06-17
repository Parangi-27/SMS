using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Repository.Model
{
    public class Teacher
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "User ID is required.")]
        [ForeignKey("User")]
        public int UserId { get; set; }
        public virtual Users User { get; set; }

        [Required(ErrorMessage = "Class is required.")]
        public ClassLevels Class { get; set; }

        [Required(ErrorMessage = "Subject ID is required.")]
        [ForeignKey("Subject")]
        public int SubjectId { get; set; }
        public virtual Subject Subject { get; set; }
        [Required]
        public int Salary { get; set; }

        [Required(ErrorMessage = "Qualification is required.")]
        [MaxLength(100, ErrorMessage = "Qualification cannot exceed 100 characters.")]
        public string Qualification { get; set; }

        [Required(ErrorMessage = "Created On date is required.")]
        [Column(TypeName = "date")]
        public DateTime CreatedOn { get; set; }

        [Column(TypeName = "date")]
        public DateTime? ModifiedOn { get; set; }
    }
}

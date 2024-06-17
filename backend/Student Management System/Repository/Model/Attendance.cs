using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Repository.Model
{
    public class Attendance
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Student ID is required.")]
        [ForeignKey("Student")]
        public int StudentId { get; set; }
        public virtual Student Student { get; set; }

        [Required(ErrorMessage = "Teacher ID is required.")]
        [ForeignKey("Teacher")]
        public int TeacherId { get; set; }
        public virtual Teacher Teacher { get; set; }

        [Required(ErrorMessage = "Class is required.")]
        public ClassLevels ClassLevel { get; set; }

        [Required(ErrorMessage = "Subject ID is required.")]
        [ForeignKey("Subject")]
        public int SubjectId { get; set; }
        public virtual Subject Subject { get; set; }

        [Required(ErrorMessage = "Date is required.")]
        [Column(TypeName = "date")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Attendance status is required.")]
        public bool isPresent { get; set; }

        [Required(ErrorMessage = "Created On date is required.")]
        [Column(TypeName = "date")]
        public DateTime CreatedOn { get; set; }

        [Required(ErrorMessage = "Created By is required.")]
        public int CreatedBy { get; set; }

        [Column(TypeName = "date")]
        public DateTime? ModifiedOn { get; set; }
        public int? ModifiedBy { get; set; }
    }
}

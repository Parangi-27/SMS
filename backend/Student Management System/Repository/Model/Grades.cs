using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Repository.Model
{
    public enum ClassLevels
    {
        Class1 = 1,
        Class2,
        Class3,
        Class4,
        Class5,
        Class6,
        Class7,
        Class8,
        Class9,
        Class10,
        Class11,
        Class12,
    }
    public class Grades
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

        [Required(ErrorMessage = "Marks are required.")]
        public int Marks { get; set; }

        [Required(ErrorMessage = "Total Marks are required.")]
        public int TotalMarks { get; set; }

        [Required(ErrorMessage = "Exam Month is required.")]
        [Column(TypeName = "date")]
        public DateTime ExamMonth { get; set; }

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

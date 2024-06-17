using System.ComponentModel.DataAnnotations;

namespace Repository.Model
{
    public class Subject
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Subject Name is required.")]
        [MaxLength(50, ErrorMessage = "Subject Name cannot exceed 50 characters.")]
        public string SubjectName { get; set; }
    }
}

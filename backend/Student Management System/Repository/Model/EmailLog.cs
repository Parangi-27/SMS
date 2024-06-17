using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Repository.Model
{
    public class EmailLog
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string RecipientEmail { get; set; }
        public string? CC { get; set; }
        [Required]
        public string Subject { get; set; }
        [Required]
        public DateTime SentDate { get; set; }

    }
}

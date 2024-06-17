using System.ComponentModel.DataAnnotations;

namespace Service.DTO
{
    public class EmailLogDTO
    {
        public List<string> RecipientEmail { get; set; }
        public List<string>? CC { get; set; }
        public string Subject { get; set; }
        public DateTime SentDate { get; set; }
    }
}

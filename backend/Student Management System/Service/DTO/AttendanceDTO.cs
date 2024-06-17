using Repository.Model;

namespace Service.DTO
{
    public class AttendanceDTO
    {
        public int? Id { get; set; }
        public int StudentId { get; set; }
        public string StudentName{ get; set; }
        public int SubjectId { get; set; }
        public ClassLevels? ClassLevel { get; set; }
        public DateTime? Date { get; set; }
        public bool IsPresent { get; set; }
    }
}

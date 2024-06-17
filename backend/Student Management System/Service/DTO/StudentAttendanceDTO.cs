namespace Service.DTO
{
    public class StudentAttendanceDTO
    {
        public bool IsPresent { get; set; }
        public int SubjectId { get; set; }
        public DateTime Date { get; set; }
    }
}

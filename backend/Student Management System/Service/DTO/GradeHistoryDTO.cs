namespace Service.DTO
{
    public class GradeHistoryDTO
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public int SubjectId { get; set; }
        public CommonGradeDTO GradeDetails { get; set; }
    }

}

namespace Service.DTO
{
    public class StudentDetailsWithGradesDTO
    {
        public StudentDTO StudentDetails { get; set; }
        public CommonGradeDTO GradeDetails { get; set; }
        public double Percentage { get; set; }
    }

}

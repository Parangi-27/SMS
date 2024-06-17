using Repository.Model;

namespace Service.DTO
{
    public class GradesDTO
    {
    
        public int StudentId { get; set; }
        public ClassLevels? Class { get; set; }
        public int SubjectId { get; set; }
        public CommonGradeDTO GradeDetails { get; set; }
    }
}

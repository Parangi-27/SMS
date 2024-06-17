using Repository.Model;

namespace Service.DTO
{
    public class StudentDTO
    {
       public CommonStudentDTO Student { get; set; }
       public ClassLevels Class { get; set; }
    }
}

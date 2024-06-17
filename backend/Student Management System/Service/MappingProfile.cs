using AutoMapper;
using Repository.Model;
using Service.DTO;

namespace Service
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Users, TeacherDTO>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth))
            .ForMember(dest => dest.DateOfEnrollment, opt => opt.MapFrom(src => src.DateOfEnrollment)).ReverseMap();

            CreateMap<Teacher, TeacherDTO>()
            .ForMember(dest => dest.Class, opt => opt.MapFrom(src => src.Class))
            .ForMember(dest => dest.SubjectId, opt => opt.MapFrom(src => src.SubjectId))
            .ForMember(dest => dest.Salary, opt => opt.MapFrom(src => src.Salary))
            .ForMember(dest => dest.Qualification, opt => opt.MapFrom(src => src.Qualification)).ReverseMap();

            CreateMap<Users, StudentDTO>()
            .ForPath(dest => dest.Student.Name, opt => opt.MapFrom(src => src.Name))
            .ForPath(dest => dest.Student.Email, opt => opt.MapFrom(src => src.Email))
            .ForPath(dest => dest.Student.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth))
            .ForPath(dest => dest.Student.DateOfEnrollment, opt => opt.MapFrom(src => src.DateOfEnrollment))
            .ReverseMap();

            CreateMap<Student, StudentDTO>()
            .ForPath(dest => dest.Student.RollNumber, opt => opt.MapFrom(src => src.RollNumber))
            .ForMember(dest => dest.Class, opt => opt.MapFrom(src => src.Class)).ReverseMap();

            CreateMap<Student, CommonStudentDTO>()
            .ForMember(dest => dest.RollNumber, opt => opt.MapFrom(src => src.RollNumber)).ReverseMap();

            CreateMap<AttendanceDTO, Attendance>()
            .ForMember(dest => dest.StudentId, opt => opt.MapFrom(src => src.StudentId))
            //.ForMember(dest => dest.TeacherId, opt => opt.MapFrom(src => src.TeacherId))
            .ForMember(dest => dest.SubjectId, opt => opt.MapFrom(src => src.SubjectId))
            //.ForMember(dest => dest.ClassLevel, opt => opt.MapFrom(src => src.ClassLevels))
            .ForMember(dest => dest.isPresent, opt => opt.MapFrom(src => src.IsPresent))
            .ForMember(dest => dest.Date, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedOn, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore()).ReverseMap();

            CreateMap<GradesDTO, Grades>()
                .ForMember(dest => dest.StudentId, opt => opt.MapFrom(src => src.StudentId))
                .ForMember(dest => dest.Marks, opt => opt.MapFrom(src => src.GradeDetails.MarksObtained))
                .ForMember(dest => dest.TotalMarks, opt => opt.MapFrom(src => src.GradeDetails.TotalMarks))
                .ForMember(dest => dest.ExamMonth, opt => opt.MapFrom(src => src.GradeDetails.Date)).ReverseMap();

            CreateMap<Users, UserDTO>().ReverseMap();
        }
    }
}

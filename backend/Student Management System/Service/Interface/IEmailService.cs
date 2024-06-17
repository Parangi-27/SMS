using Service.DTO;

namespace Service.Interface
{
    public interface IEmailService
    {
        Task<bool> SendEmail(EmailDTO emailDTO);
        Task<bool> SendAbsentEmail(EmailDTO emailDTO);
    }
}

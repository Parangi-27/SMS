using Repository.Model;

namespace Repository.Interface
{
    public interface IEmailLogRepo
    {
        Task AddEmailLog(EmailLog emailLog);
    }
}

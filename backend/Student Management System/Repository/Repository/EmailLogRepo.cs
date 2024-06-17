using Repository.Interface;
using Repository.Model;

namespace Repository.Repository
{
    public class EmailLogRepo : IEmailLogRepo
    {
        #region props
        public readonly AppDbContext _context;

        #endregion

        #region ctor
        public EmailLogRepo(AppDbContext context)
        {
            _context = context;
        }
        #endregion

        public async Task AddEmailLog(EmailLog emailLog)
        {
            await _context.EmailLogs.AddAsync(emailLog);
            await _context.SaveChangesAsync();
            
        }
    }
}

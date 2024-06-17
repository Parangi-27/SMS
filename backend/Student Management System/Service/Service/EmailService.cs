using Microsoft.Extensions.Configuration;
using Repository.Interface;
using Repository.Model;
using Service.DTO;
using Service.Interface;
using System.Net;
using System.Net.Mail;

namespace Service.Service
{
    public class EmailService : IEmailService
    {
        #region props
        private readonly IConfiguration _config;
        private readonly ITeacherRepo _teacherRepo;
        private readonly IEmailLogRepo _emailLogRepo;
        #endregion

        #region ctor
        public EmailService(IConfiguration config, ITeacherRepo teacherRepo, IEmailLogRepo emailLogRepo)
        {
            _config = config;
            _teacherRepo = teacherRepo;
            _emailLogRepo = emailLogRepo;
        }

        #endregion

        #region send mail
        public async Task<bool> SendEmail(EmailDTO emailDTO)
        {
            string smtpClient = _config["EmailService:smtpClient"];
            int smtpPort = int.Parse(_config["EmailService:smtpPort"]);
            string emailFrom = _config["EmailService:emailFrom"];
            string emailPass = _config["EmailService:emailPass"];

            try
            {
                using (SmtpClient client = new SmtpClient(smtpClient, smtpPort))
                {
                    client.EnableSsl = true;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(emailFrom, emailPass);

                    using (MailMessage mailMessage = new MailMessage())
                    {
                        mailMessage.From = new MailAddress(emailFrom);
                        foreach (var email in emailDTO.Email)
                        {
                            mailMessage.To.Add(email);
                        }
                        mailMessage.Subject = emailDTO.Subject;
                        mailMessage.IsBodyHtml = true;
                        mailMessage.Body = emailDTO.Body;

                        var emailLogs = emailDTO.Email.Select(email => new EmailLog
                        {
                            RecipientEmail = email,
                            Subject = emailDTO.Subject,
                            SentDate = DateTime.UtcNow,
                        }).ToList();

                        // Assuming AddEmailLogs accepts a list of EmailLog objects
                        foreach (var email in emailDTO.Email)
                        {
                            var emailLog = new EmailLog
                            {
                                RecipientEmail = email,
                                Subject = emailDTO.Subject,
                                SentDate = DateTime.UtcNow,
                            };

                            await _emailLogRepo.AddEmailLog(emailLog);
                        }
                        await client.SendMailAsync(mailMessage);
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        #endregion

        #region send mail
        public async Task<bool> SendAbsentEmail(EmailDTO emailDTO)
        {
            string smtpClient = _config["EmailService:smtpClient"];
            int smtpPort = int.Parse(_config["EmailService:smtpPort"]);
            string emailFrom = _config["EmailService:emailFrom"];
            string emailPass = _config["EmailService:emailPass"];

            try
            {
                using (SmtpClient client = new SmtpClient(smtpClient, smtpPort))
                {
                    client.EnableSsl = true;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(emailFrom, emailPass);

                    using (MailMessage mailMessage = new MailMessage())
                    {
                        mailMessage.From = new MailAddress(emailFrom);
                        foreach (var email in emailDTO.Email)
                        {
                            mailMessage.To.Add(email);
                        }

                        // Split the CC string into individual email addresses
                        var emailLogs = emailDTO.Email.Select(email => new EmailLog
                        {
                            RecipientEmail = email,
                            Subject = emailDTO.Subject,
                            SentDate = DateTime.UtcNow,
                        }).ToList();
                        foreach (var cc in emailDTO.CC)
                        {
                            foreach (var emailLog in emailLogs)
                            {
                                emailLog.CC = cc;
                            }
                        }
                        mailMessage.Subject = emailDTO.Subject;
                        mailMessage.IsBodyHtml = true;
                        mailMessage.Body = emailDTO.Body;

                        foreach (var emailLog in emailLogs)
                        {
                            await _emailLogRepo.AddEmailLog(emailLog);
                        }
                        await client.SendMailAsync(mailMessage);
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                //Handle exceptions or log them as needed
                return false;
            }
        }

        #endregion
    }
}

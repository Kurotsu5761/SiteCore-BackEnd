using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace SiteCore_BackEnd.Common
{
    public class MailService : IMailService
    {
        private string _sender;
        private string _server;

        public MailService(string sender, string server)
        {
            _sender = sender;
            _server = server;
        }

        public void Send (string userEmail, string title, string message)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient(_server);

                mail.From = new MailAddress(userEmail);
                mail.To.Add(userEmail);
                mail.Subject = title;
                mail.Body = message;

                SmtpServer.Port = 25;
                SmtpServer.Credentials = new System.Net.NetworkCredential("me", "password");
                SmtpServer.EnableSsl = true;

                SmtpServer.Send(mail);
            }
            catch
            {
                throw;
            }

        }
    }
}

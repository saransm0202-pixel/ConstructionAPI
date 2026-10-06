using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace SSConstructions.Repository.Utility
{
    public static class Email
    {
        public static async Task SendEmail(string toEmail, string subject, string body, string accountName = "Constructions")
        {
            await SendEmail(toEmail, subject, body, accountName, null, null);
        }

        public static async Task SendEmail(string toEmail, string subject, string body, string accountName, byte[]? attachmentBytes, string? attachmentName)
        {
            try
            {
                using (var smtpClient = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtpClient.EnableSsl = true;
                    smtpClient.Credentials = new NetworkCredential("inferotech02@gmail.com", "xlyd hgkk kzmx geak"); 

                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtpClient.EnableSsl = true;
                    MailMessage mail = new MailMessage();

                    //Setting From , To and CC
                    mail.IsBodyHtml = true;
                    mail.From = new MailAddress("inferotech02@gmail.com", accountName);
                    var emailList = toEmail.Split(',', StringSplitOptions.RemoveEmptyEntries);

                    foreach (var email in emailList)
                    {
                        mail.To.Add(email.Trim());
                    }

                    mail.Subject = subject;
                    mail.Body = body;

                    if (attachmentBytes != null && attachmentBytes.Length > 0)
                    {
                        var stream = new MemoryStream(attachmentBytes);
                        var attachment = new Attachment(
                            stream,
                            string.IsNullOrWhiteSpace(attachmentName) ? "Attachment.pdf" : attachmentName,
                            "application/pdf");
                        mail.Attachments.Add(attachment);
                    }

                    await smtpClient.SendMailAsync(mail);
                }
            }
            catch (Exception ex) 
            {
                throw;
            }
        }

    }
}

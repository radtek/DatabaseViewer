using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace GA_SMO
{
    public static class GA_Mail
    {
        private static string _Mail_Body;

        public static string Mail_Body
        {
            get { return Mail_Body; }
            set { Mail_Body = value; }
        }
        
        public static void MailErrorMessage(string Module, Exception ex)
        {
            using (MailMessage mail = new MailMessage())
            {
                // Initialize the Mail properties
                mail.From = new MailAddress("DoNotReply@groupassociates.com");
                mail.Subject = "DB Object Scripting";

                mail.To.Add("gai_dba@groupassociates.com");

                mail.Body = BuildExceptionBody(Module, ex);
                mail.IsBodyHtml = true;

                string IPAddress = Settings1.Default.SMTPIPAddress;
                SmtpClient smtp = new SmtpClient(IPAddress, 25);

                // Send the message
                smtp.Send(mail);
            }
        }

        private static string BuildExceptionBody(string Module, Exception ex)
        {
           string InnerException = string.Empty;

           if (!(ex.InnerException == null))
              InnerException = ex.InnerException.Message;
           
            return string.Format(@"
            <html>
            <body>
              <h1>An Error Has Occurred!</h1>
              <table cellpadding=""5"" cellspacing=""0"" border=""1"">
              <tr>
              <td text-align: right;font-weight: bold"">Module:</td>
              <td>{0}</td>
              </tr>
              <tr>
              <td text-align: right;font-weight: bold"">Exception Type:</td>
              <td>{1}</td>
              </tr>
              <tr>
              <td text-align: right;font-weight: bold"">Message:</td>
              <td>{2}</td>
              </tr>
              <tr>
              <td text-align: right;font-weight: bold"">Inner Exception:</td>
              <td>{3}</td>
              </tr>
              <tr>
              <td text-align: right;font-weight: bold"">Stack Trace:</td>
              <td>{4}</td>
              </tr>
              </table>
            </body>
            </html>",
                    Module,
                    ex.GetType(),
                    ex.Message,
                    InnerException,
                    ex.StackTrace.Replace(Environment.NewLine, "<br />"));
        }

    }
}
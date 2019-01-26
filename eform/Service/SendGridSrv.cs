using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Configuration;

namespace eform.Service
{
    public class SendGridSrv
    {
        public static async Task sendEmail(List<EmailAddress> mailList ,string subject, string content)
        {
            //var apiKey = Environment.GetEnvironmentVariable("NAME_OF_THE_ENVIRONMENT_VARIABLE_FOR_YOUR_SENDGRID_KEY");
            var apiKey = "";
            apiKey=ConfigurationManager.AppSettings["sendgrid"].ToString();
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("sys@sys.com", "系統帳號");
            //var to = new EmailAddress("hahalin@gmail.com", "Frank 林");
            //var plainTextContent = "測試內容";
            //var htmlContent = "測試內容<strong>測試內容</strong>";
            var msg = MailHelper.CreateSingleEmailToMultipleRecipients(from, mailList, subject, content, content, false);
            //var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
        }
    }
}

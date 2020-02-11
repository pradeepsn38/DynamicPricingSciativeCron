using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Configuration;

namespace DyanmicPricingTripCron.Util
{
    public class Email
    {
        public static void SendMail(string strMessage)
        {
            MailMessage message = null;
            try
            {

                string smtpServer = ConfigurationManager.AppSettings["SMTPServer"];
                string smtpPort = ConfigurationManager.AppSettings["SMTPPort"];
                string tyEmailId = ConfigurationManager.AppSettings["EmailId"];
                string tyEmailPassword = ConfigurationManager.AppSettings["EmailPassword"];
                string strTo = ConfigurationManager.AppSettings["EmailTo"];

                SmtpClient smtp = new SmtpClient(smtpServer, Convert.ToInt32(smtpPort));
                message = new MailMessage();

                smtp.UseDefaultCredentials = false;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Timeout = 60000;
                smtp.EnableSsl = true;
                smtp.Credentials = new System.Net.NetworkCredential(tyEmailId, tyEmailPassword);
                message.From = new MailAddress(tyEmailId);
                string[] strArrEmails = strTo.Split(',');
                foreach (string strToEmail in strArrEmails)
                {
                    if (strToEmail != "")
                        message.To.Add(new MailAddress(strToEmail));
                }


                message.BodyEncoding = System.Text.Encoding.UTF8;
                message.Body = strMessage;
                message.IsBodyHtml = true;
                message.Subject = "Alert :: Sciative Trip Refresh Status";

                smtp.Send(message);

            }
            catch (Exception ex)
            {
                Logger.WriteLog("SendMail : " + ex.Message);
            }
            finally
            {
                if (message != null)
                    message.Dispose();
            }
        }

    }
}

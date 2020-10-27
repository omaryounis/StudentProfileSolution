using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace Dashboard_StudentProfile.Cls
{
    public class CustomMailMessage
    {
        static void _SendMail(string _MailTo, string _Subject, string _Body)
        {
            var message = new System.Net.Mail.MailMessage();
            message.To.Add(new System.Net.Mail.MailAddress(_MailTo));
            message.From = new System.Net.Mail.MailAddress(ConfigurationManager.AppSettings["Mail"]);
            message.Subject = _Subject;
            message.Body = _Body;
            message.IsBodyHtml = true;
            message.Priority = MailPriority.High;

            using (var smtp = new System.Net.Mail.SmtpClient())
            {
                smtp.UseDefaultCredentials = false;

                smtp.Credentials = new System.Net.NetworkCredential
                {
                    UserName = ConfigurationManager.AppSettings["Mail"],
                    Password = ConfigurationManager.AppSettings["Password"]
                };

                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.EnableSsl = true;
                smtp.Send(message);
            }
        }

        public void SendMail(string _MailTo, string _Subject, string _Body)
        {
            _SendMail(_MailTo, _Subject, _Body);
        }

        public void SendMailConfirmationCode(string _MailTo, string _Name, string _Code)
        {
            _SendMail(
                _MailTo,
                string.Format(@"{0} .. تأكيد البريد الالكترونى", ConfigurationManager.AppSettings["ApplicationName"]),
                string.Format(@"<span style='text-align:center;'>
                <p>مرحبا <b>{0}</b></p>
                <p>: لتاكيد بيانات <b>{0}</b> كود التفعيل الخاصة بالمستخدم </p>
                <p><b>{1}</b></p>
                </span>
                ", _Name, _Code)
                );
        }


    }
}
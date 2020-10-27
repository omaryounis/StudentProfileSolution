using StudentProfile.DAL.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace StudentProfile.Components
{
    public class CustomMailMessage
    {
        static void _SendMail(string _MailTo, string _Subject, string _Body)
        {
            var message = new MailMessage();
            message.To.Add(new MailAddress(_MailTo));
            message.From = new MailAddress(ConfigurationManager.AppSettings["Mail"]);
            message.Subject = _Subject;
            message.Body = _Body;
            message.IsBodyHtml = true;
            message.Priority = MailPriority.High;

            using (var smtp = new SmtpClient())
            {
                smtp.UseDefaultCredentials = false;

                smtp.Credentials = new NetworkCredential
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
    public class SendMessage
    {
        private SchoolAccGam3aEntities dbSC = new SchoolAccGam3aEntities();
        public bool _SendMessage(string Message, string PhoneNumber)
        {
            try
            {
                //Student Data 
                var UserID = int.Parse(HttpContext.Current.Session["UserId"].ToString());
                var StData = dbSC.INTEGRATION_All_Students.FirstOrDefault(x => x.STUDENT_ID == UserID);

                if (PhoneNumber != null && PhoneNumber.Length > 8)
                {
                    PhoneNumber = PhoneNumber.Replace("+", "");
                    if (!PhoneNumber.StartsWith("966"))
                    {
                        PhoneNumber = "966" + PhoneNumber;
                    }

                    int? SchoolID = dbSC.StudentBasicData.Where(x => x.IDNumber == StData.NATIONAL_ID).FirstOrDefault()
                        ?.School_ID;
                    string sender = "iu-edu-s-AD";
                    string UserName = "iuedusa";
                    string Password = "135246";


                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create
                        ("http://www.csms.co/api/sendsms.php?username=" + UserName + "&password=" + Password +
                         "&lang=ar&numbers=" + PhoneNumber.Trim() + "&sender=" + sender + "&message=" + Message);
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    Stream responseStream = response.GetResponseStream();
                    StreamReader readStream = new StreamReader(responseStream, Encoding.UTF8);
                    string strSMSResponseString = readStream.ReadToEnd();
                    MessagesLog msgLog = new MessagesLog();
                    msgLog.MessageText = Message;
                    msgLog.National_Id = StData.NATIONAL_ID;
                    msgLog.StDetailID = dbSC.StDetails.Where(x => x.StudentBasicData.IDNumber == StData.NATIONAL_ID)
                        .FirstOrDefault()?.StDetailID;
                    msgLog.Student_Id = StData.STUDENT_ID;
                    msgLog.Student_Name = StData.STUDENT_NAME;
                    msgLog.Student_Mobile = PhoneNumber;
                    msgLog.SendDate = DateTime.Now;
                    dbSC.MessagesLog.Add(msgLog);
                }

                dbSC.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }
    public class VerificationCode
    {
        private Random Random = new Random();
        private const string Chars = "0123456789";
        private const int DefaultVerificationCodeLength = 5;

        public string Generate(int length)
        {
            return new string(Enumerable.Repeat(Chars, length)
              .Select(s => s[Random.Next(s.Length)]).ToArray());
        }

        public string Generate() => Generate(DefaultVerificationCodeLength);

    }
}

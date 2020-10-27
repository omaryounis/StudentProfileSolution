using StudentProfile.DAL.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace Dashboard_StudentProfile.Cls
{
    public class SendMessage
    {
        private SchoolAccGam3aEntities dbSC = new SchoolAccGam3aEntities();
        public bool _SendMessage(string Message, string PhoneNumber)
        {
            try
            {
                //Student Data 
                var UserID = int.Parse(HttpContext.Current.Session["UserId"].ToString());
                var StData = dbSC.INTEGRATION_All_Students.FirstOrDefault(x=>x.STUDENT_ID == UserID);

                    if (PhoneNumber != null && PhoneNumber.Length > 8)
                    {
                    PhoneNumber = PhoneNumber.Replace("+", "");
                        if (!PhoneNumber.StartsWith("966"))
                        {
                        PhoneNumber = "966" + PhoneNumber;
                        }

                        int? SchoolID = dbSC.StudentBasicData.Where(x => x.IDNumber == StData.NATIONAL_ID).FirstOrDefault()
                            ?.School_ID;
                        string sender = string.Empty;
                        string UserName = string.Empty;
                        string Password = string.Empty;
                        var dtschool = dbSC.usp_School_Select(SchoolID).CopyToDataTable();
                        if (dtschool.Rows.Count > 0)
                        {
                        sender = "iu-edu-s-AD";
                        UserName = "iuedusa";
                        Password = "135246";
                    }

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


}
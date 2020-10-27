using StudentProfile.DAL.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.WebPages;

using Newtonsoft.Json;
using StudentProfile.Components;
using StudentProfile.DAL.Models.VM;
using System.Configuration;
using CaptchaMvc.HtmlHelpers;
using StudentProfile.Components.Security;
using System.Threading.Tasks;

namespace StudentProfile.Controllers
{
    //  [RequireHttps]
    public class LoginController : Controller
    {
        //protected override void OnActionExecuting(ActionExecutingContext filterContext)
        //{
        //    var userIP = filterContext.RequestContext.HttpContext.Request.UserHostAddress;

        //    var privateKey = ConfigurationManager.AppSettings["ReCaptcha.PrivateKey"];

        //    if (string.IsNullOrWhiteSpace(privateKey))
        //        throw new InvalidKeyException("ReCaptcha.PrivateKey missing from appSettings");

        //    var postData = string.Format("&secret={0}&remoteip={1}&response={2}",
        //                                 privateKey,
        //                                 userIP,
        //                                 filterContext.RequestContext.HttpContext.Request.Form["g-recaptcha-response"]
        //                                );

        //    var postDataAsBytes = Encoding.UTF8.GetBytes(postData);

        //    // Create web request
        //    var request = WebRequest.Create("https://www.google.com/recaptcha/api/siteverify");
        //    request.Method = "POST";
        //    request.ContentType = "application/x-www-form-urlencoded";
        //    request.ContentLength = postDataAsBytes.Length;
        //    var dataStream = request.GetRequestStream();
        //    dataStream.Write(postDataAsBytes, 0, postDataAsBytes.Length);
        //    dataStream.Close();

        //    // Get the response.
        //    var response = request.GetResponse();

        //    using (dataStream = response.GetResponseStream())
        //    {
        //        using (var reader = new StreamReader(dataStream))
        //        {
        //            var responseFromServer = reader.ReadToEnd();
        //            var res = Newtonsoft.Json.JsonConvert.DeserializeObject<CaptchaResponse>(responseFromServer);
        //            if (!res.success)
        //                ((Controller)filterContext.Controller).ModelState.AddModelError("ReCaptcha", "Captcha words typed incorrectly");



        //        }
        //    }
        //}
        // GET: Login
        private SchoolAccGam3aEntities db = new SchoolAccGam3aEntities();

        public ActionResult Login(int? Id)
        {
            string userName = "";
            Session["StudentLogin"] = 0;
            DashBoard_Users newUser = new DashBoard_Users();


            //    ///////////////////////////////////////////////////
            if (Request.QueryString["ticket"]!=null && !Request.QueryString["ticket"].IsEmpty()) //check if url has ticket query string
            {

                //if ticket exist
                // is the ticket is valid 

                var ticketUrl = System.Configuration.ConfigurationManager.AppSettings["cas.serviceValidate.key"];
                WebRequest rq = WebRequest.Create(ticketUrl +
                                                  Request.QueryString["ticket"]);
                rq.Method = "GET";
                WebResponse rs = rq.GetResponse();
                System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                StreamReader rd = new StreamReader(rs.GetResponseStream());
                doc.LoadXml(rd.ReadToEnd());
                var UserNode = doc.GetElementsByTagName("cas:user")[0]; //get the userid
                rd.Close();
                rs.Close();
                if (UserNode != null)
                {
                    userName = UserNode.InnerText; //this will be the userid or student id
                    decimal id = -1;
                    decimal.TryParse(userName, out id);


                    DashBoard_Users user = db.DashBoard_Users.Where(x => x.Username == userName).ToList().Select(x => new DashBoard_Users
                    {
                        ID = x.ID,
                        CreatedDate = x.CreatedDate,
                        IsAdmin = x.IsAdmin,
                        IsStudent = false,
                        Password = x.Password,
                        Name = x.Name,
                        PasswordHash = x.PasswordHash,
                        LastChangeDatetime = x.LastChangeDatetime,
                        Username = x.Username,
                        AccID = x.AccID,
                        Mobile = x.Mobile
                    }).SingleOrDefault();
                    if (user == null)
                    {
                        INTEGRATION_All_Students student = db.INTEGRATION_All_Students.Where(x => x.STUDENT_ID == id).FirstOrDefault();
                        if (student == null)
                        {
                            Session["UserId"] = newUser;

                            return RedirectToAction("NoPermissions", "Login", newUser.ID);

                        }
                        user = new DashBoard_Users()
                        {
                            ID = student.ID,
                            CreatedDate = student.JOIN_DATE,
                            IsAdmin = false,
                            IsStudent = true,
                            Password = Passwordhelper.HashText(student.STUDENT_ID.ToString()),
                            Name = student.STUDENT_NAME,
                            PasswordHash = Passwordhelper.HashText(student.STUDENT_ID.ToString()),
                            LastChangeDatetime = DateTime.Now,
                            Username = student.STUDENT_ID.ToString(),
                            AccID = null,
                            Mobile = student.MOBILE_NO
                        };
                        Session["UserId"] = user;
                        return RedirectToAction("UpdateData", "Students", new { id = EncryptDecrypt.Encrypt(($"{userName}").ToString(), false).ToString() });
                    }
                    else
                    {
                        Session["UserId"] = user;

                        int? count = db.DashBoard_UserGroups.Where(x => x.User_ID == user.ID).Count();
                        if (count != null && count > 0)
                        {
                            return RedirectToAction("AdvancedSearch", "Students");
                        }

                        return RedirectToAction("NoPermissions", "Login", new { id = user.ID });
                    }
                }
            }

                var casloginService = System.Configuration.ConfigurationManager.AppSettings["cas.loginUrl"];
                //return RedirectToAction("Esol", "login");
                //no ticket in the url
                //redirect to single sign on page 
                var urlBuilder =
              new System.UriBuilder(Request.Url.AbsoluteUri)
              {
                  Path = Url.Action("Login", "Login"),
                  Query = null,
              };

                Uri uri = urlBuilder.Uri;
                string url = urlBuilder.ToString();
               return Redirect(casloginService +
                                          Server.UrlEncode(url));

        }

        [NoCache]

        public ActionResult Esol(int? Id)
        {

            // Session["UserId"] = null;

            if (TempData["Error"] != null)
            {
                ModelState.AddModelError("CodeError", TempData["Error"].ToString());
            }

            LoginViewModel user = new LoginViewModel();
            if (Id > 0)
            {
                var model = db.DashBoard_Users.Where(x => x.ID == Id).SingleOrDefault();


                user.ID = model.ID;
                user.Username = model.Username;
                user.Password = model.Password;




            }

            return View("Login", user);
        }


        [HttpPost]

        public ActionResult Login(LoginViewModel user)
        {

            if (ModelState.IsValid)
            {
                //if (CheckRecaptcha(Request))
                //{

                if (this.IsCaptchaValid("Validate your captcha"))
                {
                    DashBoard_Users model = new DashBoard_Users() { Username = user.Username, Password = user.Password };
                    var ReturnUser = Passwordhelper.Login(model);
                    if (ReturnUser != null && ReturnUser.ID > 0)
                    {
                        System.Web.HttpContext.Current.Session["UserId"] = ReturnUser;

                        if ((ReturnUser.LastChangeDatetime <= ReturnUser.CreatedDate))
                        {

                            return RedirectToAction("ChangePassword");
                        }
                        //if (user.Username == "85244" && user.Password == "85244")
                        //{
                        //    ReturnUser.IsStudent = true;
                        //    user.ID = 85244;
                        //}
                        if (ReturnUser.IsStudent == true)
                        {
                            return RedirectToAction("UpdateData", "Students", new { id = EncryptDecrypt.Encrypt(($"{ReturnUser.Username}").ToString(), false).ToString() });
                        }
                        return RedirectToAction("AdvancedSearch", "Students");
                        //Check For LastTime UserChange His Password 


                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "فشل التحقق");
                    return View("Login", new LoginViewModel());
                }

            }
            ModelState.AddModelError(string.Empty, "من فضلك تاكد من صحة بياناتك");
            return View("Login", new LoginViewModel());

            //}







            #region OldLogin Code

            //Session["StudentLogin"] = 0;

            //var HashedPassword = StudentProfile.Helpers.Passwordhelper.EncryptPassword(user.Password);
            //DashBoard_Users Authorized = db.DashBoard_Users
            //    .Where(x => x.Username == user.Username && x.Password == HashedPassword).SingleOrDefault();
            //try
            //{
            //    if (Authorized == null)
            //    {
            //        ModelState.AddModelError(string.Empty, "من فضلك تاكد من صحة بياناتك");
            //        return View("Login", new DashBoard_Users());
            //    }
            //    else
            //    {
            //        if (TempData["Error"] != null)
            //        {
            //            ModelState.AddModelError("CodeError", TempData["Error"].ToString());
            //        }
            //        if (user.Username == "361014563")
            //        {
            //            Session["StudentLogin"] = 1;
            //            Session["UserId"] = 361014563;
            //            Session["UserName"] = "عبدالله احمد حسن";
            //            return RedirectToAction("Index", "Home");
            //        }

            //        Session["UserId"] = Authorized.ID;
            //        Session["UserName"] = Authorized.Username;
            //        Session["Name"] = Authorized.Name;

            //        return RedirectToAction("AdvancedSearch", "Students");
            //    }
            //}
            //catch (Exception e)
            //{
            //    ModelState.AddModelError(string.Empty, "من فضلك تاكد من صحة بياناتك");
            //    return View("Login", new DashBoard_Users());
            //}
            #endregion OldLogin Code









        }
        [HttpPost]

        public ActionResult IsValidOldPassword(string OldPassword)
        {


            var usr = Session["UserId"] as DashBoard_Users;
            usr.Password = OldPassword;
            if (Passwordhelper.IsValidPassword(usr))
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }


        public bool CheckRecaptcha(HttpRequestBase req)
        {
            var userIP = req.UserHostAddress;

            var privateKey = ConfigurationManager.AppSettings["ReCaptcha.PrivateKey"];

            if (string.IsNullOrWhiteSpace(privateKey))
                throw new InvalidKeyException("ReCaptcha.PrivateKey missing from appSettings");

            var postData = string.Format("&secret={0}&remoteip={1}&response={2}",
                                         privateKey,
                                         userIP,
                                        req.Form["g-recaptcha-response"]
                                        );

            var postDataAsBytes = Encoding.UTF8.GetBytes(postData);

            // Create web request
            var request = WebRequest.Create(System.Configuration.ConfigurationManager.AppSettings["Captcha.ValidateUrl"]);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = postDataAsBytes.Length;
            var dataStream = request.GetRequestStream();
            dataStream.Write(postDataAsBytes, 0, postDataAsBytes.Length);
            dataStream.Close();

            // Get the response.
            var response = request.GetResponse();
            CaptchaResponse res;
            using (dataStream = response.GetResponseStream())
            {
                using (var reader = new StreamReader(dataStream))
                {
                    var responseFromServer = reader.ReadToEnd();
                    res = Newtonsoft.Json.JsonConvert.DeserializeObject<CaptchaResponse>(responseFromServer);





                }
            }
            return res.success;
        }

        public ActionResult IsAuthorized(int? id, FormCollection form)
        {
            string userCode = form["code"];
            string systemCode = TempData["Code"].ToString();
            if (userCode == systemCode)
            {
                return RedirectToAction("Index", "Students");
            }
            else
            {
                DashBoard_Users modelitem = db.DashBoard_Users.Where(x => x.ID == id).SingleOrDefault();
                TempData["Error"] = "الكود الذي ادخلته غير صحيح";
                return RedirectToAction("Login", new { Id = id });
            }
        }

        public ActionResult LogOut()
        {
            Session["UserId"] = null;
            Session.Clear();
            Session.RemoveAll();

            //  Session Fixation


            //if (Request.Cookies["ASP.NET_SessionId"] != null)
            //{
            //    Response.Cookies["ASP.NET_SessionId"].Value = string.Empty;
            //    Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddMonths(-10);
            //}

            //if (Request.Cookies["AuthenticationToken"] != null)
            //{
            //    Response.Cookies["AuthenticationToken"].Value = string.Empty;
            //    Response.Cookies["AuthenticationToken"].Expires = DateTime.Now.AddMonths(-10);
            //}


            return Redirect(System.Configuration.ConfigurationManager.AppSettings["cas.loginUrl"] +
                            Server.UrlEncode(System.Configuration.ConfigurationManager.AppSettings["StudentDashboard.PublishUrl"]));
        }
        public ActionResult NoPermissions(int? id)
        {
            //if (Session["UserId"] == null)
            //{
            //    RedirectToAction("Login", "Login");
            //}

            if (id != null && id > 0)
            {
                int? count = db.DashBoard_UserGroups.Where(x => x.User_ID == id).Count();
                if (count != null && count > 0)
                {
                    return RedirectToAction("Index", "Students");
                }
            }

            ViewBag.Message = "عفوا هذا المستخدم ليس لديه أي صلاحيات برجاء الرجوع لإدارة النظام";
            return View("../Security/NoPermissions");
                
                
        }
        public string GenerateCode()
        {
            string PasswordLength = "4";
            string NewPassword = "";
            string allowedChars = "";

            allowedChars = "1,2,3,4,5,6,7,8,9,0";

            //allowedChars += "A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z,";

            //allowedChars += "a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z,";
            char[] sep = { ',' };
            string[] arr = allowedChars.Split(sep);
            string temp = "";
            Random rand = new Random();
            for (int i = 0; i < Convert.ToInt32(PasswordLength); i++)
            {
                temp = arr[rand.Next(0, arr.Length)];
                NewPassword += temp;
            }

            TempData["Code"] = NewPassword;
            return NewPassword;
        }

        public void ResendCode(string mobile)
        {
            string code = GenerateCode();
            send_Message(mobile, code);
        }

        public void send_Message(string Numbers, string code)
        {
            try
            {
                string sender = "esol";
                string UserName = "aboabdo";
                string Password = "123456";
                string message = "كود التأكيد الخاص بك هو: " + code + "";

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create
                (System.Configuration.ConfigurationManager.AppSettings["SMS.Service.Url"] + "?username=" + UserName + "&password=" + Password +
                 "&lang=ar&numbers=" + Numbers.Trim() + "&sender=" + sender + "&message=" + message);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream responseStream = response.GetResponseStream();
                StreamReader readStream = new StreamReader(responseStream, Encoding.UTF8);
                string strSMSResponseString = readStream.ReadToEnd();
            }
            catch (Exception ex)
            {
            }
        }



        public ActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordViewModel model)
        {

            if (ModelState.IsValid)
            {
                var usr = Session["UserId"] as DashBoard_Users;
                model.ID = usr.ID;
                model.UserName = usr.Username;
                var result = Passwordhelper.SaveChangePassword(model);
                if (result > 0)
                {
                    return RedirectToAction("Esol", "Login");
                }

                return RedirectToAction("Esol", "Login");
            }
            ModelState.AddModelError(string.Empty, "من فضلك تاكد من صحة بياناتك");
            return View("ChangePassword", new ChangePasswordViewModel());

        }


        //protected override void OnException(ExceptionContext filterContext)
        //{
        //    filterContext.ExceptionHandled = true;




        //    //Redirect or return a view, but not both.
        //    filterContext.Result = RedirectToAction("Index", "ErrorHandler");
        //    // OR 
        //    filterContext.Result = new ViewResult
        //    {
        //        ViewName = "~/Views/ErrorHandler/Index.cshtml"
        //    };
        //}
    }
}
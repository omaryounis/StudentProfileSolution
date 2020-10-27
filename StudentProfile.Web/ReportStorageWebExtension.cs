using System;
using StudentProfile.DAL.Models;
using DevExpress.XtraReports.UI;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Data;
using StudentProfile.Components;
using System.Data.SqlClient;

namespace StudentProfile
{
    public class ReportStorageWebExtension : DevExpress.XtraReports.Web.Extensions.ReportStorageWebExtension
    {
    
        private readonly string _reportsTemplatesPath =
            System.Web.HttpContext.Current.Server.MapPath("~/Reports/Templates/");

        public override bool CanSetData(string url)
        {
            int id;
            if (int.TryParse(url, out id))
            {
                ReportsTemplates report = GetReport(id);
                if (report != null)
                {
                    return true;
                }
            }

            return false;
        }
    
        public override bool IsValidUrl(string url)
        {
            int id;
            return int.TryParse(url, out id);
        }

        //OPEN
        public override byte[] GetData(string url)
        {
            int id;
            if (!int.TryParse(url, out id))
            {
                return null;
            }

            var reportTemplate = GetReport(id);
            if (reportTemplate == null)
            {
                return null;
            }

            using (var file = new FileStream($"{_reportsTemplatesPath}{reportTemplate.TemplatePath}", FileMode.Open,
                FileAccess.Read))
            {
                var ms = new MemoryStream();
                file.CopyTo(ms);
                return ms.ToArray();
            }
        }

        public override Dictionary<string, string> GetUrls()
        {
            var result = new Dictionary<string, string>();


            var currentUser = System.Web.HttpContext.Current.Session["UserId"] as DashBoard_Users;
            //using (var db = new SchoolAccGam3aEntities())
            //{
            foreach (var reportTemplate in GetAllReport(0, currentUser.ID))
                {
                    result.Add(reportTemplate.TemplatesId.ToString(), reportTemplate.TemplateName);
                }
           // }

            return result;
        }

        //SAVE
        public override void SetData(XtraReport report, string url)
        {
            int id;
            if (int.TryParse(url, out id))
            {
                var reportTemplate = GetReport(id);
                if (reportTemplate != null)
                {
                    using (var db = new SchoolAccGam3aEntities())
                    {
                        var guid = Guid.NewGuid().ToString();

                        using (var file = new FileStream($"{url}_{guid}", FileMode.Create,
                            FileAccess.Write))
                        {
                            report.SaveLayoutToXml(file);
                        }

                        db.SaveChanges();
                    }
                }
                // Error?
            }

            // Error?
        }

        //saveAS
        public override string SetNewData(XtraReport report, string defaultUrl)
        {
            try
            {
                var currentUser = System.Web.HttpContext.Current.Session["UserId"] as DashBoard_Users;

                var reportTemplate = new ReportsTemplates { TemplateName = defaultUrl };
                var guid = Guid.NewGuid().ToString();

                using (var file = new FileStream($"{_reportsTemplatesPath}{defaultUrl}_{guid}",
                    FileMode.Create, FileAccess.Write))
                {
                    report.SaveLayoutToXml(file);
                    reportTemplate.TemplatePath = $"{defaultUrl}_{guid}";
                }

                var ID = SaveReport(reportTemplate, currentUser.ID);
                //using (var db = new SchoolAccGam3aEntities())
                //{



                //    db.ReportsTemplates.Add(reportTemplate);
                //    db.SaveChanges();
                //}

                return ID.ToString();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }

        private ReportsTemplates GetReport(int id)
        {

           var currentUser= System.Web.HttpContext.Current.Session["UserId"] as DashBoard_Users;


            return GetReportById(id, currentUser.ID);

            //using (var db = new SchoolAccGam3aEntities())
            //{
            //    return db.ReportsTemplates.FirstOrDefault(r => r.TemplatesId == id);
            //}
        }


        private ReportsTemplates GetReportById(int id,int userId)
        {
            SqlParameter[] sqlParams = new SqlParameter[]
          {
                  new SqlParameter("@ReportId",id),
                  new SqlParameter("@UserID",userId)

          };

            StudentProfile.DAL.Models.ReportsTemplates user = new ReportsTemplates();
            DataSet Ds = SqlHelper.ExecuteDataset(SqlHelper.Gam3aConnectionString, "PR_GetReportById", sqlParams);
            if (Ds != null && Ds.Tables != null && Ds.Tables[0].Rows.Count >= 1)
            {
                    if (Ds.Tables[0].Rows[0]["TemplatesId"] != DBNull.Value)
                    {
                        user.TemplatesId = Convert.ToInt32(Ds.Tables[0].Rows[0]["TemplatesId"]);
                    }
                    if (Ds.Tables[0].Rows[0]["TemplatePath"] != DBNull.Value)
                    {
                        user.TemplatePath = Ds.Tables[0].Rows[0]["TemplatePath"].ToString();
                    }
                    if (Ds.Tables[0].Rows[0]["TemplateName"] != DBNull.Value)
                    {
                        user.TemplateName = Ds.Tables[0].Rows[0]["TemplateName"].ToString();
                    }
            }
            return user;
        }

        private List<ReportsTemplates> GetAllReport(int id, int userId)
        {
            SqlParameter[] sqlParams = new SqlParameter[]
          {
                 
                  new SqlParameter("@UserID",userId)

          };

           



            DataSet Ds = SqlHelper.ExecuteDataset(SqlHelper.Gam3aConnectionString, "PR_GetAllRepoerts", sqlParams);

            List<ReportsTemplates> list = new List<ReportsTemplates>();
            if (Ds != null && Ds.Tables != null && Ds.Tables[0].Rows.Count >= 1)
            {

                foreach (DataRow item in Ds.Tables[0].Rows)
                {
                    StudentProfile.DAL.Models.ReportsTemplates user = new ReportsTemplates();


                    if (item["TemplatesId"] != DBNull.Value)
                    {
                        user.TemplatesId = Convert.ToInt32(item["TemplatesId"]);
                    }
                    if (item["TemplatePath"] != DBNull.Value)
                    {
                        user.TemplatePath = item["TemplatePath"].ToString();
                    }

                    if (item["TemplateName"] != DBNull.Value)
                    {
                        user.TemplateName = item["TemplateName"].ToString();
                    }

                    list.Add(user);
                }




            }

            return list.OrderBy(r => r.TemplatesId).ToList();



        }

        private int SaveReport(ReportsTemplates rep,int UserID)
        {
            SqlParameter[] sqlParams = new SqlParameter[]
        {

                  new SqlParameter("@templateName",rep.TemplateName),
                  new SqlParameter("@templatePath",rep.TemplatePath),
                  new SqlParameter("@UserID",UserID)
                    

        };

            DataSet Ds = SqlHelper.ExecuteDataset(SqlHelper.Gam3aConnectionString, "PR_SaveReport", sqlParams);

            if(Ds!=null && Ds.Tables!=null && Ds.Tables[0].Rows.Count > 0)
            {
                return Convert.ToInt32(Ds.Tables[0].Rows[0][0]);
            }

            return 0;

          //  return Ds
            
        }

    }
}
using System;
using System.IO;
using DevExpress.Web;

namespace StudentProfile.Components
{
    public class UploadControlHelper
{
    public static readonly UploadControlValidationSettings ValidationSettings = new UploadControlValidationSettings
    {
        AllowedFileExtensions = new string[] {".jpg", ".jpeg", ".jpe"},
        MaxFileSize = 4000000
    };

    public static void uploadControl_FileUploadComplete(object sender, FileUploadCompleteEventArgs e)
    {
        if (e.UploadedFile.IsValid)
        {
            try
            {
                var name = e.UploadedFile.FileName;
                e.CallbackData = name;

                var idNumber = int.Parse(System.Web.HttpContext.Current.Session["IdNumber"].ToString());

                var filePathDirectory = $"~/Content/tempfiles/";
                var destinationPath = System.Web.HttpContext.Current.Server.MapPath(filePathDirectory);
                if (!Directory.Exists(destinationPath))
                {
                    Directory.CreateDirectory(destinationPath);
                }


                var dt = DateTime.Now.ToString("dd-MMM-yyyy");
                var generatedFileName = $"tempnotefile_{Path.GetExtension(e.UploadedFile.FileName)}";
                var resultFilePath = destinationPath + generatedFileName;
                e.UploadedFile.SaveAs(resultFilePath, true);
                System.Web.HttpContext.Current.Session["NoteFiles"] = e.UploadedFile.FileContent;
                e.CallbackData += "تم الحفظ بنجاح";
                e.ErrorText += "";

                //IUrlResolutionService urlResolver = sender as IUrlResolutionService;
                //if (urlResolver != null)
                //{
                //    e.CallbackData = urlResolver.ResolveClientUrl(resultFilePath) + "?refresh=" + Guid.NewGuid();
                //}
            }
            catch (Exception exception)
            {
                e.CallbackData += "";
                e.ErrorText += "عفواً حدث خطأ أثناء الحفظ برجاء تصحيح الأخطاء والمحاولة مرة أخرى";
            }
        }
    }
}
}
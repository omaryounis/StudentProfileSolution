using System;
using System.ComponentModel.DataAnnotations;
using DevExpress.Web;
using StudentProfile.DAL.Models.DataAnnotations;

namespace StudentProfile.DAL.Models.VM
{
    public class EmpDocumentVM
    {
        [ScaffoldColumn(true)]
        public int ID { get; set; }

        public int EmpID { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "حقل اجبارى")]
        [Display(Name = "نوع المستند")]
        public int? DocumentID { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "حقل اجبارى")]
        [Display(Name = "رقم المستند")]
        public string IdentityNum { get; set; }

        public string BarCode { get; set; }

        [StartEndDateValidator]
        [Required(AllowEmptyStrings = false, ErrorMessage = "حقل اجبارى")]
        [DataType(DataType.Date, ErrorMessage = "تاريخ خاطئ")]
        [Display(Name = "تاريخ الاصدار")]
        public DateTime? IssueDate { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "حقل اجبارى")]
        [DataType(DataType.Date, ErrorMessage = "تاريخ خاطئ")]
        [Display(Name = "تاريخ الانتهاء")]
        public DateTime? ExpDate { get; set; }

        public bool? IsActive { get; set; }


        [Display(Name = "صورة المستند"), DataType(DataType.ImageUrl), ScaffoldColumn(true)]
        public byte[] DocImagePath { get; set; }

        public UploadedFile[] DocImage { get; set; }
    }

    public static partial class MappingExtensions
    {
        //    public static EmpDocumentVM ToViewModel(this EmpDocument obj)
        //    {
        //        return new EmpDocumentVM()
        //        {
        //            ID = obj.ID,
        //            EmpID = obj.EmpID,
        //            DocumentID = obj.DocumentID,
        //            IdentityNum = obj.IdentityNum,
        //            DocImagePath = obj.DocImagePath,
        //            BarCode = obj.BarCode,
        //            IssueDate = obj.IssueDate,
        //            ExpDate = obj.ExpDate,
        //            IsActive = obj.IsActive
        //        };
        //    }

        //    public static EmpDocument ToDataModel(this EmpDocumentVM obj)
        //    {
        //        return new EmpDocument()
        //        {
        //            ID = obj.ID,
        //            EmpID = obj.EmpID,
        //            DocumentID = obj.DocumentID,
        //            IdentityNum = obj.IdentityNum,
        //            DocImagePath = obj.DocImagePath,
        //            BarCode = obj.BarCode,
        //            IssueDate = obj.IssueDate,
        //            ExpDate = obj.ExpDate,
        //            IsActive = obj.IsActive
        //        };
        //    }
    }
}
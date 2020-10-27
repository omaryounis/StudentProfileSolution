using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace StudentProfile.DAL.Models.VM
{
    public class EmpCourseVM
    //: IValidatableObject
    {
        [ScaffoldColumn(true)]
        public int ID { get; set; }

        [Display(Name = "رقم الطالب")]
        public Nullable<int> EmployeeID { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "اسم المشاركة مطلوب")]
        [Display(Name = "اسم المشاركة")]
        public string CourseName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "مكان المشاركة مطلوب")]
        [Display(Name = "مكان المشاركة")]
        public string CoursePlace { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "تاريخ المشاركة مطلوب")]
        [Display(Name = "تاريخ المشاركة"), DataType(DataType.Date)]
        public Nullable<System.DateTime> StartDate { get; set; }

        [Display(Name = "تاريخ انتهاء المشاركة"), DataType(DataType.Date)]
        public Nullable<System.DateTime> EndDate { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "عدد ساعات المشاركة مطلوب")]
        [Display(Name = "عدد ساعات المشاركة")]
        [Range(0, int.MaxValue, ErrorMessage = "الرجاء إدخال عدد ساعات المشاركة رقم صحيح")]
        public Nullable<int> CourseDays { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "نوع المشاركة مطلوب")]
        [Display(Name = "نوع المشاركة")]
        public Nullable<int> CourseTypes_ID { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "التقدير مطلوب")]
        [Display(Name = "التقدير")]
        public string Degree { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "النسبة المئوية مطلوب")]
        [Display(Name = "النسبة المئوية")]
        [Range(0, 100, ErrorMessage = "الرجاء إدخال النسبة المئوية رقم صحيح من 0 الي 100")]
        public Nullable<int> DegreePercentage { get; set; }

        [Display(Name = "المرفقات"), DataType(DataType.ImageUrl), ScaffoldColumn(true)]
        public string CourseImagePath { get; set; }

        public string CourseTypeName { get; set; }


        //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        //{
        //    if (EndDate <= StartDate)
        //    {
        //        yield return
        //          new ValidationResult(errorMessage: "تاريخ الانتهاء لابد ان يكون اكبر من تاريخ البداية",
        //                               memberNames: new[] { "EndDate" });
        //    }
        //}
    }

    public static partial class MappingExtensions
    {
        public static EmpCourseVM ToViewModel(this EmpCourses obj)
        {
            return new EmpCourseVM()
            {
                CourseDays = obj.CourseDays,
                EmployeeID = obj.EmployeeID,
                CourseName = obj.CourseName,
                CoursePlace = obj.CoursePlace,
                Degree = obj.Degree,
                EndDate = obj.EndDate,
                StartDate = obj.StartDate,
                ID = obj.ID,
                CourseImagePath = obj.CourseImagePath,
                CourseTypes_ID = obj.CourseTypes_ID,
                DegreePercentage = obj.DegreePercentage,
                CourseTypeName = obj.CourseTypes_ID != null ? obj.CourseTypes.Name : ""
            };
        }

        public static EmpCourses ToDataModel(this EmpCourseVM obj)
        {
            return new EmpCourses()
            {
                CourseDays = obj.CourseDays,
                EmployeeID = obj.EmployeeID,
                CourseName = obj.CourseName,
                CoursePlace = obj.CoursePlace,
                Degree = obj.Degree,
                EndDate = obj.EndDate,
                StartDate = obj.StartDate,
                ID = obj.ID,
                CourseImagePath = obj.CourseImagePath,
                CourseTypes_ID = obj.CourseTypes_ID,
                DegreePercentage = obj.DegreePercentage
            };
        }
    }
}
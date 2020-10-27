using System;
using System.ComponentModel.DataAnnotations;

namespace StudentProfile.DAL.Models.DataAnnotations
{
    public class StartEndDateValidator : ValidationAttribute
    {
        protected override ValidationResult
            IsValid(object value, ValidationContext validationContext)
        {
            EmpDocument model = (EmpDocument) validationContext.ObjectInstance;
            var expireDate = Convert.ToDateTime(model.ExpDate);
            var issuanceDate = Convert.ToDateTime(value);

            return issuanceDate > expireDate
                ? new ValidationResult("تاريخ الاصدار يجب ان يكون اقدم من تاريخ الانتهاء")
                : ValidationResult.Success;
        }
    }
}
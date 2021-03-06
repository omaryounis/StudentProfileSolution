﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace StudentProfile.DAL.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class HRMadinaEntities : DbContext
    {
        public HRMadinaEntities()
            : base("name=HRMadinaEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Absence> Absence { get; set; }
        public virtual DbSet<CourseTypes> CourseTypes { get; set; }
        public virtual DbSet<Documents> Documents { get; set; }
        public virtual DbSet<EmpCourses> EmpCourses { get; set; }
        public virtual DbSet<EmpDocument> EmpDocument { get; set; }
        public virtual DbSet<Employees> Employees { get; set; }
        public virtual DbSet<Relatives> Relatives { get; set; }
    
        public virtual ObjectResult<PR_EmpCourses_SelectAll_Result> PR_EmpCourses_SelectAll(Nullable<int> empID)
        {
            var empIDParameter = empID.HasValue ?
                new ObjectParameter("EmpID", empID) :
                new ObjectParameter("EmpID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<PR_EmpCourses_SelectAll_Result>("PR_EmpCourses_SelectAll", empIDParameter);
        }
    
        public virtual ObjectResult<PR_GetStudentsByFacandMaj_Result> PR_GetStudentsByFacandMaj(Nullable<int> facID, Nullable<int> majID, string selectedNames)
        {
            var facIDParameter = facID.HasValue ?
                new ObjectParameter("facID", facID) :
                new ObjectParameter("facID", typeof(int));
    
            var majIDParameter = majID.HasValue ?
                new ObjectParameter("majID", majID) :
                new ObjectParameter("majID", typeof(int));
    
            var selectedNamesParameter = selectedNames != null ?
                new ObjectParameter("selectedNames", selectedNames) :
                new ObjectParameter("selectedNames", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<PR_GetStudentsByFacandMaj_Result>("PR_GetStudentsByFacandMaj", facIDParameter, majIDParameter, selectedNamesParameter);
        }
    
        public virtual ObjectResult<PR_StudentDocuments_SelectAll_Result> PR_StudentDocuments_SelectAll(Nullable<int> studentID)
        {
            var studentIDParameter = studentID.HasValue ?
                new ObjectParameter("studentID", studentID) :
                new ObjectParameter("studentID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<PR_StudentDocuments_SelectAll_Result>("PR_StudentDocuments_SelectAll", studentIDParameter);
        }
    
        public virtual ObjectResult<PR_StudentsCourses_SelectAll_Result> PR_StudentsCourses_SelectAll()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<PR_StudentsCourses_SelectAll_Result>("PR_StudentsCourses_SelectAll");
        }
    
        public virtual ObjectResult<proc_CalcFingerPrintSheet_Result> proc_CalcFingerPrintSheet(Nullable<int> empID, Nullable<int> empFingerID, Nullable<System.DateTime> date, Nullable<int> month, Nullable<System.DateTime> dateFrom, Nullable<System.DateTime> dateTo, Nullable<int> sectionID, Nullable<int> yearID, Nullable<int> branchID, string name)
        {
            var empIDParameter = empID.HasValue ?
                new ObjectParameter("EmpID", empID) :
                new ObjectParameter("EmpID", typeof(int));
    
            var empFingerIDParameter = empFingerID.HasValue ?
                new ObjectParameter("EmpFingerID", empFingerID) :
                new ObjectParameter("EmpFingerID", typeof(int));
    
            var dateParameter = date.HasValue ?
                new ObjectParameter("Date", date) :
                new ObjectParameter("Date", typeof(System.DateTime));
    
            var monthParameter = month.HasValue ?
                new ObjectParameter("Month", month) :
                new ObjectParameter("Month", typeof(int));
    
            var dateFromParameter = dateFrom.HasValue ?
                new ObjectParameter("DateFrom", dateFrom) :
                new ObjectParameter("DateFrom", typeof(System.DateTime));
    
            var dateToParameter = dateTo.HasValue ?
                new ObjectParameter("DateTo", dateTo) :
                new ObjectParameter("DateTo", typeof(System.DateTime));
    
            var sectionIDParameter = sectionID.HasValue ?
                new ObjectParameter("SectionID", sectionID) :
                new ObjectParameter("SectionID", typeof(int));
    
            var yearIDParameter = yearID.HasValue ?
                new ObjectParameter("YearID", yearID) :
                new ObjectParameter("YearID", typeof(int));
    
            var branchIDParameter = branchID.HasValue ?
                new ObjectParameter("BranchID", branchID) :
                new ObjectParameter("BranchID", typeof(int));
    
            var nameParameter = name != null ?
                new ObjectParameter("Name", name) :
                new ObjectParameter("Name", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<proc_CalcFingerPrintSheet_Result>("proc_CalcFingerPrintSheet", empIDParameter, empFingerIDParameter, dateParameter, monthParameter, dateFromParameter, dateToParameter, sectionIDParameter, yearIDParameter, branchIDParameter, nameParameter);
        }
    
        public virtual ObjectResult<proc_EmpDocumentsDetails_Exp_Result> proc_EmpDocumentsDetails_Exp(Nullable<int> documentID, Nullable<int> userID)
        {
            var documentIDParameter = documentID.HasValue ?
                new ObjectParameter("DocumentID", documentID) :
                new ObjectParameter("DocumentID", typeof(int));
    
            var userIDParameter = userID.HasValue ?
                new ObjectParameter("UserID", userID) :
                new ObjectParameter("UserID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<proc_EmpDocumentsDetails_Exp_Result>("proc_EmpDocumentsDetails_Exp", documentIDParameter, userIDParameter);
        }
    
        public virtual ObjectResult<proc_EmpDocumentsDetails_Exp_New_Result> proc_EmpDocumentsDetails_Exp_New(Nullable<int> documentID)
        {
            var documentIDParameter = documentID.HasValue ?
                new ObjectParameter("DocumentID", documentID) :
                new ObjectParameter("DocumentID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<proc_EmpDocumentsDetails_Exp_New_Result>("proc_EmpDocumentsDetails_Exp_New", documentIDParameter);
        }
    
        public virtual ObjectResult<proc_FillMonths_Result> proc_FillMonths(Nullable<int> yearID)
        {
            var yearIDParameter = yearID.HasValue ?
                new ObjectParameter("YearID", yearID) :
                new ObjectParameter("YearID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<proc_FillMonths_Result>("proc_FillMonths", yearIDParameter);
        }
    
        public virtual ObjectResult<proc_GetAdvancesReport_ForEmp_Result> proc_GetAdvancesReport_ForEmp(Nullable<int> empID)
        {
            var empIDParameter = empID.HasValue ?
                new ObjectParameter("EmpID", empID) :
                new ObjectParameter("EmpID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<proc_GetAdvancesReport_ForEmp_Result>("proc_GetAdvancesReport_ForEmp", empIDParameter);
        }
    
        public virtual ObjectResult<proc_Select_SpecialDues_Temp_ForEmp_Result> proc_Select_SpecialDues_Temp_ForEmp(Nullable<int> empID)
        {
            var empIDParameter = empID.HasValue ?
                new ObjectParameter("EmpID", empID) :
                new ObjectParameter("EmpID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<proc_Select_SpecialDues_Temp_ForEmp_Result>("proc_Select_SpecialDues_Temp_ForEmp", empIDParameter);
        }
    }
}

//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class DocumentTypes
    {
        public int DocumentID { get; set; }
        public string DocumentName { get; set; }
        public Nullable<int> AlertDays { get; set; }
        public Nullable<bool> IsActive { get; set; }
    }
}
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
    
    public partial class Year
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public bool IsClosed { get; set; }
        public Nullable<int> SchoolID { get; set; }
        public Nullable<int> FSY_ID { get; set; }
        public Nullable<bool> IsNextYear { get; set; }
        public string Prefix { get; set; }
    }
}
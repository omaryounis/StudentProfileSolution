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
    
    public partial class Con_Company
    {
        public int ID { get; set; }
        public Nullable<int> CON_ID { get; set; }
        public Nullable<int> ComID { get; set; }
    
        public virtual COM COM { get; set; }
        public virtual Users Users { get; set; }
    }
}
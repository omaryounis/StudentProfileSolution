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
    
    public partial class Dashboard_FilterGroupUsers
    {
        public int filterGropupUserID { get; set; }
        public int filterGropupID { get; set; }
        public int userID { get; set; }
    
        public virtual Dashboard_FilterGroups Dashboard_FilterGroups { get; set; }
        public virtual DashBoard_Users DashBoard_Users { get; set; }
    }
}

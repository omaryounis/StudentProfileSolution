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
    
    public partial class Dashboard_SecretPermissions
    {
        public int PermissionId { get; set; }
        public int UserId { get; set; }
        public bool Secret { get; set; }
        public bool TopSecret { get; set; }
    
        public virtual DashBoard_Users DashBoard_Users { get; set; }
    }
}
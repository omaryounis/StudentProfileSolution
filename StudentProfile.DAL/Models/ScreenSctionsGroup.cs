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
    
    public partial class ScreenSctionsGroup
    {
        public int GroupScreenId { get; set; }
        public int GroupId { get; set; }
        public int ScreenActionId { get; set; }
    
        public virtual DashBoard_Groups DashBoard_Groups { get; set; }
        public virtual ScreenActions ScreenActions { get; set; }
    }
}

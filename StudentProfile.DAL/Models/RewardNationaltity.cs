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
    
    public partial class RewardNationaltity
    {
        public int ID { get; set; }
        public int RewardCategory_Id { get; set; }
        public int NationalityId { get; set; }
    
        public virtual RewardCategory RewardCategory { get; set; }
    }
}

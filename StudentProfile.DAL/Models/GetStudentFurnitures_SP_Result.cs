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
    
    public partial class GetStudentFurnitures_SP_Result
    {
        public int HousingFurnituresOfStudent_Id { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int FurnitureId { get; set; }
        public string FurnitureName { get; set; }
        public System.DateTime ReceivingDate { get; set; }
        public Nullable<System.DateTime> ReturnDate { get; set; }
        public int LocationId { get; set; }
    }
}
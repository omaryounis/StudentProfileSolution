using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentProfile.DAL.Models.VM
{
    public class HousingDetailsVM
    {
        public string CompanyName { get; set; }
        public string DepartementName { get; set; }
        public string FloorName { get; set; }
        public string SiteName { get; set; }
        public int HousingNumber { get; set; }
        public string HousingDate { get; set; }
        public int BedLocationId { get; set; }
        public string BedLocationCode { get; set; }
        public int SiteId { get; set; }
        public string LastUpdated { get; set; }
        public string OperationNo { get; set; }
        public string InsertionDate { get; set; }
        public string IsFamilial { get; set; }
        public string NotesOfHosing { get; set; }
    }
}

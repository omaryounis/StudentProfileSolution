using System;

namespace StudentProfile.DAL.Models
{
    public class MonthlyPremium
    {
        public int MonthId { get; set; }
        public DateTime Date { get; set; }
        public int Value { get; set; }
    }
}
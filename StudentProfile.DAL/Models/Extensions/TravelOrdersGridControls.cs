using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentProfile.DAL.Models
{
    public class TravelOrdersAmounts
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
    }

    public class TravelOrdersTiket
    {
        public int Id { get; set; }
        public string TicketNumber { get; set; }
        public decimal ApprovedAmount { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoRent.Models
{
    public class RentalDetails
    {
        public DateTime PickupDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public int ContractedKms { get; set; }
        public decimal RentalCost { get; set; }
        public string Note { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Interfaces;

namespace Model
{
    /// <summary>
    /// Class that represents the details of a rental.
    /// </summary>
    public class RentalDetails
    {        
        public Customer CurrentClient { get; set; }
        public DateTime PickupDate { get; set; }
        public DateTime ReturnDate { get; set; }        
        public int ContractedKms {get; set; }
        public decimal RentalCost { get; set; }
        public string Note { get; set; }
    }
}

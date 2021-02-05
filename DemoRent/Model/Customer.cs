using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    /// <summary>
    /// This class represents a possible extension of the application.
    /// Rentals should save their customer's data.
    /// </summary>
    public class Customer
    {
        public string Name { get; set; }
        public string NIF { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }        
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Interfaces
{
    /// <summary>
    /// An interface for all items that can be rented.
    /// Currently, only implemented by cars.
    /// </summary>
    public interface IRentable
    {
        decimal PricePerKm { get; set; }
        decimal PricePerMonth { get; set; }
        /// <summary>
        /// A flag indicating if the vehicle is booked in the current day.
        /// </summary>
        bool Booked { get; }
        /// <summary>
        /// A flag indicating if the vehicle is in the store (normally defined by the date of the car)
        /// </summary>
        bool InStore { get; }
        /// <summary>
        /// A list with all the rentals of the current vehicle.
        /// </summary>
        List<RentalDetails> Rentals { get; set; }
    }
}

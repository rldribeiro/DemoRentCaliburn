using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace DemoRent.Models
{
    public class CarModel
    {
        /// <summary>
        /// This class represents a rentable car.
        /// </summary>
        public CarModel()
        {
            this.Rentals = new List<RentalDetails>();
        }

        #region Properties

        // Car details
        public string Brand { get; set; }
        public string Model { get; set; }
        public string PhotoUrl { get; set; }
        public ImageSource Photo { get; set; }
        public int Year { get; set; }
        public int Kms { get; set; }

        // Car pricing
        public decimal PricePerKm { get; set; }
        public decimal PricePerMonth { get; set; }

        // Car renting
        public bool Booked
        {
            get
            {
                return CheckIfCarIsBooked();
            }
        }
        public bool InStore
        {
            get
            {
                if (Year == 0)
                    return false;
                else
                    return new DateTime(this.Year, 1, 1) < DateTime.Today;
            }
        }

        public List<RentalDetails> Rentals { get; set; }

        #endregion

        #region Public Methods

        public void CopyDetails(CarModel originCar)
        {
            this.Brand = originCar.Brand;
            this.Model = originCar.Model;
            this.PhotoUrl = originCar.PhotoUrl;
            this.Year = originCar.Year;

            this.Kms = originCar.Kms;
            this.PricePerKm = originCar.PricePerKm;
            this.PricePerMonth = originCar.PricePerMonth;

            this.Rentals = originCar.Rentals;
        }

        #endregion

        #region Overrides

        public override string ToString()
        {
            string booked = string.Empty;
            if (this.Booked)
                booked = "-- BOOKED --";

            return $"{Brand} {Model} ({Year}) {booked}";
        }

        #endregion

        #region Private Methods

        private bool CheckIfCarIsBooked()
        {
            for (int i = 0; i < this.Rentals.Count; i++)
            {
                RentalDetails rental = this.Rentals[i];
                if (DateTime.Today >= rental.PickupDate && DateTime.Today <= rental.ReturnDate)
                {
                    return true;
                }
            }

            return false;
        }

        #endregion
    }
}

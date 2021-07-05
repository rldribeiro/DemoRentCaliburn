using System.Linq;
using System.Threading.Tasks;

namespace DemoRent.ViewModels
{
    using Caliburn.Micro;
    using System;
    using System.Collections.Generic;
    using System.Reactive.Linq;
    using DemoRent.DataAccess;
    using DemoRent.Services;

    public class FlightsViewModel : Screen
    {
        public IEnumerable<FlightDTO> FlightsStatic { get;}

        public string LatestFlight { get; private set; }

        //public IObservable<FlightDTO> Flights => GetFlights();

        public FlightsViewModel()
        {
            FlightsStatic = new FlightDTO[]
            {
                new FlightDTO {AirLine = "TAP", FlightNumber = "12345"},
                new FlightDTO {AirLine = "Iberia", FlightNumber = "6789"},
                new FlightDTO {AirLine = "RyanAir", FlightNumber = "42"}
            };
            IterateFlights();

        }

        private async void IterateFlights()
        {
            var en = FlightsStatic.GetEnumerator();
            while (true)
            {
                if (!en.MoveNext())
                {
                    en.Reset();
                    en.MoveNext();
                }

                LatestFlight = en.Current.FlightNumber;

                NotifyOfPropertyChange(() => LatestFlight);
                await Task.Delay(500);
            }
        }
    }
}

using System;
using System.Net.Http;
using System.Reactive.Subjects;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace DemoRent.Services
{
    using System.Collections.Generic;
    using DemoRent.DataAccess;

    internal sealed class FlightScheduleProvider : IFlightScheduleProvider
    {
        private BehaviorSubject<FlightDTO> _mostRecentFlight;
        private List<FlightDTO> dummyFlights = new List<FlightDTO>{
            new FlightDTO { AirLine = "TAP", FlightNumber = "42" },
            new FlightDTO { AirLine = "Iberia", FlightNumber = "6789" },
            new FlightDTO { AirLine = "RyanAir", FlightNumber = "69" },
            new FlightDTO { AirLine = "Açoreana", FlightNumber = "1372" },
            new FlightDTO { AirLine = "Qatar", FlightNumber = "58" },
            new FlightDTO { AirLine = "Emirates", FlightNumber = "646" },
            new FlightDTO { AirLine = "Lufthansa", FlightNumber = "321" }
        };
        private int flightPos;

        public IObservable<FlightDTO> MostRecentFlight => _mostRecentFlight;

        public FlightScheduleProvider()
        {
            _mostRecentFlight = new BehaviorSubject<FlightDTO>(GetNewFlight());
            BeginFlightScanning();
        }

        private async void BeginFlightScanning()
        {
            while (true)
            {
                _mostRecentFlight.OnNext(GetNewFlight());
                await Task.Delay(2000);
            }
        }

        public FlightDTO GetNewFlight()
        {
            flightPos = flightPos == dummyFlights.Count - 1 ? 0 : flightPos + 1;
            return dummyFlights[flightPos];
        }
    }
}

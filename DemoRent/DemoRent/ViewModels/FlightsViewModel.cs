namespace DemoRent.ViewModels
{
    using Caliburn.Micro;
    using System;
    using System.Collections.Generic;
    using System.Reactive.Linq;
    using System.Threading.Tasks;
    using DemoRent.DataAccess;
    using DemoRent.Services;

    public class FlightsViewModel : PropertyChangedBase
    {
        private readonly IFlightScheduleProvider _flightScheduleProvider;
        private string _latestFlightNumber;

        public string LatestFlightNumber
        {
            get { return _latestFlightNumber; }
            set
            {
                if (_latestFlightNumber != value)
                {
                    _latestFlightNumber = value;
                    NotifyOfPropertyChange(() => LatestFlightNumber);
                };
            }
        }

        public FlightsViewModel(IFlightScheduleProvider flightScheduleProvider)
        {
            _flightScheduleProvider = flightScheduleProvider;
            flightScheduleProvider.MostRecentFlight.Subscribe(UpdateFlight);
        }

        private void UpdateFlight(FlightDTO flight)
        {
            LatestFlightNumber = $"{flight.AirLine} - {flight.FlightNumber}";
        }
    }
}

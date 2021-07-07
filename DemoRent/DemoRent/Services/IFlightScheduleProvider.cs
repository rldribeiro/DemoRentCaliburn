using System;

namespace DemoRent.Services
{
    using System.Collections.Generic;
    using DemoRent.DataAccess;

    public interface IFlightScheduleProvider
    {
        IObservable<FlightDTO> MostRecentFlight { get; }
    }
}

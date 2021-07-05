namespace DemoRent.Services
{
    using System.Collections.Generic;
    using DemoRent.DataAccess;

    public interface IFlightScheduleProvider
    {
        List<FlightDTO> GetFlights();
    }
}

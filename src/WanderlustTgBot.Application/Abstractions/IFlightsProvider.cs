namespace WanderlustTgBot.Application.Abstractions;

public interface IFlightsProvider
{
    Task<DepartureBoard> GetVisaFreeZoneFlights(FlightsSearchRequest searchRequest);
}
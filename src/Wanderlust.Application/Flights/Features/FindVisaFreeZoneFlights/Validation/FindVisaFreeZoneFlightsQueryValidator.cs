using FluentValidation;
using Wanderlust.Application.Flights.Features.FindVisaFreeZoneFlights.Queries;

namespace Wanderlust.Application.Flights.Features.FindVisaFreeZoneFlights.Validation;

public class FindVisaFreeZoneFlightsQueryValidator : AbstractValidator<FindVisaFreeZoneFlightsQuery>
{
    public FindVisaFreeZoneFlightsQueryValidator()
    {
        RuleFor(q => q.DepartureCode)
            .NotEmpty()
            .MaximumLength(3);

        RuleFor(q => q.GeoRegion)
            .IsInEnum();

        RuleFor(q => q.Date)
            .Must(date => date >= DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("Departure date cannot be in the past.");
    }
}
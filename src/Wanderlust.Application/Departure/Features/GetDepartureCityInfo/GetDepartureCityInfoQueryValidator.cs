using FluentValidation;

namespace Wanderlust.Application.Departure.Features.GetDepartureCityInfo;

public class GetDepartureCityInfoQueryValidator : AbstractValidator<GetDepartureCityInfoQuery>
{
    public GetDepartureCityInfoQueryValidator()
    {
        RuleFor(q => q.CityName).NotEmpty();
    }
}
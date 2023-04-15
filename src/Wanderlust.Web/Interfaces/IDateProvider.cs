namespace Wanderlust.Web.Interfaces;

public interface IDateProvider
{
    DateOnly Today { get; }
}
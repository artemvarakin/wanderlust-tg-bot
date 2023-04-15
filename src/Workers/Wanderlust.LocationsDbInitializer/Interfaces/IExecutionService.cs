namespace Wanderlust.LocationsDbInitializer.Interfaces;

public interface IExecutionService
{
    Task DoWorkAsync(CancellationToken cancellationToken);
}
namespace ModernWebApp.Domain;

public interface IGreetingRepository
{
    Task<Greeting> AddAsync(Greeting greeting, CancellationToken cancellationToken);

    Task<IReadOnlyList<Greeting>> ListAsync(CancellationToken cancellationToken);
}
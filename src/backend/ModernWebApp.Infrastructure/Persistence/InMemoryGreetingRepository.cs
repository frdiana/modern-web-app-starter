using System.Collections.Concurrent;
using ModernWebApp.Domain;

namespace ModernWebApp.Infrastructure.Persistence;

internal sealed class InMemoryGreetingRepository : IGreetingRepository
{
    private readonly ConcurrentQueue<Greeting> greetings = new();

    public Task<Greeting> AddAsync(Greeting greeting, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        greetings.Enqueue(greeting);
        return Task.FromResult(greeting);
    }

    public Task<IReadOnlyList<Greeting>> ListAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        IReadOnlyList<Greeting> result = greetings
            .OrderByDescending(static greeting => greeting.CreatedAt)
            .Take(100)
            .ToArray();

        return Task.FromResult(result);
    }
}
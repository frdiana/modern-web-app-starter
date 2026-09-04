namespace ModernWebApp.Api.Endpoints;

public static class EndpointRouteBuilderExtensions
{
    public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder app)
    {
        var endpointTypes = typeof(Program).Assembly
            .GetTypes()
            .Where(static type =>
                type is { IsClass: true, IsAbstract: false }
                && type.IsAssignableTo(typeof(IEndpoint)))
            .OrderBy(static type => type.FullName, StringComparer.Ordinal);

        foreach (var endpointType in endpointTypes)
        {
            var endpoint = (IEndpoint)Activator.CreateInstance(endpointType)!;
            endpoint.Map(app);
        }

        return app;
    }
}
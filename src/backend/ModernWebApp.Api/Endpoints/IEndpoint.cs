namespace ModernWebApp.Api.Endpoints;

public interface IEndpoint
{
    void Map(IEndpointRouteBuilder app);
}
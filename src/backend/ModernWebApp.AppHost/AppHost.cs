var builder = DistributedApplication.CreateBuilder(args);

var api = builder.AddProject("api", "../ModernWebApp.Api/ModernWebApp.Api.csproj")
    .WithExternalHttpEndpoints();

builder.AddPersistence(api);

var frontend = builder.AddViteApp("frontend", "../../frontend")
    .WithNpm()
    .WithEndpoint("http", endpoint => endpoint.Port = 5173)
    .WithReference(api)
    .WithEnvironment("VITE_API_BASE_URL", api.GetEndpoint("https"))
    .WithExternalHttpEndpoints();

builder.AddFrontendAuthentication(frontend);

builder.Build().Run();
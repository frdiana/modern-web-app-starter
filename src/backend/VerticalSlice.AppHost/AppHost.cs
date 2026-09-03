var builder = DistributedApplication.CreateBuilder(args);

var api = builder.AddProject("api", "../VerticalSlice.Api/VerticalSlice.Api.csproj")
    .WithExternalHttpEndpoints();

builder.AddPersistence(api);

var frontend = builder.AddViteApp("frontend", "../../frontend")
    .WithNpm()
    .WithReference(api)
    .WithEnvironment("VITE_API_BASE_URL", api.GetEndpoint("https"))
    .WithExternalHttpEndpoints();

builder.AddFrontendAuthentication(frontend);

builder.Build().Run();
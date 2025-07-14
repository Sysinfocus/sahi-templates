var builder = DistributedApplication.CreateBuilder(args);

var api = builder.AddProject<Projects.ProjectName_API>("ProjectName-api");

builder.AddProject<Projects.ProjectName_UI>("ProjectName-ui")
    .WithReference(api);

builder.Build().Run();

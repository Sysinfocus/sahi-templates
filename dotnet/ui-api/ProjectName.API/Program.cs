var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddServices();
var app = builder.Build();

app.MapDefaultEndpoints();
app.UseServices();
app.Run();
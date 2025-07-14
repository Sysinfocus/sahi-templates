var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var baseAddress = new Uri(builder.HostEnvironment.BaseAddress);

builder.Services.AddScoped(sp => new HttpClient {
    BaseAddress = baseAddress });

builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, ApplicationStateProvider>();

builder.Services.AddScoped<Settings>(_
    => new(builder.Configuration.GetSection("ApiUrl").Value!.Replace("localhost", baseAddress.Host)));

builder.Services.AddScoped<ApiService>();
builder.Services.AddScoped<Composite>();

builder.Services.AddSysinfocus();

await builder.Build().RunAsync();
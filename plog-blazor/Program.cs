using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using plog_blazor;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp =>
{
    var client = new HttpClient();
    client.BaseAddress = new Uri("http://localhost:5249/");
    client.DefaultRequestHeaders.Add("User-Agent", "Plog-Blazor/1.0");
    return client;
});

await builder.Build().RunAsync();

using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using mtgSpellbook.Client.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
var bindersConnectionString = builder.Configuration.GetConnectionString("BinderDB");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddMudServices();
builder.Services.AddDbContextFactory<BinderDataContext>(options => options.UseSqlite(bindersConnectionString));

await builder.Build().RunAsync();

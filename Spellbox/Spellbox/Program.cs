using Spellbox.Components;
using Spellbox.Model;
using MudBlazor.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();


builder.Services.AddDbContextFactory<OracleDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("OracleDb"));
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
});
builder.Services.AddDbContextFactory<CollectionDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("CollectionDb"));
});

builder.Services.AddScoped<OracleService>();
builder.Services.AddScoped<CollectionService>();
builder.Services.AddScoped<CrossService>();

builder.Services.AddMudServices();
builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

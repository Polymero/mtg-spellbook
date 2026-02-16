using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using Spellbox.Components;
using Spellbox.Contexts;
using Spellbox.Services;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMudServices();
builder.Services.AddHttpClient();


// Database contexts
builder.Services.AddDbContextFactory<OracleDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("OracleDb"));
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
});
builder.Services.AddDbContextFactory<CollectionDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("CollectionDb"));
});
builder.Services.AddDbContextFactory<CardMarketDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("CardMarketPricingDb"));
});

// Card and collection services
builder.Services.AddScoped<OracleService>();
builder.Services.AddScoped<CollectionService>();
builder.Services.AddScoped<CrossService>();

// Scryfall import services
builder.Services.AddScoped<IScryfallImportService, ScryfallImportService>();
builder.Services.AddHostedService<ScryfallSyncWorker>();

// User services
builder.Services.AddScoped<IUserSessionService, UserSessionService>();
builder.Services.AddScoped<IUserProfileService, UserProfileService>();
builder.Services.AddScoped<IUserPricingSettingsService, UserPricingSettingsService>();

// Pricing router
builder.Services.AddScoped<IPricingRouter, PricingRouter>();

// CardMarket pricing services
builder.Services.AddScoped<CardMarketPriceGuideService>();
builder.Services.AddHostedService<CardMarketSyncWorker>();
builder.Services.AddScoped<IPricingService, CardMarketPricingService>();


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

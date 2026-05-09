using eVote470Plus.Core.Application.IOC;
using eVote470Plus.Infrastructure.Identity.IOC;
using eVote470Plus.Infrastructure.Persistence.IOC;
using eVote470Plus.Infrastructure.Shared;

var builder = WebApplication.CreateBuilder(args);

// Layer registrations
builder.Services.AddPersistenceLayerIOC(builder.Configuration);
builder.Services.AddApplicationLayerIOC();
builder.Services.AddSharedLayerIoc(builder.Configuration);
builder.Services.AddIdentityInfrastructureLayerIoc(builder.Configuration);

// MVC + Session

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Seed
await app.Services.RunIdentitySeedAsync();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseSession();

app.UseAuthentication(); 
app.UseAuthorization();
app.MapStaticAssets();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Voting}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
using ItAcademy.Application.Accounts.Helper;
using ItAcademy.Domain.UserAggregate;
using ItAcademy.Infrastructure.Infrastructures.ServiceMiddleware;
using ItAcademy.Persistence.Connections;
using ItAcademy.Persistence.DataContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
Log.Logger = new LoggerConfiguration().WriteTo.Console()
    .CreateBootstrapLogger();
builder.Host.UseSerilog();
builder.Services.AddControllersWithViews();
builder.Services.AddServices();
#region DBConfiguration
builder.Services.Configure<ConnectionStrings>(builder.Configuration.GetSection(nameof(ConnectionStrings)));
builder.Services.AddDbContext<ItAcademyDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString(nameof(ConnectionStrings.DefaultConnectionString)));
});

#endregion

builder.Services.AddMemoryCache();
builder.Services.AddSession();
builder.Services.AddIdentity<AppUser, IdentityRole>().AddEntityFrameworkStores<ItAcademyDbContext>();
builder.Services.AddScoped<IPasswordHasher<AppUser>, PasswordHasher>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

try
{
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Critical Error Occured");
}
finally
{
    Log.CloseAndFlush();
}

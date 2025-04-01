using Microsoft.EntityFrameworkCore;
using MunicipalityWebsite.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllersWithViews();

var app = builder.Build();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}"
	);
app.MapControllerRoute(
	name: "citizen",
	pattern: "{controller=Citizen}/{action=Index}/{id?}"
);



app.UseStaticFiles();

app.Run();

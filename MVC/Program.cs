using Microsoft.EntityFrameworkCore;
using BusManagementSystem.Models;
using BusManagementSystem.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Route = BusManagementSystem.Models.Route;


var builder = WebApplication.CreateBuilder(args);
var folder = Environment.SpecialFolder.LocalApplicationData;
var path = Environment.GetFolderPath(folder);
var dbPath = Path.Join(path, "Bus.db");
var connectionString = builder.Configuration.GetConnectionString("Bus") ?? "Data Source=Bus.db";

// builder.Services.AddDbContext<BusContext>(options => options.UseSqlite($"Data Source={dbPath}"));

builder.Services.AddDbContext<BusContext>(options =>
    options.UseSqlite(connectionString));


// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDefaultIdentity<Driver>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<BusContext>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ManagerOnly", policy =>
        policy.RequireClaim("IsManager", "true"));
    
    options.AddPolicy("ActiveOnly", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim("IsActive", "true") || context.User.HasClaim("IsManager", "true")));
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/User/Login";
});

builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IBusServiceInterface, BusService>();
builder.Services.AddScoped<IStopServiceInterface, StopService>();
builder.Services.AddScoped<IDriverServiceInterface, DriverService>();
builder.Services.AddScoped<IEntryServiceInterface, EntryService>();
builder.Services.AddScoped<IRouteServiceInterface, RouteService>();
builder.Services.AddScoped<ILoopServiceInterface, LoopService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapDefaultControllerRoute();

app.Run();

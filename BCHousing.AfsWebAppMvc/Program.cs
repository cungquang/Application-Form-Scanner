using BCHousing.AfsWebAppMvc.Servives.BlobStorageService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BCHousing.AfsWebAppMvc.Repositories;
using BCHousing.AfsWebAppMvc.Servives.AfsDbContextService;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using BCHousing.AfsWebAppMvc.Servives.SessionManagementService;
using BCHousing.AfsWebAppMvc.Servives.AfsDatabaseService;
using BCHousing.AfsWebAppMvc.Servives.CacheManagementService;

public class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Get connection string 
        var BlobConnectionString = builder.Configuration.GetSection("ConnectionStrings:AzureBlobConnect").Value;
        var DBConnectionString = builder.Configuration.GetSection("ConnectionStrings:AzureDBConnect").Value;

        // Add services to the container.
        builder.Services.AddControllersWithViews();
        builder.Services.AddScoped<IBlobStorageService>(provider => new BlobStorageService(BlobConnectionString));
        builder.Services.AddDbContext<AfsDbContextService>(options => options.UseSqlServer(DBConnectionString));
        builder.Services.AddHttpContextAccessor();

        builder.Services.AddScoped<ISubmissionLogRepository, SubmissionLogRepository>();
        builder.Services.AddScoped<IFormRepository, FormRepository>();
        builder.Services.AddScoped<IAfsDatabaseService, AfsDatabaseService>();
        builder.Services.AddScoped<CacheManagementService>();
        builder.Services.AddScoped<SessionManagementService>();

        // Add Memory Cache and Session
        builder.Services.AddDistributedMemoryCache();
        builder.Services.AddSession(options =>
        {
            options.Cookie.Name = ".ApplicationFormScanner";
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
            options.IdleTimeout = TimeSpan.FromMinutes(15);
        });

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
        app.UseSession();

        app.UseEndpoints(endpoints =>
        {
            // Map MVC controller route
            app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

            // Map API controller route
            endpoints.MapControllers();
        });

        app.Run();
    }
}
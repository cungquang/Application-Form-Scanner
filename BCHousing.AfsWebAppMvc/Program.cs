using BCHousing.AfsWebAppMvc.Servives.BlobStorageService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BCHousing.AfsWebAppMvc.Repositories;
using BCHousing.AfsWebAppMvc.Servives.AfsDbContextService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

public class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        string BlobConnectionString = builder.Configuration.GetSection("ConnectionString:AzureBlobConnect").Value;
        
        // Add services to the container.
        builder.Services.AddControllersWithViews();
        builder.Services.AddTransient<IBlobStorageService>(provider => new BlobStorageService(BlobConnectionString));


        var DBConnectionString = builder.Configuration.GetSection("ConnectionStrings:AzureDBConnect").Value;
        builder.Services.AddDbContext<AfsDbContextService>(options => options.UseSqlServer(DBConnectionString));
        builder.Services.AddScoped<ISubmissionLogRepository, SubmissionLogRepository>();

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
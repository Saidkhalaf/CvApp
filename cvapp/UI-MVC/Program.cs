using BL;
using BL.Export;
using BL.PersonalInfo;
using BL.Training;
using cvapp.DAL;
using cvapp.DAL.EF;
using cvapp.DAL.Export;
using cvapp.DAL.PersonalInfo;
using cvapp.DAL.Training;
using CVAPP.UI.Web.MVC.Services.Export;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Infrastructure;

namespace CVAPP.UI.Web.MVC;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var dbPath = Path.Combine(builder.Environment.ContentRootPath, "CvAppDb.db");
        builder.Services.AddDbContext<CvAppDbContext>(options =>
            options.UseSqlite($"Data Source={dbPath}"));
        builder.Services.AddScoped<IPersonalInfoRepository, PersonalInfoRepository>();
        builder.Services.AddScoped<IPersonalInfoManager, PersonalInfoManager>();
        builder.Services.AddScoped<ILanguageRepository, LanguageRepository>();
        builder.Services.AddScoped<ILanguageManager, LanguageManager>();
        builder.Services.AddScoped<ICvEducationRepository, CvEducationRepository>();
        builder.Services.AddScoped<ICvEducationManager, CvEducationManager>();
        builder.Services.AddScoped<IEducationRepository, EducationRepository>();
        builder.Services.AddScoped<IEducationManager, EducationManager>();
        builder.Services.AddScoped<ICvCertificateRepository, CvCertificateRepository>();
        builder.Services.AddScoped<ICvCertificateManager, CvCertificateManager>();
        builder.Services.AddScoped<ITrainingRepository, TrainingRepository>();
        builder.Services.AddScoped<ITrainingManager, TrainingManager>();


        QuestPDF.Settings.License = LicenseType.Community;
        builder.Services.AddScoped<ICvExportRepository, CvExportRepository>();
        builder.Services.AddScoped<ICvExportManager, CvExportManager>();
        builder.Services.AddScoped<IPdfExportService, PdfCvExportService>(); 
        builder.Services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(20);
            options.Cookie.HttpOnly = true;
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });
        
        builder.Services.AddControllersWithViews(); // Add services to the container.

        
        var app = builder.Build();
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<CvAppDbContext>();
            // Recreate + reseed on every application run.
            CvAppInitializer.Initialize(dbContext, dropCreateDatabase: true);
        }

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        
        // Enable session middleware (REQUIRED for TempData to work)
        app.UseSession();
        
        app.UseRouting();

        app.UseAuthorization();

        // Map API controllers first (takes priority over MVC)
        app.MapControllers();

        // Map MVC controllers with default route
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}

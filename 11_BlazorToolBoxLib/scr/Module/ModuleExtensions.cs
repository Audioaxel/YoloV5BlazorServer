using System.IO;
using BlazorToolBoxLib.Services.HttpRequests;
using BlazorToolBoxLib.Services.Notifications.ImageUpload;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace BlazorToolBoxLib.Module;

public static class ModuleExtensions
{
    public static void RegisterBlazorToolBoxLibServices(this IServiceCollection services)
    {
        services.AddSingleton<IYoloV5ApiImageRequest, YoloV5ApiImageRequest>();
        services.AddScoped<IImageUploadNotify, ImageUploadNotify>(); 
    }

    public static void ConfigureBlazorToolBoxLib(this WebApplication app)
    {
        app.ConfigureImageUpload();
    }


    private static void ConfigureImageUpload(this WebApplication app)
    {
        string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Files");

        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);
        
        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(folderPath),
            RequestPath = "/Files"
        });
    }
}
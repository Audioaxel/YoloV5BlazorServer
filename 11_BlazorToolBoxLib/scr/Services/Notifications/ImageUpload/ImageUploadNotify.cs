using System;

namespace BlazorToolBoxLib.Services.Notifications.ImageUpload;

public class ImageUploadNotify : IImageUploadNotify
{
    public string ImageUrl { get; set; }
    public string FilePath { get; set; }

    public event Action? OnImageUpload;

    public void ImageUpload(string imageUrl, string filePath)
    {
        ImageUrl = imageUrl;
        FilePath = filePath;
        OnImageUpload?.Invoke();
    }
}
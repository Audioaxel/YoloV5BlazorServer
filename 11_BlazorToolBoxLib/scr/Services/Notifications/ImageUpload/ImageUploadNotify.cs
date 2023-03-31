using System;

namespace BlazorToolBoxLib.Services.Notifications.ImageUpload;

public class ImageUploadNotify : IImageUploadNotify
{
    public string ImageRelativePath { get; set; }
    public string FilePath { get; set; }
    public string FileName { get; set; }

    public event Action? OnImageUpload;

    public void ImageUpload(string imageRelativePath, string filePath, string fileName)
    {
        FileName = fileName;
        ImageRelativePath = imageRelativePath;
        FilePath = filePath;
        OnImageUpload?.Invoke();
    }
}
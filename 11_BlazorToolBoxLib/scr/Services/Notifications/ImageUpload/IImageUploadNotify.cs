using System;

namespace BlazorToolBoxLib.Services.Notifications.ImageUpload;

public interface IImageUploadNotify
{
    event Action? OnImageUpload;

    string ImageRelativePath { get; set; }
    string FilePath { get; set; }
    string FileName { get; set; }

    void ImageUpload(string ImageRelativePath, string filePath, string fileName);
}

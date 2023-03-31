using System;

namespace BlazorToolBoxLib.Services.Notifications.ImageUpload;

public interface IImageUploadNotify
{
    event Action? OnImageUpload;
    string ImageUrl { get; set; }

    void ImageUpload(string imageUrl);
}

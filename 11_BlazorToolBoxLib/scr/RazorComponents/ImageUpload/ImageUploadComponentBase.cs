using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using BlazorToolBoxLib.Services.Notifications.ImageUpload;

namespace BlazorToolBoxLib.RazorComponents.ImageUpload;

public class ImageUploadComponentBase : ComponentBase
{
    const int MAX_FILE_SIZE = 512 * 1024 * 1024;

    internal string ImageUrl = "";
    internal bool Uploading = false;
    internal List<string> FileUrls = new List<string>();

    // EventHandler
    [Inject]
    public IImageUploadNotify notify { get; set;}

    // support for drag/drop
    internal string dropClass = string.Empty;
    internal void HandleDragEnter()
    {
        dropClass = "dropAreaDrug";
    }
    internal void HandleDragLeave()
    {
        dropClass = string.Empty;
    }

    internal async Task OnInputFileChange(InputFileChangeEventArgs args)
    {
        dropClass = string.Empty;

        try
        {
            // disable the upload pane
            Uploading = true;
            await InvokeAsync(StateHasChanged);

            // Resize to no more than 400x400
            var format = "image/png";
            var resizedImageFile = await args.File.RequestImageFileAsync(format, 400, 400);

            using (var stream = resizedImageFile.OpenReadStream(MAX_FILE_SIZE))
            using (var memoryStream = new MemoryStream())
            {
                await stream.CopyToAsync(memoryStream);
                var buffer = memoryStream.ToArray();

                // get new filename with a bit of entropy
                string justFileName = Path.GetFileNameWithoutExtension(args.File.Name);
                string newFileNameWithoutPath = $"{justFileName}-{DateTime.Now.Ticks.ToString()}.png";
                string filename = $"{Environment.CurrentDirectory}\\files\\{newFileNameWithoutPath}";

                // write the file
                File.WriteAllBytes(filename, buffer);

                ImageUrl = $"files/{newFileNameWithoutPath}";

                await ListFiles();

                // EventHandler
                notify.ImageUpload("test");

                Uploading = false;
            }
        }
        catch (Exception ex)
        {
            Uploading = false;
            await InvokeAsync(StateHasChanged);
            System.Diagnostics.Debug.WriteLine(ex.Message);
            throw;
        }
    }

    internal async Task ListFiles()
    {
        FileUrls.Clear();
        var files = Directory.GetFiles(Environment.CurrentDirectory + "\\Files", "*.*");
        foreach (var filename in files)
        {
            var file = Path.GetFileName(filename);
            string url = $"files/{file}";
            FileUrls.Add(url);
        }
        await InvokeAsync(StateHasChanged);
    }

    protected override async Task OnInitializedAsync()
    {
        await ListFiles();
    }
}


public class ImageUploadEventArgs : EventArgs
{
    public string ImageFilePath { get; init; }

    public ImageUploadEventArgs(string imageFilePath)
    {
        ImageFilePath= imageFilePath;
    }
}
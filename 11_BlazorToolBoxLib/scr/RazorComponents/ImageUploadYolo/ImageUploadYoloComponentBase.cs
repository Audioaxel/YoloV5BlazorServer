using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using BlazorToolBoxLib.Services.HttpRequests;
using BlazorToolBoxLib.Services.YoloHelper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace BlazorToolBoxLib.RazorComponents.ImageUploadYolo;

public class ImageUploadYoloComponentBase : ComponentBase
{
    const int MAX_FILE_SIZE = 512 * 1024 * 1024;
    internal string ImageUrl = "";
    internal string imagePath = "";
    internal bool Uploading = false;
    internal List<string> FileUrls = new List<string>();

    BoundingBoxes boundingBoxes = new BoundingBoxes();

    [Inject]
    public IYoloV5ApiImageRequest YoloRequest { get; set; }
    internal string? YoloResponse { get; set; }

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

        string test = "";

        try
        {
            // disable the upload pane
            Uploading = true;
            await InvokeAsync(StateHasChanged);

            // Resize to no more than 400x400
            var format = "image/png/jpeg";
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

                string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Files");
                imagePath = folderPath + $"/{newFileNameWithoutPath}";
                

                ImageUrl = $"files/{newFileNameWithoutPath}";
               
                // Hier Shit
                YoloResponse = await YoloRequest.SendRequest(ImageUrl);             

                await ListFiles();


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

    // YoloStuff
    internal void AnalyseImage()
    {

    }
}
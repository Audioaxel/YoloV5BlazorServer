using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace BlazorToolBoxLib.RazorComponents.DragAndDropUpload;

public class DragAndDropUploadBase : ComponentBase
{
    internal const int MAX_FILE_SIZE = 512 * 1024 *1024;
    internal string _dropClass = string.Empty;
    internal bool _uploading = false;

    [Parameter]
    public EFileType FileType { get; set; }
    [Parameter]
    public EventCallback<string> OnUpload { get; set; }

    internal string? FilePath { get; set; }


    internal async Task OnInputFileChange(InputFileChangeEventArgs args)
    {
        _dropClass = string.Empty;
        _uploading = true;

        await InvokeAsync(StateHasChanged);

        try
        {
            switch (FileType)
            {
                case EFileType.Image:
                    throw new NotImplementedException();

                case EFileType.ImageCompressed:
                    await ImageUploadWithCompression(args);
                    break;
                case EFileType.Audio:
                    throw new NotImplementedException();

                default:
                    await ImageUploadWithCompression(args);
                    break;
            }

            _uploading = false;
        }
        catch (Exception ex)
        {
            _uploading = false;
            await InvokeAsync(StateHasChanged);
            System.Diagnostics.Debug.WriteLine(ex.Message);
            throw;
        }

        await OnUpload.InvokeAsync(FilePath);
    }

    internal void HandleDragEnter()
    {
        _dropClass = "dropAreaDrug";
    }
    internal void HandleDragLeave()
    {
        _dropClass = string.Empty;
    }




    private async Task ImageUploadWithCompression(InputFileChangeEventArgs args)
    {
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
            // JA WIRKLICH????
            string absoluteFilePath = $"{Environment.CurrentDirectory}\\files\\{newFileNameWithoutPath}";

            // write the file
            File.WriteAllBytes(absoluteFilePath, buffer);

            FilePath = $"files/{newFileNameWithoutPath}";
        }
    }
}
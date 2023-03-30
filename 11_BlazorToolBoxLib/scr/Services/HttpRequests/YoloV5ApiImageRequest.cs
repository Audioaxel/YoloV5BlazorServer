using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace BlazorToolBoxLib.Services.HttpRequests;

public class YoloV5ApiImageRequest : IYoloV5ApiImageRequest
{
    private const string DETECTION_URL = "http://localhost:5000/v1/object-detection/yolov5s";

    public async Task<string> SendRequest(string imagePath)
    {
        using (var client = new HttpClient())
        {
            var content = new MultipartFormDataContent();
            var imageStream = new FileStream(imagePath, FileMode.Open);

            var imageContent = new StreamContent(imageStream);
            content.Add(imageContent, "image", "image.jpg");

            var response = await client.PostAsync(DETECTION_URL, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            return responseContent;
        }
    }
}
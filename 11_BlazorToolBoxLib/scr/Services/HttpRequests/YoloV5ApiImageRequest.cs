using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using OpenCvSharp;

namespace BlazorToolBoxLib.Services.HttpRequests;

public class YoloV5ApiImageRequest : IYoloV5ApiImageRequest
{
    private const string DETECTION_URL = "http://localhost:5000/v1/object-detection/yolov5s";
    private string ImageUrl { get; set; }

    public async Task<string> SendRequest(string imageUrl)
    {
        //Test
        ImageUrl = imageUrl;

        using (var client = new HttpClient())
        {
            var content = new MultipartFormDataContent();
            var imageStream = new FileStream(imageUrl, FileMode.Open);

            var imageContent = new StreamContent(imageStream);
            content.Add(imageContent, "image", "image.jpg");

            var response = await client.PostAsync(DETECTION_URL, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            return responseContent;
        }
    }

    public async Task TestImageDraw(string filePath)
    {
        var imagePath = filePath;
        var objectData = new List<Dictionary<string, object>>
        {
            new Dictionary<string, object>
            {
                {"xmin", 59.0163726807},
                {"ymin", 12.5729751587},
                {"xmax", 355.2341308594},
                {"ymax", 398.7200927734},
                {"confidence", 0.3703993857},
                {"class", 0},
                {"name", "person"}
            }
        };
        
        bool test = File.Exists(imagePath);
        var image = new Mat(imagePath, ImreadModes.Color);

        foreach (var obj in objectData)
        {
            var xmin = (int)obj["xmin"];
            var ymin = (int)obj["ymin"];
            var xmax = (int)obj["xmax"];
            var ymax = (int)obj["ymax"];

            Cv2.Rectangle(image, new Point(xmin, ymin), new Point(xmax, ymax), Scalar.Red, 2);
            Cv2.PutText(image, (string)obj["name"], new Point(xmin, ymin - 10), HersheyFonts.HersheyComplex, 0.5, Scalar.Red);
        }

        Cv2.ImShow("Object Detection", image);
        Cv2.WaitKey(0);
        var x = filePath;
        Cv2.ImWrite(filePath, image);
    }
}
using System.Threading.Tasks;

namespace BlazorToolBoxLib.Services.HttpRequests;

public interface IYoloV5ApiImageRequest
{
    public Task<string> SendRequest(string imagePath);
}

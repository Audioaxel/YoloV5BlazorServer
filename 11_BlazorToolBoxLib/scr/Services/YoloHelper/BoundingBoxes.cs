using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using BlazorToolBoxLib.Models;
using Newtonsoft.Json;

namespace BlazorToolBoxLib.Services.YoloHelper;

public class BoundingBoxes
{
    // SkiaSharp-Bitmap zum Speichern des Bildes
    private SKBitmap _bitmap;

    // URL des Bildes, die im <img>-Tag angezeigt wird
    private string? _imageDataUrl;
    
    private SKColor _skColor = new SKColor(196, 0, 203);
    private string? _saveImagePath;


    public string? ImageDataUrl => _imageDataUrl;



    public void DrawBoundingBoxes(string fileName, string response)
    {
        // string imagePath, Dictionary coordinates
        // Hardcoded Mist
        string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Files");
        string imagePath = folderPath + $"/{fileName}";

        // Zerpflück den Namen und mach neu wie in fileupload
        string justFileName = Path.GetFileNameWithoutExtension(fileName);
        string newFileNameWithoutPath = $"{justFileName}_annotaded.png";
        string savedImagePath = folderPath + $"/{newFileNameWithoutPath}";

        _saveImagePath = savedImagePath;
            
        // Laden des Bildes
        _bitmap = SKBitmap.Decode(imagePath);

        // Erkannte Objekte
        var objects = JsonDemodulator(response);
    
        // Schleife über alle erkannten Objekte
        foreach (var obj in objects)
        {
            // Extrahieren der Koordinaten des erkannten Objekts
            var xmin = Convert.ToInt32(obj["xmin"]);
            var ymin = Convert.ToInt32(obj["ymin"]);
            var xmax = Convert.ToInt32(obj["xmax"]);
            var ymax = Convert.ToInt32(obj["ymax"]);
            var name = obj["name"].ToString();

            // Umwandeln der Koordinaten in SkiaSharp-Koordinaten
            var rect = new SKRect(xmin, ymin, xmax, ymax);

            // Festlegen der Farbe und Stärke des Rechtecks
            var paint = new SKPaint
            {
                Color = _skColor,
                StrokeWidth = 5,
                IsAntialias = true,
                Style = SKPaintStyle.Stroke
            };

            // Festlegen der Farbe, Schriftart und Schriftgröße für den Namen des Objekts
            var textPaint = new SKPaint
            {
                Color = _skColor,
                Typeface = SKTypeface.FromFamilyName("Cascadia Code"),
                TextSize = 24
            };

            // Zeichnen des Rechtecks auf dem Bild
            using (var canvas = new SKCanvas(_bitmap))
            {
                canvas.DrawRect(rect, paint);
                canvas.DrawText(name, xmin + 14, ymin + 28, textPaint);
            }
        }

        // Konvertieren des Bitmaps in eine Base64-kodierte Zeichenfolge, die als URL für das <img>-Tag verwendet werden kann
        using (var ms = new MemoryStream())
        {
            _bitmap.Encode(SKEncodedImageFormat.Jpeg, 100).SaveTo(ms);
            _imageDataUrl = $"data:image/jpeg;base64,{Convert.ToBase64String(ms.ToArray())}";
        }
    }

    public void SaveImage()
    {
        if (!string.IsNullOrEmpty(_saveImagePath))
        {
            // Save Image
            using (var imageStream = File.OpenWrite(_saveImagePath))
            {
                _bitmap.Encode(SKEncodedImageFormat.Jpeg, 100).SaveTo(imageStream);
            }
        }
    }

    private List<Dictionary<string, object>> JsonDemodulator(string response)
    {
        var serializedClasses = JsonConvert.DeserializeObject<List<YoloCoordinates>>(response);

        var listOfCoordinates = new List<Dictionary<string, object>>();      

        foreach (var entry in serializedClasses)
        {
            var dict = new Dictionary<string, object>() 
            { 
                { "xmin", entry.xmin },
                { "ymin", entry.ymin },
                { "xmax", entry.xmax },
                { "ymax", entry.ymax },
                { "name", entry.name }
            };

            listOfCoordinates.Add(dict);
        }

        return listOfCoordinates;
    }
}
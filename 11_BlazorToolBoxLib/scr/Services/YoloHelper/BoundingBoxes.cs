using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;

namespace BlazorToolBoxLib.Services.YoloHelper;

public class BoundingBoxes
{
    // SkiaSharp-Bitmap zum Speichern des Bildes
    private SKBitmap _bitmap;

    // URL des Bildes, die im <img>-Tag angezeigt wird
    private string? _imageDataUrl;
    
    private SKColor _skColor = new SKColor(196, 0, 203);


    public string? ImageDataUrl => _imageDataUrl;

    public void DrawBoundingBoxes()
    {
        // Hardcoded Mist
        string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Files");
        string imagePath = folderPath + "/homer.png";
        string savedImagePath = folderPath + "/annotated_homer.jpg";
            
        // Laden des Bildes
        _bitmap = SKBitmap.Decode(imagePath);

        // Erkannte Objekte
        var erkannteObjekte = new List<Dictionary<string, object>>
        {
            new Dictionary<string, object>
            {
                {"xmin", 59},
                {"ymin", 12},
                {"xmax", 355},
                {"ymax", 398},
                {"name", "person"}
            }
        };

        // Schleife über alle erkannten Objekte
        foreach (var obj in erkannteObjekte)
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
        // Hardcoded Mist
        string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Files");
        string imagePath = folderPath + "/homer.png";
        string savedImagePath = folderPath + "/annotated_homer.jpg";

        // Save Image
        using (var imageStream = File.OpenWrite(savedImagePath))
        {
            _bitmap.Encode(SKEncodedImageFormat.Jpeg, 100).SaveTo(imageStream);
        }

        // _imageDataUrl = null;
    }
}
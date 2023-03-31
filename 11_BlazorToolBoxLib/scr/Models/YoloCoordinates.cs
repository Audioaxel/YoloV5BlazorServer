
namespace BlazorToolBoxLib.Models;

public class YoloCoordinates
{
        public double xmin { get; set; }
        public double ymin { get; set; }
        public double xmax { get; set; }
        public double ymax { get; set; }
        public double confidence { get; set; }
        public int @class { get; set; }
        public string name { get; set; }
}
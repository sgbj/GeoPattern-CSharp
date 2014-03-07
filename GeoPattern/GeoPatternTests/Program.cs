namespace GeoPatternTests
{
    using GeoPattern;
    using System.IO;

    class Program
    {
        static void Main(string[] args)
        {
            GeoPatterns.Options(baseColor: "#2244aa");
            var image = GeoPatterns.Generate("GitHub").UriImage();
            File.WriteAllText("GeoPattern.html", "<!doctype html><html><title></title></head><body style='background: " + image + "'></body></html>");
        }
    }
}


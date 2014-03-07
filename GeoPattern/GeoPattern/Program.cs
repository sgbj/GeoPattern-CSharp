using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GeoPattern
{
    class Program
    {
        static void Main(string[] args)
        {
            var s =
@"<!doctype html>
<html>
<head>
<title></title>
</head>
<body>";
            var rng = new Random();

            foreach (var generator in Pattern.Generators.Keys)
            {
                var c = Color.FromArgb((int)((uint)rng.Next(0xFFFFFF) | 0xFF000000));
                var image = Pattern.Generator("GeoPattern", generator,
                    new Dictionary<string, object> { { "base_color", ColorTranslator.ToHtml(c) } })
                    .GeneratePattern().UriImage();
                s += "<h2>" + generator + "</h2>";
                s += "<div style='height: 200px; margin-bottom: 50px; background: " + image + ";'></div>";
            }

            s +=
@"</body>
</html>";

            File.WriteAllText("geopattern.html", s);
        }
    }
}

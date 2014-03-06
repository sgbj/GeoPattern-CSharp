using System;
using System.Collections.Generic;
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
            Pattern p = new Pattern("scott batary", new Dictionary<string, object>
            {
                //{ "base_color", "#003300" },
                { "generator", "mosaic_squares" }
            });
            File.WriteAllText("image.html", 
                @"<!doctype html>
<html>
<head>
<title>Test</title>
<style>
body {
	background: " + p.UriImage() +
@"}
</style>
</head>
<body>
</body>
</html>");
            Console.WriteLine(p.UriImage());
        }
    }
}

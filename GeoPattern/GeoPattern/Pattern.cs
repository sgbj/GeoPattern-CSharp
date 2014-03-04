using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoPattern
{
    public class Pattern
    {
        private Dictionary<string, object> DEFAULTS = new Dictionary<string, object>
        {
            { "base_color", "#933c3c" }
        };

        private Dictionary<string, Action<Pattern>> PATTERNS = new Dictionary<string, Action<Pattern>>
        {
            { "octogons", p => p.GeoHexagons() },
            { "overlapping_circles", p => p.GeoHexagons() },
            { "plus_signs", p => p.GeoHexagons() },
            { "xes", p => p.GeoHexagons() },
            { "sine_waves", p => p.GeoSineWaves() },
            { "hexagons", p => p.GeoHexagons() },
            { "overlapping_rings", p => p.GeoHexagons() },
            { "plaid", p => p.GeoHexagons() },
            { "triangles", p => p.GeoHexagons() },
            { "squares", p => p.GeoHexagons() },
            { "concentric_circles", p => p.GeoHexagons() },
            { "diamonds", p => p.GeoHexagons() },
            { "tessellation", p => p.GeoHexagons() },
            { "nested_squares", p => p.GeoHexagons() },
            { "mosaic_squares", p => p.GeoHexagons() },
            { "triangles_rotated", p => p.GeoHexagons() },
            { "chevrons", p => p.GeoChevrons() }
        };

        private readonly string FILL_COLOR_DARK = "#222";
        private readonly string FILL_COLOR_LIGHT = "#ddd";
        private readonly string STROKE_COLOR = "#000";
        private readonly double STROKE_OPACITY = 0.02;
        private readonly double OPACITY_MIN = 0.02;
        private readonly double OPACITY_MAX = 0.15;

        private Dictionary<string, object> Options;
        private string Hash;
        private Svg Svg;

        public Pattern(string str, Dictionary<string, object> options = null)
        {
            Options = DEFAULTS.Merge(options);
            Hash = str.Sha1HexDigest();
            Svg = new Svg();
            GenerateBackground();
            GeneratePattern();
        }

        public string SvgString()
        {
            return Svg.ToString();
        }

        public override string ToString()
        {
            return SvgString();
        }

        public string Base64String()
        {
            return Svg.ToString().ToBase64();
        }

        public string UriImage()
        {
            return string.Format("url(data:image/svg+xml;base64,{0});", Base64String());
        }

        public void GenerateBackground()
        {
            var hueOffset = Map(HexVal(14, 3), 0, 4095, 0, 359);
            var satOffset = HexVal(17, 1);
            var baseColor = new HSLColor(ColorTranslator.FromHtml((string)Options["base_color"]));
            baseColor.Hue -= hueOffset;
            baseColor.Saturation += satOffset % 2 == 0 ? satOffset : -satOffset;
            Color rgb = baseColor;
            Svg.Rect(0, 0, "100%", "100%", new Dictionary<string, object> 
            { 
                { "fill", string.Format("rgb({0}, {1}, {2})", rgb.R, rgb.G, rgb.B) } 
            });
        }

        public void GeneratePattern()
        {
            if (Options.ContainsKey("generator"))
            {
                if (!PATTERNS.ContainsKey((string)Options["generator"]))
                    throw new ArgumentException("Error: the requested generator is invalid.");
                PATTERNS[(string)Options["generator"]](this);
            }
            else
            {
                PATTERNS.ToList()[HexVal(20, 1)].Value(this);
            }
        }

        public void GeoHexagons()
        {
            var scale = HexVal(0, 1);
            var sideLength = Map(scale, 0, 15, 8, 60);
            var hexHeight = sideLength * Math.Sqrt(3);
            var hexWidth = sideLength * 2;
            var hex = BuildHexagonShape(sideLength);

            Svg.Width = (hexWidth * 3) + (sideLength * 3);
            Svg.Height = hexHeight * 6;

            var i = 0;

            for (var y = 0; y <= 5; y++)
            {
                for (var x = 0; x <= 5; x++)
                {
                    var val = HexVal(i, 1);
                    var dy = x % 2 == 0 ? y * hexHeight : y * hexHeight + hexHeight / 2;
                    var opacity = Opacity(val);
                    var fill = FillColor(val);

                    var styles = new Dictionary<string, object>
                    {
                        { "fill", fill },
                        { "fill-opacity", opacity },
                        { "stroke", STROKE_COLOR },
                        { "stroke-opacity", STROKE_OPACITY }
                    };

                    Svg.PolyLine(hex, styles.Merge(Transform("translate({0}, {1})", x * sideLength * 1.5 - hexWidth / 2, dy - hexHeight / 2)));

                    // Add an extra one at top-right, for tiling.
                    if (x == 0)
                    {
                        Svg.PolyLine(hex, styles.Merge(Transform("translate({0}, {1})", 6 * sideLength * 1.5 - hexWidth / 2, dy - hexHeight / 2)));
                    }

                    // Add an extra row at the end that matches the first row, for tiling.
                    if (y == 0)
                    {
                        dy = x % 2 == 0 ? 6 * hexHeight : 6 * hexHeight + hexHeight / 2;
                        Svg.PolyLine(hex, styles.Merge(Transform("translate({0}, {1})", x * sideLength * 1.5 - hexWidth / 2, dy - hexHeight / 2)));
                    }

                    // Add an extra one at bottom-right, for tiling.
                    if (x == 0 && y == 0)
                    {
                        Svg.PolyLine(hex, styles.Merge(Transform("translate({0}, {1})", 6 * sideLength * 1.5 - hexWidth / 2, 5 * hexHeight + hexHeight / 2)));
                    }

                    i++;
                }
            }
        }

        public void GeoSineWaves()
        {
            var period = Math.Floor(Map(HexVal(0, 1), 0, 15, 100, 400));
            var amplitude = Math.Floor(Map(HexVal(1, 1), 0, 15, 30, 100));
            var waveWidth = Math.Floor(Map(HexVal(2, 1), 0, 15, 3, 30));

            Svg.Width = period;
            Svg.Height = waveWidth * 36;

            for (var i = 0; i <= 35; i++)
            {
                var val = HexVal(i, 1);
                var opacity = Opacity(val);
                var fill = FillColor(val);
                var xOffset = period / 4 * 0.7;

                var styles = new Dictionary<string, object>
                {
                    { "fill", "none" },
                    { "stroke", fill },
                    { "style", 
                        new Dictionary<string, object> 
                        {
                            { "opacity", opacity },
                            { "stroke-width", waveWidth + "px" }
                        }
                    }
                };

                var str = "M0 " + amplitude
                        + " C " + xOffset + " 0, " + (period / 2 - xOffset) + " 0, " + (period / 2) + " " + amplitude
                        + " S " + (period - xOffset) + " " + (amplitude * 2) + ", " + period + " " + amplitude
                        + " S " + (period * 1.5 - xOffset) + " 0, " + (period * 1.5) + ", " + amplitude;

                Svg.Path(str, styles.Merge(Transform("translate(-{0}, {1})", period / 4, waveWidth * i - amplitude * 1.5)));
                Svg.Path(str, styles.Merge(Transform("translate(-{0}, {1})", period / 4, waveWidth * i - amplitude * 1.5 + waveWidth * 36)));
            }
        }

        public void GeoChevrons()
        {
            var chevron_width = Map(HexVal(0, 1), 0, 15, 30, 80);
            var chevron_height = Map(HexVal(0, 1), 0, 15, 30, 80);

            Svg.Width = chevron_width * 6;
            Svg.Height = chevron_height * 6 * 0.66;

            var i = 0;

            for (var y = 0; y <= 5; y++)
            {
                for (var x = 0; x <= 5; x++)
                {
                    var val = HexVal(i, 1);
                    var opacity = Opacity(val);
                    var fill = FillColor(val);

                    var styles = new Dictionary<string, object>
                    {
                        { "stroke", STROKE_COLOR },
                        { "stroke-opacity", STROKE_OPACITY },
                        { "fill", fill },
                        { "fill-opacity", opacity },
                        { "stroke-width", 1 }
                    };

                    BuildChevronShape(chevron_width, chevron_height, styles.Merge(Transform("translate({0}, {1})", x * chevron_width, y * chevron_height * 0.66 - chevron_height / 2)));

                    if (y == 0)
                    {
                        BuildChevronShape(chevron_width, chevron_height, styles.Merge(Transform("translate({0}, {1})", x * chevron_width, 6 * chevron_height * 0.66 - chevron_height / 2)));
                    }

                    i++;
                }
            }
        }

        private string BuildHexagonShape(double sideLength)
        {
            var c = sideLength;
            var a = c / 2;
            var b = Math.Sin(60 * Math.PI / 180) * c;
            return string.Format("0,{0},{1},0,{2},0,{3},{4},{5},{6},{7},{8},0,{9}",
                b, a, a + c, 2 * c, b, a + c, 2 * b, a, 2 * b, b);
        }

        private void BuildChevronShape(double width, double height, Dictionary<string, object> args = null)
        {
            var e = height * 0.66;
            Svg.BeginGroup(args)
               .PolyLine(string.Format("0,0,{0},{1},{2},{3},0,{4},0,0",
                    width / 2, height - e, width / 2, height, e))
               .PolyLine(string.Format("{0},{1},{2},0,{3},{4},{5},{6},{7},{8}",
                    width / 2, height - e, width, width, e, width / 2, height, width / 2, height - e))
               .EndGroup();
        }

        //-----------------------------------------------------

        public int HexVal(int index, int len)
        {
            return int.Parse(Hash.Substring(index, len), NumberStyles.HexNumber);
        }

        public string FillColor(int val)
        {
            return val % 2 == 0 ? FILL_COLOR_LIGHT : FILL_COLOR_DARK;
        }

        public double Opacity(double val)
        {
            return Map(val, 0, 15, OPACITY_MIN, OPACITY_MAX);
        }

        public double Map(double value, double vMin, double vMax, double dMin, double dMax)
        {
            return (value - vMin) * (dMax - dMin) / (vMax - vMin) + dMin;
        }

        private Dictionary<string, object> Transform(string format, params object[] args)
        {
            return new Dictionary<string, object>
            {
                { "transform", string.Format(format, args) }
            };
        }
    }
}

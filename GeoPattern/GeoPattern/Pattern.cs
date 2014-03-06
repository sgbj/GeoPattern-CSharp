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
            { "octagons", p => p.GeoOctagons() },
            { "overlapping_circles", p => p.GeoOverlappingCircles() },
            { "plus_signs", p => p.GeoPlusSigns() },
            { "xes", p => p.GeoXes() },
            { "sine_waves", p => p.GeoSineWaves() },
            { "hexagons", p => p.GeoHexagons() },
            { "overlapping_rings", p => p.GeoOverlappingRings() },
            { "plaid", p => p.GeoPlaid() },
            { "triangles", p => p.GeoTriangles() },
            { "squares", p => p.GeoSquares() },
            { "concentric_circles", p => p.GeoConcentricCircles() },
            { "diamonds", p => p.GeoDiamonds() },
            { "tessellation", p => p.GeoTessellation() },
            { "nested_squares", p => p.GeoNestedSquares() },
            { "mosaic_squares", p => p.GeoMosaicSquares() },
            { "triangles_rotated", p => p.GeoTrianglesRotated() },
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

        public void GeoPlusSigns()
        {
            var square_size = Map(HexVal(0, 1), 0, 15, 10, 25);
            var plus_size = square_size * 3;

            Svg.Width = square_size * 12;
            Svg.Height = square_size * 12;

            var i = 0;

            for (var y = 0; y <= 5; y++)
            {
                for (var x = 0; x <= 5; x++)
                {
                    var val = HexVal(i, 1);
                    var opacity = Opacity(val);
                    var fill = FillColor(val);
                    var dx = (y % 2 == 0) ? 0 : 1;

                    var styles = new Dictionary<string, object> 
                    {
                        {"fill", fill},
                        {"stroke", STROKE_COLOR},
                        {"stroke-opacity", STROKE_OPACITY},
                        {"style", new Dictionary<string, object> 
                            {
                                {"fill-opacity", opacity}
                            }
                        }
                    };

                    BuildPlusShape(square_size, styles.Merge(Transform("translate({0},{1})", x * plus_size - x * square_size + dx * square_size - square_size, y * plus_size - y * square_size - plus_size / 2)));

                    // Add an extra column on the right for tiling.
                    if (x == 0)
                    {
                        BuildPlusShape(square_size, styles.Merge(Transform("translate({0},{1})", 4 * plus_size - x * square_size + dx * square_size - square_size, y * plus_size - y * square_size - plus_size / 2)));
                    }

                    // Add an extra row on the bottom that matches the first row, for tiling.
                    if (y == 0)
                    {
                        BuildPlusShape(square_size, styles.Merge(Transform("translate({0},{1})", x * plus_size - x * square_size + dx * square_size - square_size, 4 * plus_size - y * square_size - plus_size / 2)));
                    }

                    // Add an extra one at top-right and bottom-right, for tiling.
                    if (x == 0 && y == 0)
                    {
                        BuildPlusShape(square_size, styles.Merge(Transform("translate({0},{1})", 4 * plus_size - x * square_size + dx * square_size - square_size, 4 * plus_size - y * square_size - plus_size / 2)));
                    }

                    i++;
                }
            }
        }




        public void GeoXes()
        {
            var square_size = Map(HexVal(0, 1), 0, 15, 10, 25);
            var x_size = square_size * 3 * 0.943;

            Svg.Width = x_size * 3;
            Svg.Height = x_size * 3;

            var i = 0;

            for (var y = 0; y <= 5; y++)
            {
                for (var x = 0; x <= 5; x++)
                {
                    var val = HexVal(i, 1);
                    var opacity = Opacity(val);
                    var dy = x % 2 == 0 ? y * x_size - x_size * 0.5 : y * x_size - x_size * 0.5 + x_size / 4;
                    var fill = FillColor(val);

                    var styles = new Dictionary<string, object> 
                    {
                        {"fill", fill},
                        {"style", new Dictionary<string, object> 
                            {
                                {"opacity", opacity}
                            }
                        }
                    };

                    BuildPlusShape(square_size, styles.Merge(Transform("translate({0},{1}) rotate(45, {2}, {2})", x * x_size / 2 - x_size / 2, dy - y * x_size / 2, x_size / 2)));

                    // Add an extra column on the right for tiling.
                    if (x == 0)
                    {
                        BuildPlusShape(square_size, styles.Merge(Transform("translate({0},{1}) rotate(45, {2}, {2})", 6 * x_size / 2 - x_size / 2, dy - y * x_size / 2, x_size / 2)));
                    }

                    // Add an extra row on the bottom that matches the first row, for tiling.
                    if (y == 0)
                    {
                        dy = x % 2 == 0 ? 6 * x_size - x_size / 2 : 6 * x_size - x_size / 2 + x_size / 4;
                        BuildPlusShape(square_size, styles.Merge(Transform("translate({0},{1}) rotate(45, {2}, {2})", x * x_size / 2 - x_size / 2, dy - 6 * x_size / 2, x_size / 2)));
                    }

                    // These can hang off the bottom, so put a row at the top for tiling.
                    if (y == 5)
                    {
                        BuildPlusShape(square_size, styles.Merge(Transform("translate({0},{1}) rotate(45, {2}, {2})", x * x_size / 2 - x_size / 2, dy - 11 * x_size / 2, x_size / 2)));
                    }
                    // Add an extra one at top-right and bottom-right, for tiling.
                    if (x == 0 && y == 0)
                    {
                        BuildPlusShape(square_size, styles.Merge(Transform("translate({0},{1}) rotate(45, {2}, {2})", 6 * x_size / 2 - x_size / 2, dy - 6 * x_size / 2, x_size / 2)));
                    }

                    i++;
                }
            }
        }

        public void GeoOverlappingCircles()
        {
            var scale = HexVal(0, 1);
            var diameter = Map(scale, 0, 15, 25, 200);
            var radius = diameter / 2;

            Svg.Width = radius * 6;
            Svg.Height = radius * 6;

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
                        {"fill", fill},
                        {"style", new Dictionary<string, object> 
                            {
                                {"opacity", opacity}
                            }
                        }
                    };

                    Svg.Circle(x * radius, y * radius, radius, styles);

                    // Add an extra one at top-right, for tiling.
                    if (x == 0)
                    {
                        Svg.Circle(6 * radius, y * radius, radius, styles);
                    }

                    // Add an extra row at the end that matches the first row, for tiling.
                    if (y == 0)
                    {
                        Svg.Circle(x * radius, 6 * radius, radius, styles);
                    }

                    // Add an extra one at bottom-right, for tiling.
                    if (x == 0 && y == 0)
                    {
                        Svg.Circle(6 * radius, 6 * radius, radius, styles);
                    }

                    i++;
                }
            }
        }

        public void GeoOctagons()
        {
            var square_size = Map(HexVal(0, 1), 0, 15, 10, 60);
            var tile = BuildOctagonShape(square_size);

            Svg.Width = square_size * 6;
            Svg.Height = square_size * 6;

            var i = 0;

            for (var y = 0; y <= 5; y++)
            {
                for (var x = 0; x <= 5; x++)
                {
                    var val = HexVal(i, 1);
                    var opacity = Opacity(val);
                    var fill = FillColor(val);

                    Svg.PolyLine(tile, new Dictionary<string, object> 
                    {
                        {"fill", fill},
                        {"fill-opacity", opacity},
                        {"stroke", STROKE_COLOR},
                        {"stroke-opacity", STROKE_OPACITY},
                        {"transform", string.Format("translate({0}, {1})", x*square_size,y*square_size)}
                    });

                    i++;
                }
            }
        }

        public void GeoSquares()
        {
            var square_size = Map(HexVal(0, 1), 0, 15, 10, 60);

            Svg.Width = square_size * 6;
            Svg.Height = square_size * 6;

            var i = 0;

            for (var y = 0; y <= 5; y++)
            {
                for (var x = 0; x <= 5; x++)
                {
                    var val = HexVal(i, 1);
                    var opacity = Opacity(val);
                    var fill = FillColor(val);

                    Svg.Rect(x * square_size, y * square_size, square_size, square_size, new Dictionary<string, object> {
                        {"fill", fill},
                        {"fill-opacity", opacity},
                        {"stroke", STROKE_COLOR},
                        {"stroke-opacity", STROKE_OPACITY}
                    });

                    i++;
                }
            }
        }

        public void GeoConcentricCircles()
        {
            var scale = HexVal(0, 1);
            var ring_size = Map(scale, 0, 15, 10, 60);
            var stroke_width = ring_size / 5;

            Svg.Width = (ring_size + stroke_width) * 6;
            Svg.Height = (ring_size + stroke_width) * 6;

            var i = 0;

            for (var y = 0; y <= 5; y++)
            {
                for (var x = 0; x <= 5; x++)
                {
                    var val = HexVal(i, 1);
                    var opacity = Opacity(val);
                    var fill = FillColor(val);

                    Svg.Circle(x * ring_size + x * stroke_width + (ring_size + stroke_width) / 2,
                        y * ring_size + y * stroke_width + (ring_size + stroke_width) / 2,
                        ring_size / 2,
                        new Dictionary<string, object> {
                            {"fill", "none"},
                            {"stroke", fill},
                            {
                                "style", new Dictionary<string, object> {
                                    {"opacity", opacity},
                                    {"stroke-width", stroke_width + "px"}
                                }
                            }
                        });

                    val = HexVal(39 - i, 1);
                    opacity = Opacity(val);
                    fill = FillColor(val);

                    Svg.Circle(x * ring_size + x * stroke_width + (ring_size + stroke_width) / 2,
                              y * ring_size + y * stroke_width + (ring_size + stroke_width) / 2,
                              ring_size / 4,
                              new Dictionary<string, object> {
                            {"fill", fill},
                            {"fill-opacity", opacity}
                        });

                    i++;
                }
            }
        }

        public void GeoOverlappingRings()
        {
            var scale = HexVal(0, 1);
            var ring_size = Map(scale, 0, 15, 10, 60);
            var stroke_width = ring_size / 4;

            Svg.Width = ring_size * 6;
            Svg.Height = ring_size * 6;

            var i = 0;

            for (var y = 0; y <= 5; y++)
            {
                for (var x = 0; x <= 5; x++)
                {
                    var val = HexVal(i, 1);
                    var opacity = Opacity(val);
                    var fill = FillColor(val);

                    var styles = new Dictionary<string, object> {
                        {"fill", "none"},
                        {"stroke", fill},
                        {
                            "style", new Dictionary<string, object> {
                                {"opacity", opacity},
                                {"stroke-width", stroke_width + "px"}
                            }
                        }
                    };

                    Svg.Circle(x * ring_size, y * ring_size, ring_size - stroke_width / 2, styles);

                    // Add an extra one at top-right, for tiling.
                    if (x == 0)
                    {
                        Svg.Circle(6 * ring_size, y * ring_size, ring_size - stroke_width / 2, styles);
                    }

                    if (y == 0)
                    {
                        Svg.Circle(x * ring_size, 6 * ring_size, ring_size - stroke_width / 2, styles);
                    }

                    if (x == 0 && y == 0)
                    {
                        Svg.Circle(6 * ring_size, 6 * ring_size, ring_size - stroke_width / 2, styles);
                    }

                    i++;
                }
            }
        }

        public void GeoTriangles()
        {

            var scale = HexVal(0, 1);
            var side_length = Map(scale, 0, 15, 15, 80);
            var triangle_height = side_length / 2 * Math.Sqrt(3);
            var triangle = BuildTriangleShape(side_length, triangle_height);

            Svg.Width = side_length * 3;
            Svg.Height = triangle_height * 6;

            var i = 0;

            for (var y = 0; y <= 5; y++)
            {
                for (var x = 0; x <= 5; x++)
                {

                    var val = HexVal(i, 1);
                    var opacity = Opacity(val);
                    var fill = FillColor(val);

                    var styles = new Dictionary<string, object> {
                        {"fill"           , fill},
                        {"fill-opacity"   , opacity},
                        {"stroke"         , STROKE_COLOR},
                        {"stroke-opacity" , STROKE_OPACITY}
                    };

                    var rotation = 0;

                    if (y % 2 == 0)
                    {
                        rotation = x % 2 == 0 ? 180 : 0;
                    }
                    else
                    {
                        rotation = x % 2 != 0 ? 180 : 0;
                    }

                    Svg.PolyLine(triangle, styles.Merge(Transform("translate({0}, {1}) rotate({2}, {3}, {4})",
                        x * side_length * 0.5 - side_length / 2, triangle_height * y, rotation, side_length / 2, triangle_height / 2)));

                    // Add an extra one at top-right, for tiling.
                    if (x == 0)
                    {
                        Svg.PolyLine(triangle, styles.Merge(Transform("translate({0}, {1}) rotate({2}, {3}, {4})",
                            6 * side_length * 0.5 - side_length / 2, triangle_height * y, rotation, side_length / 2, triangle_height / 2)));
                    }

                    i++;
                }
            }
        }

        public void GeoTrianglesRotated()
        {
            var scale = HexVal(0, 1);
            var side_length = Map(scale, 0, 15, 15, 80);
            var triangle_width = side_length / 2 * Math.Sqrt(3);
            var triangle = build_rotated_triangle_shape(side_length, triangle_width);

            Svg.Width = triangle_width * 6;
            Svg.Height = side_length * 3;

            var i = 0;

            for (var y = 0; y <= 5; y++)
            {
                for (var x = 0; x <= 5; x++)
                {

                    var val = HexVal(i, 1);
                    var opacity = Opacity(val);
                    var fill = FillColor(val);

                    var styles = new Dictionary<string, object> {
                        {"fill"           , fill},
                        {"fill-opacity"   , opacity},
                        {"stroke"         , STROKE_COLOR},
                        {"stroke-opacity" , STROKE_OPACITY}
                    };

                    var rotation = 0;

                    if (y % 2 == 0)
                    {
                        rotation = x % 2 == 0 ? 180 : 0;
                    }
                    else
                    {
                        rotation = x % 2 != 0 ? 180 : 0;
                    }

                    Svg.PolyLine(triangle, styles.Merge(Transform("translate({0}, {1}) rotate({2}, {3}, {4})",
                        triangle_width * x, y * side_length * 0.5 - side_length / 2, rotation, triangle_width / 2, side_length / 2)));

                    // Add an extra one at top-right, for tiling.
                    if (y == 0)
                    {
                        Svg.PolyLine(triangle, styles.Merge(Transform("translate({0}, {1}) rotate({2}, {3}, {4})",
                        triangle_width * x, 6 * side_length * 0.5 - side_length / 2, rotation, triangle_width / 2, side_length / 2)));
                    }

                    i++;
                }
            }
        }

        public void GeoDiamonds()
        {
            var diamond_width = Map(HexVal(0, 1), 0, 15, 10, 50);
            var diamond_height = Map(HexVal(1, 1), 0, 15, 10, 50);
            var diamond = build_diamond_shape(diamond_width, diamond_height);

            Svg.Width = diamond_width * 6;
            Svg.Height = diamond_height * 3;

            var i = 0;

            for (var y = 0; y <= 5; y++)
            {
                for (var x = 0; x <= 5; x++)
                {
                    var val = HexVal(i, 1);
                    var opacity = Opacity(val);
                    var fill = FillColor(val);

                    var styles = new Dictionary<string, object> {
                        {"fill"           , fill},
                        {"fill-opacity"   , opacity},
                        {"stroke"         , STROKE_COLOR},
                        {"stroke-opacity" , STROKE_OPACITY}
                    };

                    var dx = (y % 2 == 0) ? 0 : diamond_width / 2;

                    Svg.PolyLine(diamond, styles.Merge(Transform("translate({0}, {1})", x * diamond_width - diamond_width / 2 + dx, diamond_height / 2 * y - diamond_height / 2)));

                    // Add an extra one at top-right, for tiling.
                    if (x == 0)
                    {
                        Svg.PolyLine(diamond, styles.Merge(Transform("translate({0}, {1})", 6 * diamond_width - diamond_width / 2 + dx, diamond_height / 2 * y - diamond_height / 2)));
                    }

                    // Add an extra row at the end that matches the first row, for tiling.
                    if (y == 0)
                    {
                        Svg.PolyLine(diamond, styles.Merge(Transform("translate({0}, {1})", x * diamond_width - diamond_width / 2 + dx, diamond_height / 2 * 6 - diamond_height / 2)));
                    }

                    // Add an extra one at bottom-right, for tiling.
                    if (x == 0 && y == 0)
                    {
                        Svg.PolyLine(diamond, styles.Merge(Transform("translate({0}, {1})", 6 * diamond_width - diamond_width / 2 + dx, diamond_height / 2 * 6 - diamond_height / 2)));
                    }

                    i++;
                }
            }
        }

        public void GeoNestedSquares()
        {
            var block_size = Map(HexVal(0, 1), 0, 15, 4, 12);
            var square_size = block_size * 7;

            Svg.Width = (square_size + block_size) * 6 + block_size * 6;
            Svg.Height = (square_size + block_size) * 6 + block_size * 6;

            var i = 0;

            for (var y = 0; y <= 5; y++)
            {
                for (var x = 0; x <= 5; x++)
                {
                    var val = HexVal(i, 1);
                    var opacity = Opacity(val);
                    var fill = FillColor(val);

                    var styles = new Dictionary<string, object> {
                        {"fill", "none"},
                        {"stroke", fill},
                        {
                            "style", new Dictionary<string, object> {
                              {"opacity", opacity},
                              {"stroke-width", block_size + "px"}
                            }
                        }
                    };

                    Svg.Rect(x * square_size + x * block_size * 2 + block_size / 2,
                              y * square_size + y * block_size * 2 + block_size / 2,
                              square_size, square_size, styles);

                    val = HexVal(39 - i, 1);
                    opacity = Opacity(val);
                    fill = FillColor(val);

                    styles = new Dictionary<string, object> {
                        {"fill", "none"},
                        {"stroke", fill},
                        {
                            "style", new Dictionary<string, object> {
                              {"opacity", opacity},
                              {"stroke-width", block_size + "px"}
                            }
                        }
                    };

                    Svg.Rect(x * square_size + x * block_size * 2 + block_size / 2 + block_size * 2,
                              y * square_size + y * block_size * 2 + block_size / 2 + block_size * 2,
                              block_size * 3, block_size * 3, styles);

                    i++;
                }
            }
        }

        public void GeoPlaid()
        {
            var height = 0.0;
            var width = 0.0;

            // horizontal stripes
            var i = 0;
            for (var j = 0; j < 18; j++)
            {
                var space = HexVal(i, 1);
                height += space + 5;

                var val = HexVal(i + 1, 1);
                var opacity = Opacity(val);
                var fill = FillColor(val);
                var stripe_height = val + 5;

                Svg.Rect(0, height, "100%", stripe_height, new Dictionary<string, object> {
                      {"opacity", opacity},
                      {"fill", fill}
                });

                height += stripe_height;
                i += 2;
            }

            // vertical stripes
            i = 0;
            for (var j = 0; j < 18; j++)
            {
                var space = HexVal(i, 1);
                width += space + 5;

                var val = HexVal(i + 1, 1);
                var opacity = Opacity(val);
                var fill = FillColor(val);
                var stripe_width = val + 5;

                Svg.Rect(width, 0, stripe_width, "100%", new Dictionary<string, object>{
                      {"opacity", opacity},
                      {"fill", fill}
                });

                width += stripe_width;
                i += 2;
            }

            Svg.Width = width;
            Svg.Height = height;
        }

        public void GeoMosaicSquares()
        {
            var triangle_size = Map(HexVal(0, 1), 0, 15, 15, 50);

            Svg.Width = triangle_size * 8;
            Svg.Height = triangle_size * 8;

            var i = 0;

            for (var y = 0; y <= 3; y++)
            {
                for (var x = 0; x <= 3; x++)
                {

                    if (x % 2 == 0)
                    {
                        if (y % 2 == 0)
                        {
                            draw_outer_mosaic_tile(x * triangle_size * 2, y * triangle_size * 2, triangle_size, HexVal(i, 1));
                        }
                        else
                        {
                            draw_inner_mosaic_tile(x * triangle_size * 2, y * triangle_size * 2, triangle_size, new[] { HexVal(i, 1), HexVal(i + 1, 1) });
                        }
                    }
                    else
                    {
                        if (y % 2 == 0)
                        {
                            draw_inner_mosaic_tile(x * triangle_size * 2, y * triangle_size * 2, triangle_size, new[] { HexVal(i, 1), HexVal(i + 1, 1) });
                        }
                        else
                        {
                            draw_outer_mosaic_tile(x * triangle_size * 2, y * triangle_size * 2, triangle_size, HexVal(i, 1));
                        }
                    }

                    i++;
                }
            }
        }

        private void draw_outer_mosaic_tile(double x, double y, double triangle_size, double val)
        {
            var opacity = Opacity(val);
            var fill = FillColor(val);
            var triangle = build_right_triangle_shape(triangle_size);
            var styles = new Dictionary<string, object> {
                {"stroke"         , STROKE_COLOR},
                {"stroke-opacity" , STROKE_OPACITY},
                {"fill-opacity"   , opacity},
                {"fill"           , fill}
            };

            Svg.PolyLine(triangle, styles.Merge(Transform("translate({0}, {1}) scale(1, -1)", x, y + triangle_size)));
            Svg.PolyLine(triangle, styles.Merge(Transform("translate({0}, {1}) scale(-1, -1)", x + triangle_size * 2, y + triangle_size)));
            Svg.PolyLine(triangle, styles.Merge(Transform("translate({0}, {1}) scale(1, 1)", x, y + triangle_size)));
            Svg.PolyLine(triangle, styles.Merge(Transform("translate({0}, {1}) scale(-1, 1)", x + triangle_size * 2, y + triangle_size)));
        }

        private void draw_inner_mosaic_tile(double x, double y, double triangle_size, int[] vals)
        {
            var triangle = build_right_triangle_shape(triangle_size);
            var opacity = Opacity(vals[0]);
            var fill = FillColor(vals[0]);
            var styles = new Dictionary<string, object>{
                {"stroke"         , STROKE_COLOR},
                {"stroke-opacity" , STROKE_OPACITY},
                {"fill-opacity"   , opacity},
                {"fill"           , fill}
            };
            Svg.PolyLine(triangle, styles.Merge(Transform("translate({0}, {1}) scale(-1, 1)", x + triangle_size, y)));
            Svg.PolyLine(triangle, styles.Merge(Transform("translate({0}, {1}) scale(1, -1)", x + triangle_size, y + triangle_size * 2)));

            opacity = Opacity(vals[1]);
            fill = FillColor(vals[1]);
            styles = new Dictionary<string, object>{
                {"stroke"         , STROKE_COLOR},
                {"stroke-opacity" , STROKE_OPACITY},
                {"fill-opacity"   , opacity},
                {"fill"           , fill}
            };
            Svg.PolyLine(triangle, styles.Merge(Transform("translate({0}, {1}) scale(-1, -1)", x + triangle_size, y + triangle_size * 2)));
            Svg.PolyLine(triangle, styles.Merge(Transform("translate({0}, {1}) scale(1, 1)", x + triangle_size, y)));
        }

        public void GeoTessellation()
        {
            // 3.4.6.4 semi-regular tessellation
      var side_length     = Map(HexVal(0, 1), 0, 15, 5, 40);
      var hex_height      = side_length * Math.Sqrt(3);
      var hex_width       = side_length * 2;
      var triangle_height = side_length/2 * Math.Sqrt(3);
      var triangle        = build_rotated_triangle_shape(side_length, triangle_height);
      var tile_width      = side_length*3 + triangle_height*2;
      var tile_height     = (hex_height * 2) + (side_length * 2);

      Svg.Width = tile_width;
      Svg.Height = tile_height;

      for (var i = 0; i <= 19; i++)
      {
        var val     = HexVal(i, 1);
        var opacity = Opacity(val);
        var fill    = FillColor(val);

        var styles  = new Dictionary<string, object>{
                {"stroke"         , STROKE_COLOR},
                {"stroke-opacity" , STROKE_OPACITY},
                {"fill"           , fill},
                {"fill-opacity"   , opacity},
                {"stroke-width"   , 1}
        };

        switch(i)
        {
        case 0: // all 4 corners
          Svg.Rect(-side_length/2, -side_length/2, side_length, side_length, styles);
          Svg.Rect(tile_width - side_length/2, -side_length/2, side_length, side_length, styles);
          Svg.Rect(-side_length/2, tile_height-side_length/2, side_length, side_length, styles);
          Svg.Rect(tile_width - side_length/2, tile_height-side_length/2, side_length, side_length, styles);
                break;
        case 1: // center / top square
          Svg.Rect(hex_width/2 + triangle_height, hex_height/2, side_length, side_length, styles);
                break;
        case 2: // side squares
          Svg.Rect(-side_length/2, tile_height/2-side_length/2, side_length, side_length, styles);
          Svg.Rect(tile_width-side_length/2, tile_height/2-side_length/2, side_length, side_length, styles);
                break;
        case 3: // center / bottom square
          Svg.Rect(hex_width/2 + triangle_height, hex_height * 1.5 + side_length, side_length, side_length, styles);
                break;
        //case 4: // left top / bottom triangle
        //  svg.polyline(triangle, styles.merge({"transform" => "translate(#{side_length/2}, #{-side_length/2}) rotate(0, #{side_length/2}, #{triangle_height/2})"}))
        //  svg.polyline(triangle, styles.merge({"transform" => "translate(#{side_length/2}, #{tile_height--side_length/2}) rotate(0, #{side_length/2}, #{triangle_height/2}) scale(1, -1)"}))
        //    break;
        //case 5: // right top / bottom triangle
        //  svg.polyline(triangle, styles.merge({"transform" => "translate(#{tile_width-side_length/2}, #{-side_length/2}) rotate(0, #{side_length/2}, #{triangle_height/2}) scale(-1, 1)"}))
        //  svg.polyline(triangle, styles.merge({"transform" => "translate(#{tile_width-side_length/2}, #{tile_height+side_length/2}) rotate(0, #{side_length/2}, #{triangle_height/2}) scale(-1, -1)"}))
        //    break;
        //case 6: // center / top / right triangle
        //  svg.polyline(triangle, styles.merge({"transform" => "translate(#{tile_width/2+side_length/2}, #{hex_height/2})"}))
        //    break;
        //case 7: // center / top / left triangle
        //  svg.polyline(triangle, styles.merge({"transform" => "translate(#{tile_width-tile_width/2-side_length/2}, #{hex_height/2}) scale(-1, 1)"}))
        //    break;
        //case 8: // center / bottom / right triangle
        //  svg.polyline(triangle, styles.merge({"transform" => "translate(#{tile_width/2+side_length/2}, #{tile_height-hex_height/2}) scale(1, -1)"}))
        //    break;
        //case 9: // center / bottom / left triangle
        //  svg.polyline(triangle, styles.merge({"transform" => "translate(#{tile_width-tile_width/2-side_length/2}, #{tile_height-hex_height/2}) scale(-1, -1)"}))
        //    break;
        //case 10: // left / middle triangle
        //  svg.polyline(triangle, styles.merge({"transform" => "translate(#{side_length/2}, #{tile_height/2 - side_length/2})"}))
        //    break;
        //case 11: // right / middle triangle
        //  svg.polyline(triangle, styles.merge({"transform" => "translate(#{tile_width-side_length/2}, #{tile_height/2 - side_length/2}) scale(-1, 1)"}))
        //    break;
        //case 12: // left / top square
        //  svg.rect(0, 0, side_length, side_length,
        //            styles.merge({"transform" => "translate(#{side_length/2}, #{side_length/2}) rotate(-30, 0, 0)"}))
        //    break;
        //case 13: // right / top square
        //  svg.rect(0, 0, side_length, side_length,
        //            styles.merge({"transform" => "scale(-1, 1) translate(#{-tile_width+side_length/2}, #{side_length/2}) rotate(-30, 0, 0)" }))
        //    break;
        //case 14: // left / center-top square
        //  svg.rect(0, 0, side_length, side_length,
        //            styles.merge({"transform" => "translate(#{side_length/2}, #{tile_height/2-side_length/2-side_length}) rotate(30, 0, #{side_length})" }))
        //    break;
        //case 15: // right / center-top square
        //  svg.rect(0, 0, side_length, side_length,
        //            styles.merge({"transform" => "scale(-1, 1) translate(#{-tile_width+side_length/2}, #{tile_height/2-side_length/2-side_length}) rotate(30, 0, #{side_length})" }))
        //    break;
        //case 16: // left / center-top square
        //  svg.rect(0, 0, side_length, side_length,
        //            styles.merge({"transform" => "scale(1, -1) translate(#{side_length/2}, #{-tile_height+tile_height/2-side_length/2-side_length}) rotate(30, 0, #{side_length})" }))
        //    break;
        //case 17: // right / center-bottom square
        //  svg.rect(0, 0, side_length, side_length,
        //            styles.merge({"transform" => "scale(-1, -1) translate(#{-tile_width+side_length/2}, #{-tile_height+tile_height/2-side_length/2-side_length}) rotate(30, 0, #{side_length})" }))
        //    break;
        //case 18: // left / bottom square
        //  svg.rect(0, 0, side_length, side_length,
        //            styles.merge({"transform" => "scale(1, -1) translate(#{side_length/2}, #{-tile_height+side_length/2}) rotate(-30, 0, 0)"}))
        //    break;
        //case 19: // right / bottom square
        //  svg.rect(0, 0, side_length, side_length,
        //            styles.merge({"transform" => "scale(-1, -1) translate(#{-tile_width+side_length/2}, #{-tile_height+side_length/2}) rotate(-30, 0, 0)"}))
        //    break;
        }
        }
        }

        private string build_rotated_triangle_shape(double side_length, double width)
        {
            var half_height = side_length / 2;
            return string.Format("0, 0, {0}, {1}, 0, {2}, 0, 0",
                width, half_height, side_length);
        }

        private string build_right_triangle_shape(double side_length)
        {
            return string.Format("0, 0, {0}, {0}, 0, {0}, 0, 0", side_length);
        }

        private string build_diamond_shape(double width, double height)
        {
            return string.Format("{0}, 0, {1}, {2}, {3}, {4}, 0, {5}",
                width / 2, width, height / 2, width / 2, height, height / 2);
        }

        private string BuildTriangleShape(double side_length, double height)
        {
            var half_width = side_length / 2;
            return string.Format("{0}, 0, {1}, {2}, 0, {3}, {4}, 0",
                half_width, side_length, height, height, half_width);
        }

        private string BuildOctagonShape(double squareSize)
        {
            var s = squareSize;
            var c = s * 0.33;
            return string.Format("{0},0,{1},0,{2},{3},{4},{5},{6},{7},{8},{9},0,{10},0,{11},{12},0",
                c, s - c, s, c, s, s - c, s - c, s, c, s, s - c, c, c);
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

        private void BuildPlusShape(double square_size, Dictionary<string, object> args = null)
        {
            Svg.BeginGroup(args)
               .Rect(square_size, 0, square_size, square_size * 3)
               .Rect(0, square_size, square_size * 3, square_size)
               .EndGroup();
        }

        //-----------------------------------------------------

        public int HexVal(int index, int len)
        {
            return int.Parse(Hash.Substring(index, len), NumberStyles.HexNumber);
        }

        public string FillColor(double val)
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

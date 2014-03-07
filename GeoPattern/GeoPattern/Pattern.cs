using GeoPattern.Generators;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoPattern
{
    public static class Pattern
    {
        public static Dictionary<string, object> Defaults = new Dictionary<string, object>
        {
            { "base_color", "#933c3c" },
            { "fill_color_dark", "#222222" },
            { "fill_color_light", "#dddddd" },
            { "stroke_color", "#000000" },
            { "stroke_opacity", 0.02 },
            { "opacity_min", 0.02 },
            { "opacity_max", 0.15 }
        };

        public static Dictionary<string, Type> Generators = new Dictionary<string, Type>
        {
            { "octagons", typeof(OctagonsGenerator) },
            { "overlapping_circles", typeof(OverlappingCirclesGenerator) },
            { "plus_signs", typeof(PlusSignsGenerator) },
            { "xes", typeof(XesGenerator) },
            { "sine_waves", typeof(SineWavesGenerator) },
            { "hexagons", typeof(HexagonsGenerator) },
            { "overlapping_rings", typeof(OverlappingRingsGenerator) },
            { "plaid", typeof(PlaidGenerator) },
            { "triangles", typeof(TrianglesGenerator) },
            { "squares", typeof(SquaresGenerator) },
            { "concentric_circles", typeof(ConcentricCirclesGenerator) },
            { "diamonds", typeof(DiamondsGenerator) },
            { "tessellation", typeof(TessellationGenerator) },
            { "nested_squares", typeof(NestedSquaresGenerator) },
            { "mosaic_squares", typeof(MosaicSquaresGenerator) },
            { "triangles_rotated", typeof(TrianglesRotatedGenerator) },
            { "chevrons", typeof(ChevronsGenerator) }
        };

        public static PatternGenerator Generator(string str, string generator, Dictionary<string, object> options = null)
        {
            return (PatternGenerator)Activator.CreateInstance(Generators[generator], str, Defaults.Merge(options));
        }

        public static PatternGenerator Generator(string str, Dictionary<string, object> options = null)
        {
            return Generator(str, Generators.ToList()[int.Parse(str.ToBase64().Substring(20, 1), NumberStyles.HexNumber)].Key, options);
        }
    }
}

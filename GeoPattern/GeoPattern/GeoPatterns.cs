namespace GeoPattern
{
    using GeoPattern.Patterns;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    public static class GeoPatterns
    {
        private static Dictionary<string, object> Defaults = new Dictionary<string,object>();
        private static Dictionary<string, Type> Patterns = new Dictionary<string, Type>();

        static GeoPatterns()
        {
            Options();
            Register<OctagonsPattern>("octagons");
            Register<OverlappingCirclesPattern>("overlapping_circles");
            Register<PlusSignsPattern>("plus_signs");
            Register<XesPattern>("xes");
            Register<SineWavesPattern>("sine_waves");
            Register<HexagonsPattern>("hexagons");
            Register<OverlappingRingsPattern>("overlapping_rings");
            Register<PlaidPattern>("plaid");
            Register<TrianglesPattern>("triangles");
            Register<SquaresPattern>("squares");
            Register<ConcentricCirclesPattern>("concentric_circles");
            Register<DiamondsPattern>("diamonds");
            Register<TessellationPattern>("tessellation");
            Register<NestedSquaresPattern>("nested_squares");
            Register<MosaicSquaresPattern>("mosaic_squares");
            Register<TrianglesRotatedPattern>("triangles_rotated");
            Register<ChevronsPattern>("chevrons");
        }

        public static Pattern Generate(string customString, string patternName, Dictionary<string, object> options = null)
        {
            var pattern = (Pattern)Activator.CreateInstance(Patterns[patternName]);
            pattern.Options = Defaults.Merge(options);
            pattern.Hash = customString.Sha1HexDigest();
            pattern.Svg = new Svg();
            pattern.Generate();
            return pattern;
        }

        public static Pattern Generate(string customString, Dictionary<string, object> options = null)
        {
            var index = int.Parse(customString.ToBase64().Sha1HexDigest().Substring(20, 4), NumberStyles.HexNumber);
            var patternName = Patterns.ToList()[index % Patterns.Count].Key;
            return Generate(customString, patternName, options);
        }

        public static void Register<T>(string name) where T : Pattern, new()
        {
            Patterns[name] = typeof(T);
        }

        public static List<string> PatternNames()
        {
            return Patterns.Keys.ToList();
        }

        public static Dictionary<string, object> Options(string baseColor = "#933c3c", string fillColorDark = "#222", 
                                   string fillColorLight = "#ddd", string strokeColor = "#000", 
                                   double strokeOpacity = 0.02, double opacityMin = 0.02, 
                                   double opacityMax = 0.15)
        {
            var options = new Dictionary<string, object>
            {
                {"base_color",       baseColor},
                {"fill_color_dark",  fillColorDark},
                {"fill_color_light", fillColorLight},
                {"stroke_color",     strokeColor},
                {"stroke_opacity",   strokeOpacity},
                {"opacity_min",      opacityMin},
                {"opacity_max",      opacityMax}
            };
            Defaults = Defaults.Merge(options);
            return options;
        }
    }
}

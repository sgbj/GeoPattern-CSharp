namespace GeoPattern.Patterns
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Globalization;

    public abstract class Pattern
    {
        internal Dictionary<string, object> Options;
        internal string Hash;
        internal Svg Svg;

        public Pattern Generate()
        {
            GenerateBackground();
            GeneratePattern();
            return this;
        }

        protected abstract void GeneratePattern();

        protected void GenerateBackground()
        {
            var hueOffset = HexVal(14, 3).MapTo(0, 4095, 0, 1);
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

        protected int HexVal(int index, int len)
        {
            return int.Parse(Hash.Substring(index, len), NumberStyles.HexNumber);
        }

        protected string FillColor(double val)
        {
            return (string)(val % 2 == 0 ? Options["fill_color_light"] : Options["fill_color_dark"]);
        }

        protected double Opacity(double val)
        {
            return val.MapTo(0, 15, (double)Options["opacity_min"], (double)Options["opacity_max"]);
        }

        protected Dictionary<string, object> Transform(string format, params object[] args)
        {
            return new Dictionary<string, object>
            {
                { "transform", string.Format(format, args) }
            };
        }

        public string SvgString()
        {
            return Svg.ToString();
        }

        public string Base64String()
        {
            return Svg.ToString().ToBase64();
        }

        public string UriImage()
        {
            return string.Format("url(data:image/svg+xml;base64,{0});", Base64String());
        }

        public string UriData()
        {
            return string.Format("data:image/svg+xml;base64,{0}", Base64String());
        }
    }
}

namespace GeoPattern
{
    using System.Collections.Generic;
    using System.Text;

    public class Svg
    {
        public object Width { get; set; }
        public object Height { get; set; }
        private StringBuilder SvgString = new StringBuilder();

        public Svg(int width = 100, int height = 100)
        {
            Width = width;
            Height = height;
        }

        private string SvgHeader()
        {
            return string.Format(@"<svg xmlns=""http://www.w3.org/2000/svg"" width=""{0}"" height=""{1}"">",
                Width, Height);
        }

        private string SvgCloser()
        {
            return "</svg>";
        }

        public override string ToString()
        {
            return SvgHeader() + SvgString + SvgCloser();
        }

        public Svg Rect(object x, object y, object width, object height, Dictionary<string, object> args = null)
        {
            SvgString.AppendFormat(@"<rect x=""{0}"" y=""{1}"" width=""{2}"" height=""{3}"" {4} />",
                x, y, width, height, WriteArgs(args));
            return this;
        }

        public Svg Circle(object cx, object cy, object r, Dictionary<string, object> args = null)
        {
            SvgString.AppendFormat(@"<circle cx=""{0}"" cy=""{1}"" r=""{2}"" {3} />",
                cx, cy, r, WriteArgs(args));
            return this;
        }

        public Svg Path(string str, Dictionary<string, object> args = null)
        {
            SvgString.AppendFormat(@"<path d=""{0}"" {1} />",
                str, WriteArgs(args));
            return this;
        }

        public Svg PolyLine(string str, Dictionary<string, object> args = null)
        {
            SvgString.AppendFormat(@"<polyline points=""{0}"" {1} />",
                str, WriteArgs(args));
            return this;
        }

        public Svg BeginGroup(Dictionary<string, object> args = null)
        {
            SvgString.AppendFormat(@"<g {0}>",
                WriteArgs(args));
            return this;
        }

        public Svg EndGroup()
        {
            SvgString.Append("</g>");
            return this;
        }

        private string WriteArgs(Dictionary<string, object> args)
        {
            return args == null ? "" : args.ToAttributeString();
        }
    }
}

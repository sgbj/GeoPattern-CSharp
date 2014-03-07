namespace GeoPattern.Patterns
{
    using System.Collections.Generic;

    public class ChevronsPattern : Pattern
    {

        protected override void GeneratePattern()
        {
            var chevron_width = HexVal(0, 1).MapTo(0, 15, 30, 80);
            var chevron_height = HexVal(0, 1).MapTo(0, 15, 30, 80);

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
                        { "stroke", Options["stroke_color"] },
                        { "stroke-opacity", Options["stroke_opacity"] },
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
    }
}

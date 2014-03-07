namespace GeoPattern.Patterns
{
    using System.Collections.Generic;

    public class DiamondsPattern : Pattern
    {

        protected override void GeneratePattern()
        {
            var diamond_width = HexVal(0, 1).MapTo(0, 15, 10, 50);
            var diamond_height = HexVal(1, 1).MapTo(0, 15, 10, 50);
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
                        {"stroke"         , Options["stroke_color"]},
                        {"stroke-opacity" , Options["stroke_opacity"]}
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

        private string build_diamond_shape(double width, double height)
        {
            return string.Format("{0}, 0, {1}, {2}, {3}, {4}, 0, {5}",
                width / 2, width, height / 2, width / 2, height, height / 2);
        }
    }
}

namespace GeoPattern.Patterns
{
    using System;
    using System.Collections.Generic;

    public class TrianglesPattern : Pattern
    {

        protected override void GeneratePattern()
        {
            var scale = HexVal(0, 1);
            var side_length = scale.MapTo(0, 15, 15, 80);
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
                        {"stroke"         , Options["stroke_color"]},
                        {"stroke-opacity" , Options["stroke_opacity"]}
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

        private string BuildTriangleShape(double side_length, double height)
        {
            var half_width = side_length / 2;
            return string.Format("{0}, 0, {1}, {2}, 0, {3}, {4}, 0",
                half_width, side_length, height, height, half_width);
        }
    }
}

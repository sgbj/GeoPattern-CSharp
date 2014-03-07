using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoPattern.Generators
{
    public class TrianglesRotatedGenerator : PatternGenerator
    {
        public TrianglesRotatedGenerator(string str, Dictionary<string, object> options) : base(str, options)
        {
        }

        protected override void Generate()
        {
            var scale = HexVal(0, 1);
            var side_length = scale.MapTo(0, 15, 15, 80);
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

        private string build_rotated_triangle_shape(double side_length, double width)
        {
            var half_height = side_length / 2;
            return string.Format("0, 0, {0}, {1}, 0, {2}, 0, 0",
                width, half_height, side_length);
        }
    }
}

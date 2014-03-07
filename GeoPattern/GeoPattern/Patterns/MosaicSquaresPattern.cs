namespace GeoPattern.Patterns
{
    using System.Collections.Generic;

    public class MosaicSquaresPattern : Pattern
    {

        protected override void GeneratePattern()
        {
            var triangle_size = HexVal(0, 1).MapTo(0, 15, 15, 50);

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
                {"stroke"         , Options["stroke_color"]},
                {"stroke-opacity" , Options["stroke_opacity"]},
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
                {"stroke"         , Options["stroke_color"]},
                {"stroke-opacity" , Options["stroke_opacity"]},
                {"fill-opacity"   , opacity},
                {"fill"           , fill}
            };
            Svg.PolyLine(triangle, styles.Merge(Transform("translate({0}, {1}) scale(-1, 1)", x + triangle_size, y)));
            Svg.PolyLine(triangle, styles.Merge(Transform("translate({0}, {1}) scale(1, -1)", x + triangle_size, y + triangle_size * 2)));

            opacity = Opacity(vals[1]);
            fill = FillColor(vals[1]);
            styles = new Dictionary<string, object>{
                {"stroke"         , Options["stroke_color"]},
                {"stroke-opacity" , Options["stroke_opacity"]},
                {"fill-opacity"   , opacity},
                {"fill"           , fill}
            };
            Svg.PolyLine(triangle, styles.Merge(Transform("translate({0}, {1}) scale(-1, -1)", x + triangle_size, y + triangle_size * 2)));
            Svg.PolyLine(triangle, styles.Merge(Transform("translate({0}, {1}) scale(1, 1)", x + triangle_size, y)));
        }

        private string build_right_triangle_shape(double side_length)
        {
            return string.Format("0, 0, {0}, {0}, 0, {0}, 0, 0", side_length);
        }
    }
}

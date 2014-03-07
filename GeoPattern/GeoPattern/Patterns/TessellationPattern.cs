namespace GeoPattern.Patterns
{
    using System;
    using System.Collections.Generic;

    public class TessellationPattern : Pattern
    {

        protected override void GeneratePattern()
        {
            // 3.4.6.4 semi-regular tessellation
            var side_length = HexVal(0, 1).MapTo(0, 15, 5, 40);
            var hex_height = side_length * Math.Sqrt(3);
            var hex_width = side_length * 2;
            var triangle_height = side_length / 2 * Math.Sqrt(3);
            var triangle = build_rotated_triangle_shape(side_length, triangle_height);
            var tile_width = side_length * 3 + triangle_height * 2;
            var tile_height = (hex_height * 2) + (side_length * 2);

            Svg.Width = tile_width;
            Svg.Height = tile_height;

            for (var i = 0; i <= 19; i++)
            {
                var val = HexVal(i, 1);
                var opacity = Opacity(val);
                var fill = FillColor(val);

                var styles = new Dictionary<string, object>{
                    {"stroke"         , Options["stroke_color"]},
                    {"stroke-opacity" , Options["stroke_opacity"]},
                    {"fill"           , fill},
                    {"fill-opacity"   , opacity},
                    {"stroke-width"   , 1}
                };

                switch (i)
                {
                    case 0: // all 4 corners
                        Svg.Rect(-side_length / 2, -side_length / 2, side_length, side_length, styles);
                        Svg.Rect(tile_width - side_length / 2, -side_length / 2, side_length, side_length, styles);
                        Svg.Rect(-side_length / 2, tile_height - side_length / 2, side_length, side_length, styles);
                        Svg.Rect(tile_width - side_length / 2, tile_height - side_length / 2, side_length, side_length, styles);
                        break;
                    case 1: // center / top square
                        Svg.Rect(hex_width / 2 + triangle_height, hex_height / 2, side_length, side_length, styles);
                        break;
                    case 2: // side squares
                        Svg.Rect(-side_length / 2, tile_height / 2 - side_length / 2, side_length, side_length, styles);
                        Svg.Rect(tile_width - side_length / 2, tile_height / 2 - side_length / 2, side_length, side_length, styles);
                        break;
                    case 3: // center / bottom square
                        Svg.Rect(hex_width / 2 + triangle_height, hex_height * 1.5 + side_length, side_length, side_length, styles);
                        break;
                    case 4: // left top / bottom triangle
                        Svg.PolyLine(triangle, styles.Merge(Transform("translate({0}, {1}) rotate(0, {2}, {3})", side_length / 2, -side_length / 2, side_length / 2, triangle_height / 2)));
                        Svg.PolyLine(triangle, styles.Merge(Transform("translate({0}, {1}) rotate(0, {2}, {3}) scale(1, -1)", side_length / 2, tile_height + side_length / 2, side_length / 2, triangle_height / 2))); // arg {1} was --, now +
                        break;
                    case 5: // right top / bottom triangle
                        Svg.PolyLine(triangle, styles.Merge(Transform("translate({0}, {1}) rotate(0, {2}, {3}) scale(-1, 1)", tile_width - side_length / 2, -side_length / 2, side_length / 2, triangle_height / 2)));
                        Svg.PolyLine(triangle, styles.Merge(Transform("translate({0}, {1}) rotate(0, {2}, {3}) scale(-1, -1)", tile_width - side_length / 2, tile_height + side_length / 2, side_length / 2, triangle_height / 2)));
                        break;
                    case 6: // center / top / right triangle
                        Svg.PolyLine(triangle, styles.Merge(Transform("translate({0}, {1})", tile_width / 2 + side_length / 2, hex_height / 2)));
                        break;
                    case 7: // center / top / left triangle
                        Svg.PolyLine(triangle, styles.Merge(Transform("translate({0}, {1}) scale(-1, 1)", tile_width - tile_width / 2 - side_length / 2, hex_height / 2)));
                        break;
                    case 8: // center / bottom / right triangle
                        Svg.PolyLine(triangle, styles.Merge(Transform("translate({0}, {1}) scale(1, -1)", tile_width / 2 + side_length / 2, tile_height - hex_height / 2)));
                        break;
                    case 9: // center / bottom / left triangle
                        Svg.PolyLine(triangle, styles.Merge(Transform("translate({0}, {1}) scale(-1, -1)", tile_width - tile_width / 2 - side_length / 2, tile_height - hex_height / 2)));
                        break;
                    case 10: // left / middle triangle
                        Svg.PolyLine(triangle, styles.Merge(Transform("translate({0}, {1})", side_length / 2, tile_height / 2 - side_length / 2)));
                        break;
                    case 11: // right / middle triangle
                        Svg.PolyLine(triangle, styles.Merge(Transform("translate({0}, {1}) scale(-1, 1)", tile_width - side_length / 2, tile_height / 2 - side_length / 2)));
                        break;
                    case 12: // left / top square
                        Svg.Rect(0, 0, side_length, side_length, styles.Merge(Transform("translate({0}, {1}) rotate(-30, 0, 0)", side_length / 2, side_length / 2)));
                        break;
                    case 13: // right / top square
                        Svg.Rect(0, 0, side_length, side_length, styles.Merge(Transform("scale(-1, 1) translate({0}, {1}) rotate(-30, 0, 0)", -tile_width + side_length / 2, side_length / 2)));
                        break;
                    case 14: // left / center-top square
                        Svg.Rect(0, 0, side_length, side_length, styles.Merge(Transform("translate({0}, {1}) rotate(30, 0, {2})", side_length / 2, tile_height / 2 - side_length / 2 - side_length, side_length)));
                        break;
                    case 15: // right / center-top square
                        Svg.Rect(0, 0, side_length, side_length, styles.Merge(Transform("scale(-1, 1) translate({0}, {1}) rotate(30, 0, {2})", -tile_width + side_length / 2, tile_height / 2 - side_length / 2 - side_length, side_length)));
                        break;
                    case 16: // left / center-top square
                        Svg.Rect(0, 0, side_length, side_length, styles.Merge(Transform("scale(1, -1) translate({0}, {1}) rotate(30, 0, {2})", side_length / 2, -tile_height + tile_height / 2 - side_length / 2 - side_length, side_length)));
                        break;
                    case 17: // right / center-bottom square
                        Svg.Rect(0, 0, side_length, side_length, styles.Merge(Transform("scale(-1, -1) translate({0}, {1}) rotate(30, 0, {2})", -tile_width + side_length / 2, -tile_height + tile_height / 2 - side_length / 2 - side_length, side_length)));
                        break;
                    case 18: // left / bottom square
                        Svg.Rect(0, 0, side_length, side_length, styles.Merge(Transform("scale(1, -1) translate({0}, {1}) rotate(-30, 0, 0)", side_length / 2, -tile_height + side_length / 2)));
                        break;
                    case 19: // right / bottom square
                        Svg.Rect(0, 0, side_length, side_length, styles.Merge(Transform("scale(-1, -1) translate({0}, {1}) rotate(-30, 0, 0)", -tile_width + side_length / 2, -tile_height + side_length / 2)));
                        break;
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

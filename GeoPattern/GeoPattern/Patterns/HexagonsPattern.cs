namespace GeoPattern.Patterns
{
    using System;
    using System.Collections.Generic;

    public class HexagonsPattern : Pattern
    {
        
        protected override void GeneratePattern()
        {
            var scale = HexVal(0, 1);
            var sideLength = scale.MapTo(0, 15, 8, 60);
            var hexHeight = sideLength * Math.Sqrt(3);
            var hexWidth = sideLength * 2;
            var hex = BuildHexagonShape(sideLength);

            Svg.Width = (hexWidth * 3) + (sideLength * 3);
            Svg.Height = hexHeight * 6;

            var i = 0;

            for (var y = 0; y <= 5; y++)
            {
                for (var x = 0; x <= 5; x++)
                {
                    var val = HexVal(i, 1);
                    var dy = x % 2 == 0 ? y * hexHeight : y * hexHeight + hexHeight / 2;
                    var opacity = Opacity(val);
                    var fill = FillColor(val);

                    var styles = new Dictionary<string, object>
                    {
                        { "fill", fill },
                        { "fill-opacity", opacity },
                        { "stroke", Options["stroke_color"] },
                        { "stroke-opacity", Options["stroke_opacity"] }
                    };

                    Svg.PolyLine(hex, styles.Merge(Transform("translate({0}, {1})", x * sideLength * 1.5 - hexWidth / 2, dy - hexHeight / 2)));

                    // Add an extra one at top-right, for tiling.
                    if (x == 0)
                    {
                        Svg.PolyLine(hex, styles.Merge(Transform("translate({0}, {1})", 6 * sideLength * 1.5 - hexWidth / 2, dy - hexHeight / 2)));
                    }

                    // Add an extra row at the end that matches the first row, for tiling.
                    if (y == 0)
                    {
                        dy = x % 2 == 0 ? 6 * hexHeight : 6 * hexHeight + hexHeight / 2;
                        Svg.PolyLine(hex, styles.Merge(Transform("translate({0}, {1})", x * sideLength * 1.5 - hexWidth / 2, dy - hexHeight / 2)));
                    }

                    // Add an extra one at bottom-right, for tiling.
                    if (x == 0 && y == 0)
                    {
                        Svg.PolyLine(hex, styles.Merge(Transform("translate({0}, {1})", 6 * sideLength * 1.5 - hexWidth / 2, 5 * hexHeight + hexHeight / 2)));
                    }

                    i++;
                }
            }
        }

        private string BuildHexagonShape(double sideLength)
        {
            var c = sideLength;
            var a = c / 2;
            var b = Math.Sin(60 * Math.PI / 180) * c;
            return string.Format("0,{0},{1},0,{2},0,{3},{4},{5},{6},{7},{8},0,{9}",
                b, a, a + c, 2 * c, b, a + c, 2 * b, a, 2 * b, b);
        }
    }
}

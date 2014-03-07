namespace GeoPattern.Patterns
{
    using System.Collections.Generic;

    public class OverlappingCirclesPattern : Pattern
    {

        protected override void GeneratePattern()
        {
            var scale = HexVal(0, 1);
            var diameter = scale.MapTo(0, 15, 25, 200);
            var radius = diameter / 2;

            Svg.Width = radius * 6;
            Svg.Height = radius * 6;

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
                        {"fill", fill},
                        {"style", new Dictionary<string, object> 
                            {
                                {"opacity", opacity}
                            }
                        }
                    };

                    Svg.Circle(x * radius, y * radius, radius, styles);

                    // Add an extra one at top-right, for tiling.
                    if (x == 0)
                    {
                        Svg.Circle(6 * radius, y * radius, radius, styles);
                    }

                    // Add an extra row at the end that matches the first row, for tiling.
                    if (y == 0)
                    {
                        Svg.Circle(x * radius, 6 * radius, radius, styles);
                    }

                    // Add an extra one at bottom-right, for tiling.
                    if (x == 0 && y == 0)
                    {
                        Svg.Circle(6 * radius, 6 * radius, radius, styles);
                    }

                    i++;
                }
            }
        }
    }
}

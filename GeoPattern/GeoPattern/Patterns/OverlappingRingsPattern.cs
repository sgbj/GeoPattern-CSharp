namespace GeoPattern.Patterns
{
    using System.Collections.Generic;

    public class OverlappingRingsPattern : Pattern
    {

        protected override void GeneratePattern()
        {
            var scale = HexVal(0, 1);
            var ring_size = scale.MapTo(0, 15, 10, 60);
            var stroke_width = ring_size / 4;

            Svg.Width = ring_size * 6;
            Svg.Height = ring_size * 6;

            var i = 0;

            for (var y = 0; y <= 5; y++)
            {
                for (var x = 0; x <= 5; x++)
                {
                    var val = HexVal(i, 1);
                    var opacity = Opacity(val);
                    var fill = FillColor(val);

                    var styles = new Dictionary<string, object> {
                        {"fill", "none"},
                        {"stroke", fill},
                        {
                            "style", new Dictionary<string, object> {
                                {"opacity", opacity},
                                {"stroke-width", stroke_width + "px"}
                            }
                        }
                    };

                    Svg.Circle(x * ring_size, y * ring_size, ring_size - stroke_width / 2, styles);

                    // Add an extra one at top-right, for tiling.
                    if (x == 0)
                    {
                        Svg.Circle(6 * ring_size, y * ring_size, ring_size - stroke_width / 2, styles);
                    }

                    if (y == 0)
                    {
                        Svg.Circle(x * ring_size, 6 * ring_size, ring_size - stroke_width / 2, styles);
                    }

                    if (x == 0 && y == 0)
                    {
                        Svg.Circle(6 * ring_size, 6 * ring_size, ring_size - stroke_width / 2, styles);
                    }

                    i++;
                }
            }
        }
    }
}

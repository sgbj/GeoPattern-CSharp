namespace GeoPattern.Patterns
{
    using System.Collections.Generic;

    public class ConcentricCirclesPattern : Pattern
    {

        protected override void GeneratePattern()
        {
            var scale = HexVal(0, 1);
            var ring_size = scale.MapTo(0, 15, 10, 60);
            var stroke_width = ring_size / 5;

            Svg.Width = (ring_size + stroke_width) * 6;
            Svg.Height = (ring_size + stroke_width) * 6;

            var i = 0;

            for (var y = 0; y <= 5; y++)
            {
                for (var x = 0; x <= 5; x++)
                {
                    var val = HexVal(i, 1);
                    var opacity = Opacity(val);
                    var fill = FillColor(val);

                    Svg.Circle(x * ring_size + x * stroke_width + (ring_size + stroke_width) / 2,
                        y * ring_size + y * stroke_width + (ring_size + stroke_width) / 2,
                        ring_size / 2,
                        new Dictionary<string, object> {
                            {"fill", "none"},
                            {"stroke", fill},
                            {
                                "style", new Dictionary<string, object> {
                                    {"opacity", opacity},
                                    {"stroke-width", stroke_width + "px"}
                                }
                            }
                        });

                    val = HexVal(39 - i, 1);
                    opacity = Opacity(val);
                    fill = FillColor(val);

                    Svg.Circle(x * ring_size + x * stroke_width + (ring_size + stroke_width) / 2,
                              y * ring_size + y * stroke_width + (ring_size + stroke_width) / 2,
                              ring_size / 4,
                              new Dictionary<string, object> {
                            {"fill", fill},
                            {"fill-opacity", opacity}
                        });

                    i++;
                }
            }
        }
    }
}

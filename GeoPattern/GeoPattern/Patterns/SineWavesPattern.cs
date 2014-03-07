namespace GeoPattern.Patterns
{
    using System;
    using System.Collections.Generic;

    public class SineWavesPattern : Pattern
    {

        protected override void GeneratePattern()
        {
            var period = Math.Floor(HexVal(0, 1).MapTo(0, 15, 100, 400));
            var amplitude = Math.Floor(HexVal(1, 1).MapTo(0, 15, 30, 100));
            var waveWidth = Math.Floor(HexVal(2, 1).MapTo(0, 15, 3, 30));

            Svg.Width = period;
            Svg.Height = waveWidth * 36;

            for (var i = 0; i <= 35; i++)
            {
                var val = HexVal(i, 1);
                var opacity = Opacity(val);
                var fill = FillColor(val);
                var xOffset = period / 4 * 0.7;

                var styles = new Dictionary<string, object>
                {
                    { "fill", "none" },
                    { "stroke", fill },
                    { "style", 
                        new Dictionary<string, object> 
                        {
                            { "opacity", opacity },
                            { "stroke-width", waveWidth + "px" }
                        }
                    }
                };

                var str = "M0 " + amplitude
                        + " C " + xOffset + " 0, " + (period / 2 - xOffset) + " 0, " + (period / 2) + " " + amplitude
                        + " S " + (period - xOffset) + " " + (amplitude * 2) + ", " + period + " " + amplitude
                        + " S " + (period * 1.5 - xOffset) + " 0, " + (period * 1.5) + ", " + amplitude;

                Svg.Path(str, styles.Merge(Transform("translate(-{0}, {1})", period / 4, waveWidth * i - amplitude * 1.5)));
                Svg.Path(str, styles.Merge(Transform("translate(-{0}, {1})", period / 4, waveWidth * i - amplitude * 1.5 + waveWidth * 36)));
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoPattern.Generators
{
    public class PlaidGenerator : PatternGenerator
    {
        public PlaidGenerator(string str, Dictionary<string, object> options) : base(str, options)
        {
        }

        protected override void Generate()
        {
            var height = 0.0;
            var width = 0.0;

            // horizontal stripes
            var i = 0;
            for (var j = 0; j < 18; j++)
            {
                var space = HexVal(i, 1);
                height += space + 5;

                var val = HexVal(i + 1, 1);
                var opacity = Opacity(val);
                var fill = FillColor(val);
                var stripe_height = val + 5;

                Svg.Rect(0, height, "100%", stripe_height, new Dictionary<string, object> {
                      {"opacity", opacity},
                      {"fill", fill}
                });

                height += stripe_height;
                i += 2;
            }

            // vertical stripes
            i = 0;
            for (var j = 0; j < 18; j++)
            {
                var space = HexVal(i, 1);
                width += space + 5;

                var val = HexVal(i + 1, 1);
                var opacity = Opacity(val);
                var fill = FillColor(val);
                var stripe_width = val + 5;

                Svg.Rect(width, 0, stripe_width, "100%", new Dictionary<string, object>{
                      {"opacity", opacity},
                      {"fill", fill}
                });

                width += stripe_width;
                i += 2;
            }

            Svg.Width = width;
            Svg.Height = height;
        }
    }
}

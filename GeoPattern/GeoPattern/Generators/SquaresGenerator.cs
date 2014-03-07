using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoPattern.Generators
{
    public class SquaresGenerator : PatternGenerator
    {
        public SquaresGenerator(string str, Dictionary<string, object> options) : base(str, options)
        {
        }

        protected override void Generate()
        {
            var square_size = HexVal(0, 1).MapTo(0, 15, 10, 60);

            Svg.Width = square_size * 6;
            Svg.Height = square_size * 6;

            var i = 0;

            for (var y = 0; y <= 5; y++)
            {
                for (var x = 0; x <= 5; x++)
                {
                    var val = HexVal(i, 1);
                    var opacity = Opacity(val);
                    var fill = FillColor(val);

                    Svg.Rect(x * square_size, y * square_size, square_size, square_size, new Dictionary<string, object> {
                        {"fill", fill},
                        {"fill-opacity", opacity},
                        {"stroke", Options["stroke_color"]},
                        {"stroke-opacity", Options["stroke_opacity"]}
                    });

                    i++;
                }
            }
        }
    }
}

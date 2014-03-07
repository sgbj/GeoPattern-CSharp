using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoPattern.Generators
{
    public class NestedSquaresGenerator : PatternGenerator
    {
        public NestedSquaresGenerator(string str, Dictionary<string, object> options) : base(str, options)
        {
        }

        protected override void Generate()
        {
            var block_size = HexVal(0, 1).MapTo(0, 15, 4, 12);
            var square_size = block_size * 7;

            Svg.Width = (square_size + block_size) * 6 + block_size * 6;
            Svg.Height = (square_size + block_size) * 6 + block_size * 6;

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
                              {"stroke-width", block_size + "px"}
                            }
                        }
                    };

                    Svg.Rect(x * square_size + x * block_size * 2 + block_size / 2,
                              y * square_size + y * block_size * 2 + block_size / 2,
                              square_size, square_size, styles);

                    val = HexVal(39 - i, 1);
                    opacity = Opacity(val);
                    fill = FillColor(val);

                    styles = new Dictionary<string, object> {
                        {"fill", "none"},
                        {"stroke", fill},
                        {
                            "style", new Dictionary<string, object> {
                              {"opacity", opacity},
                              {"stroke-width", block_size + "px"}
                            }
                        }
                    };

                    Svg.Rect(x * square_size + x * block_size * 2 + block_size / 2 + block_size * 2,
                              y * square_size + y * block_size * 2 + block_size / 2 + block_size * 2,
                              block_size * 3, block_size * 3, styles);

                    i++;
                }
            }
        }
    }
}

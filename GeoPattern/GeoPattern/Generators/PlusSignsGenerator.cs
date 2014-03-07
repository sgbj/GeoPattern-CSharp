using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoPattern.Generators
{
    public class PlusSignsGenerator : PatternGenerator
    {
        public PlusSignsGenerator(string str, Dictionary<string, object> options) : base(str, options)
        {
        }

        protected override void Generate()
        {
            var square_size = HexVal(0, 1).MapTo(0, 15, 10, 25);
            var plus_size = square_size * 3;

            Svg.Width = square_size * 12;
            Svg.Height = square_size * 12;

            var i = 0;

            for (var y = 0; y <= 5; y++)
            {
                for (var x = 0; x <= 5; x++)
                {
                    var val = HexVal(i, 1);
                    var opacity = Opacity(val);
                    var fill = FillColor(val);
                    var dx = (y % 2 == 0) ? 0 : 1;

                    var styles = new Dictionary<string, object> 
                    {
                        {"fill", fill},
                        {"stroke", Options["stroke_color"]},
                        {"stroke-opacity", Options["stroke_opacity"]},
                        {"style", new Dictionary<string, object> 
                            {
                                {"fill-opacity", opacity}
                            }
                        }
                    };

                    BuildPlusShape(square_size, styles.Merge(Transform("translate({0},{1})", x * plus_size - x * square_size + dx * square_size - square_size, y * plus_size - y * square_size - plus_size / 2)));

                    // Add an extra column on the right for tiling.
                    if (x == 0)
                    {
                        BuildPlusShape(square_size, styles.Merge(Transform("translate({0},{1})", 4 * plus_size - x * square_size + dx * square_size - square_size, y * plus_size - y * square_size - plus_size / 2)));
                    }

                    // Add an extra row on the bottom that matches the first row, for tiling.
                    if (y == 0)
                    {
                        BuildPlusShape(square_size, styles.Merge(Transform("translate({0},{1})", x * plus_size - x * square_size + dx * square_size - square_size, 4 * plus_size - y * square_size - plus_size / 2)));
                    }

                    // Add an extra one at top-right and bottom-right, for tiling.
                    if (x == 0 && y == 0)
                    {
                        BuildPlusShape(square_size, styles.Merge(Transform("translate({0},{1})", 4 * plus_size - x * square_size + dx * square_size - square_size, 4 * plus_size - y * square_size - plus_size / 2)));
                    }

                    i++;
                }
            }
        }

        private void BuildPlusShape(double square_size, Dictionary<string, object> args = null)
        {
            Svg.BeginGroup(args)
               .Rect(square_size, 0, square_size, square_size * 3)
               .Rect(0, square_size, square_size * 3, square_size)
               .EndGroup();
        }
    }
}

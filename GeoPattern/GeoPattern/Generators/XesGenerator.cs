using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoPattern.Generators
{
    public class XesGenerator : PatternGenerator
    {
        public XesGenerator(string str, Dictionary<string, object> options) : base(str, options)
        {
        }

        protected override void Generate()
        {
            var square_size = HexVal(0, 1).MapTo(0, 15, 10, 25);
            var x_size = square_size * 3 * 0.943;

            Svg.Width = x_size * 3;
            Svg.Height = x_size * 3;

            var i = 0;

            for (var y = 0; y <= 5; y++)
            {
                for (var x = 0; x <= 5; x++)
                {
                    var val = HexVal(i, 1);
                    var opacity = Opacity(val);
                    var dy = x % 2 == 0 ? y * x_size - x_size * 0.5 : y * x_size - x_size * 0.5 + x_size / 4;
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

                    BuildPlusShape(square_size, styles.Merge(Transform("translate({0},{1}) rotate(45, {2}, {2})", x * x_size / 2 - x_size / 2, dy - y * x_size / 2, x_size / 2)));

                    // Add an extra column on the right for tiling.
                    if (x == 0)
                    {
                        BuildPlusShape(square_size, styles.Merge(Transform("translate({0},{1}) rotate(45, {2}, {2})", 6 * x_size / 2 - x_size / 2, dy - y * x_size / 2, x_size / 2)));
                    }

                    // Add an extra row on the bottom that matches the first row, for tiling.
                    if (y == 0)
                    {
                        dy = x % 2 == 0 ? 6 * x_size - x_size / 2 : 6 * x_size - x_size / 2 + x_size / 4;
                        BuildPlusShape(square_size, styles.Merge(Transform("translate({0},{1}) rotate(45, {2}, {2})", x * x_size / 2 - x_size / 2, dy - 6 * x_size / 2, x_size / 2)));
                    }

                    // These can hang off the bottom, so put a row at the top for tiling.
                    if (y == 5)
                    {
                        BuildPlusShape(square_size, styles.Merge(Transform("translate({0},{1}) rotate(45, {2}, {2})", x * x_size / 2 - x_size / 2, dy - 11 * x_size / 2, x_size / 2)));
                    }
                    // Add an extra one at top-right and bottom-right, for tiling.
                    if (x == 0 && y == 0)
                    {
                        BuildPlusShape(square_size, styles.Merge(Transform("translate({0},{1}) rotate(45, {2}, {2})", 6 * x_size / 2 - x_size / 2, dy - 6 * x_size / 2, x_size / 2)));
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

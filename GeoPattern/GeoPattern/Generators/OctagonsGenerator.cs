using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoPattern.Generators
{
    public class OctagonsGenerator : PatternGenerator
    {
        public OctagonsGenerator(string str, Dictionary<string, object> options) : base(str, options)
        {
        }

        protected override void Generate()
        {
            var square_size = HexVal(0, 1).MapTo(0, 15, 10, 60);
            var tile = BuildOctagonShape(square_size);

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

                    Svg.PolyLine(tile, new Dictionary<string, object> 
                    {
                        {"fill", fill},
                        {"fill-opacity", opacity},
                        {"stroke", Options["stroke_color"]},
                        {"stroke-opacity", Options["stroke_opacity"]},
                        {"transform", string.Format("translate({0}, {1})", x*square_size,y*square_size)}
                    });

                    i++;
                }
            }
        }

        private string BuildOctagonShape(double squareSize)
        {
            var s = squareSize;
            var c = s * 0.33;
            return string.Format("{0},0,{1},0,{2},{3},{4},{5},{6},{7},{8},{9},0,{10},0,{11},{12},0",
                c, s - c, s, c, s, s - c, s - c, s, c, s, s - c, c, c);
        }
    }
}

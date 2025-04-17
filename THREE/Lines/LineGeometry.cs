using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace THREE
{
    public class LineGeometry : LineSegmentsGeometry
    {
        public LineGeometry() : base() 
        {
            type = "LineGeometry";
        }
        public new LineGeometry SetPositions(float[] array)
        {
            var length = array.Length - 3;
            var points = new float[2 * length];

            for (var i = 0; i < length; i += 3)
            {

                points[2 * i] = array[i];
                points[2 * i + 1] = array[i + 1];
                points[2 * i + 2] = array[i + 2];

                points[2 * i + 3] = array[i + 3];
                points[2 * i + 4] = array[i + 4];
                points[2 * i + 5] = array[i + 5];

            }

            base.SetPositions(points);

            return this;
        }
        public new LineGeometry SetColors(float[] array)
        {
            var length = array.Length - 3;
            var colors = new float[2 * length];

            for (var i = 0; i < length; i += 3)
            {

                colors[2 * i] = array[i];
                colors[2 * i + 1] = array[i + 1];
                colors[2 * i + 2] = array[i + 2];

                colors[2 * i + 3] = array[i + 3];
                colors[2 * i + 4] = array[i + 4];
                colors[2 * i + 5] = array[i + 5];

            }

            base.SetColors(colors);

            return this;
        }
        public LineGeometry FromLine(Line line)
        {
            var geometry = line.Geometry as BufferGeometry;

            this.SetPositions((geometry.Attributes["position"] as BufferAttribute<float>).Array); // assumes non-indexed

            // set colors, maybe

            return this;
        }
    }
}

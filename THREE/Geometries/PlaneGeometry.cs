using System.Collections.Generic;
using System.Linq;

namespace THREE
{
    public struct PlaneParameter
    {
        public float Width;

        public float Height;

        public float WidthSegments;

        public float HeightSegments;
    }

    public class PlaneGeometry : Geometry
    {
        public PlaneParameter parameters;

        public PlaneGeometry(float width,float height,float widthSegments=0,float heightSegments=0) :base()
        {

            this.type = "PlaneGeometry";
            
         
            parameters = new PlaneParameter { 
                    Width = width, 
                    Height = height, 
                    WidthSegments = widthSegments, 
                    HeightSegments = heightSegments 
            };

            this.FromBufferGeometry(new PlaneBufferGeometry(width, height, widthSegments, heightSegments));
            this.MergeVertices();
        }
    }

    public class PlaneBufferGeometry : BufferGeometry
    {
        public PlaneParameter parameters;

        public PlaneBufferGeometry(float width, float height, float widthSegments = 0, float heightSegments = 0)
            : base()
        {
            this.type = "PlaneBufferGeometry";

            this.parameters = new PlaneParameter
            {
                Width = width,
                Height = height,
                WidthSegments = widthSegments,
                HeightSegments = heightSegments
            };

            width = width == 0 ? 1 : width;
            height = height == 0 ? 1 : height;

            widthSegments = widthSegments == 0 ? 1 : widthSegments;
            heightSegments = heightSegments == 0 ? 1 : heightSegments;

            var width_half = width / 2;
            var height_half = height / 2;



            int gridX = (int)System.Math.Floor(widthSegments);
            int gridY = (int)System.Math.Floor(heightSegments);

            int gridX1 = gridX + 1;
            int gridY1 = gridY + 1;

            var segment_width = width / gridX;
            var segment_height = height / gridY;

            List<int> indices = new List<int>();
            List<float> vertices = new List<float>();
            List<float> normals = new List<float>();
            List<float> uvs = new List<float>();

            //generate vertices, normals and uvs

            for (int iy = 0; iy < gridY1; iy++)
            {
                var y = iy * segment_height - height_half;

                for (int ix = 0; ix < gridX1; ix++)
                {
                    var x = ix * segment_width - width_half;

                    vertices.Add(x);
                    vertices.Add(-y);
                    vertices.Add(0);

                    normals.Add(0);
                    normals.Add(0);
                    normals.Add(1);

                    uvs.Add(ix / (float)gridX);
                    uvs.Add(1 - (iy /(float)gridY));
                }
            }

            // indices
            for (int iy = 0; iy < gridY; iy++)
            {
                for (int ix = 0; ix < gridX; ix++)
                {
                    var a = ix + gridX1 * iy;
                    var b = ix + gridX1 * (iy + 1);
                    var c = (ix + 1) + gridX1 * (iy + 1);
                    var d = (ix + 1) + gridX1 * iy;

                    //faces
                    indices.Add(a); indices.Add(b); indices.Add(d);
                    indices.Add(b); indices.Add(c); indices.Add(d);
                }
            }

            this.SetIndex(indices);
            this.SetAttribute("position", new BufferAttribute<float>(vertices.ToArray<float>(), 3));
            this.SetAttribute("normal", new BufferAttribute<float>(normals.ToArray<float>(), 3));
            this.SetAttribute("uv", new BufferAttribute<float>(uvs.ToArray<float>(), 2));
        }
    }
}

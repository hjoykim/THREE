using System.Collections.Generic;

namespace THREE
{
    public class BoxGeometry : Geometry
    {
        public float Width;

        public float Height;

        public float Depth;

        public int WidthSegments;

        public int HeightSegments;

        public int DepthSegments;

        public BoxGeometry(float width, float height, float depth, int widthSegments=1, int heightSegments=1, int depthSegments=1) :base()
        {

            this.type = "BoxGeometry";

            this.Width = width;

            this.Height = height;

            this.Depth = depth;

            this.WidthSegments = widthSegments;

            this.HeightSegments = heightSegments;

            this.DepthSegments = depthSegments;

            this.FromBufferGeometry(new BoxBufferGeometry(this.Width, this.Height, this.Depth, this.WidthSegments, this.HeightSegments, this.DepthSegments));

            this.MergeVertices();
        }
    }
    public class BoxBufferGeometry : BufferGeometry
    {
        public float Width;

        public float Height;

        public float Depth;

        public int WidthSegments;

        public int HeightSegments;

        public int DepthSegments;

        private List<int> indices = new List<int>();

        private List<float> vertices = new List<float>();

        private List<float> uvs = new List<float>();

        private List<float> normals = new List<float>();

        private int numberOfVertices = 0;

        private int groupStart = 0;

        public BoxBufferGeometry(float width, float height, float depth, int widthSegments = 1, int heightSegments = 1, int depthSegments = 1)
            : base()
        {

            this.Width = width;

            this.Height = height;

            this.Depth = depth == 0 ? 1 : depth;

            // segments

            this.WidthSegments = System.Math.Floor((float)widthSegments) > 0 ? widthSegments : 1;

            this.HeightSegments = System.Math.Floor((float)heightSegments) > 0 ? heightSegments : 1;

            this.DepthSegments = System.Math.Floor((float)depthSegments) > 0 ? depthSegments : 1;

            // build each side of the box geometry

            BuildPlane('z', 'y', 'x', -1, -1, this.Depth, this.Height, this.Width, this.DepthSegments, this.HeightSegments, 0); // px
            BuildPlane('z', 'y', 'x', 1, -1, this.Depth, this.Height, -this.Width, this.DepthSegments, this.HeightSegments, 1); // nx
            BuildPlane('x', 'z', 'y', 1, 1, this.Width, this.Depth, this.Height, this.WidthSegments, this.DepthSegments, 2); // py
            BuildPlane('x', 'z', 'y', 1, -1, this.Width, this.Depth, -this.Height, this.WidthSegments, this.DepthSegments, 3); // ny
            BuildPlane('x', 'y', 'z', 1, -1, this.Width, this.Height, this.Depth, this.WidthSegments, this.HeightSegments, 4); // pz
            BuildPlane('x', 'y', 'z', -1, -1, this.Width, this.Height, -this.Depth, this.WidthSegments, this.HeightSegments, 5); // nz

            this.SetIndex(this.indices);
            this.SetAttribute("position", new BufferAttribute<float>(this.vertices.ToArray(), 3));
            this.SetAttribute("normal", new BufferAttribute<float>(this.normals.ToArray(), 3));
            this.SetAttribute("uv", new BufferAttribute<float>(this.uvs.ToArray(), 2));
        }

        private void BuildPlane(char u, char v, char w, int udir, int vdir, float width, float height, float depth, int gridX, int gridY, int materialIndex)
        {
            var segmentWidth = width / gridX;
            var segmentHeight = height / gridY;

            var widthHalf = width / 2;
            var heightHalf = height / 2;
            var depthHalf = depth / 2;

            var gridX1 = gridX + 1;
            var gridY1 = gridY + 1;

            var vertexCounter = 0;
            var groupCount = 0;

            int ix, iy;

            var vector = new Vector3();

            // generate vertices, normals and uvs

            for (iy = 0; iy < gridY1; iy++)
            {

                var y = iy * segmentHeight - heightHalf;

                for (ix = 0; ix < gridX1; ix++)
                {

                    var x = ix * segmentWidth - widthHalf;


                    // set values to correct vector component

                    vector[u] = x * udir;
                    vector[v] = y * vdir;
                    vector[w] = depthHalf;

                    // now apply vector to vertex buffer

                    vertices.Add(vector.X); vertices.Add(vector.Y); vertices.Add(vector.Z);

                    // set values to correct vector component

                    vector[u] = 0;
                    vector[v] = 0;
                    vector[w] = depth > 0 ? 1 : -1;

                    // now apply vector to normal buffer
                    normals.Add(vector.X); normals.Add(vector.Y); normals.Add(vector.Z);


                    // uvs
                    uvs.Add(ix / gridX); uvs.Add(1 - (iy / gridY));

                    // counters

                    vertexCounter += 1;

                }

            }


            // indices

            // 1. you need three indices to draw a single face
            // 2. a single segment consists of two faces
            // 3. so we need to generate six (2*3) indices per segment

            for (iy = 0; iy < gridY; iy++)
            {

                for (ix = 0; ix < gridX; ix++)
                {

                    var a = numberOfVertices + ix + gridX1 * iy;
                    var b = numberOfVertices + ix + gridX1 * (iy + 1);
                    var c = numberOfVertices + (ix + 1) + gridX1 * (iy + 1);
                    var d = numberOfVertices + (ix + 1) + gridX1 * iy;

                    // faces

                    indices.Add(a);
                    indices.Add(b);
                    indices.Add(d);

                    indices.Add(b);
                    indices.Add(c);
                    indices.Add(d);

                    // increase counter
                    groupCount += 6;

                }

            }

            // add a group to the geometry. this will ensure multi material support

            this.AddGroup(groupStart, groupCount, materialIndex);

            // calculate new start value for groups

            groupStart += groupCount;

            // update total number of vertices

            numberOfVertices += vertexCounter;

        }

    }
}

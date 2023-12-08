using System.Collections;
using System.Collections.Generic;

namespace THREE
{
    public class TubeGeometry : Geometry
    {
        public List<Vector3> tangents = new List<Vector3>();

        public List<Vector3> normals = new List<Vector3>();

        public List<Vector3> binormals = new List<Vector3>();

        public Hashtable parameters;

        public TubeGeometry(Curve path,int? tubularSegments=null,float? radius=null,int? radialSegments=null,bool? closed = null) : base()
        {
            this.type = "TubeGeometry";

            parameters = new Hashtable()
            {
                {"path",path },
                {"tubularSegments",tubularSegments },
                {"radius",radius },
                {"radialSegments",radialSegments },
                {"closed",closed }
            };

            var bufferGeometry = new TubeBufferGeometry(path, tubularSegments, radius, radialSegments, closed);

            this.tangents = bufferGeometry.tangents;

            this.normals = bufferGeometry.normals;

            this.binormals = bufferGeometry.binormals;

            this.FromBufferGeometry(bufferGeometry);
            this.MergeVertices();
             
        }
    }

    public class TubeBufferGeometry : BufferGeometry
    {
        public Hashtable parameters;

        Curve path;

        int tubularSegments;

        float radius;

        int radialSegments;

        bool closed;

        public List<Vector3> tangents = new List<Vector3>();

        public List<Vector3> normals = new List<Vector3>();

        public List<Vector3> binormals = new List<Vector3>();

        private List<float> verticeList = new List<float>();

        private List<float> normalList = new List<float>();

        private List<float> uvList = new List<float>();

        private List<int> indexList = new List<int>();

        Vector3 P = new Vector3();

        Vector3 vertex = new Vector3();

        Vector3 normal = new Vector3();

        Vector2 uv = new Vector2();

        
        public TubeBufferGeometry(Curve path, int? tubularSegments = null, float? radius = null, int? radialSegments=null, bool? closed = null) : base()
        {
            this.type = "TubeBufferGeometry";

            parameters = new Hashtable()
            {
                {"path",path },
                {"tubularSegments",tubularSegments },
                {"radius",radius },
                {"radialSegments",radialSegments },
                {"closed",closed }
            };

            this.path =path;

            this.tubularSegments = tubularSegments != null ? (int)tubularSegments : 64;

            this.radius = radius != null ? (float)radius : 1;

            this.radialSegments = radialSegments != null ? (int)radialSegments : 8;

            this.closed = closed != null ? (bool)closed : false;

            var frames = path.ComputeFrenetFrames(this.tubularSegments, this.closed);

            this.tangents = (List<Vector3>)frames["tangents"];
            this.normals = (List<Vector3>)frames["normals"];
            this.binormals = (List<Vector3>)frames["binormals"];

            GenerateBufferData();

            this.SetIndex(indexList);

            this.SetAttribute("position", new BufferAttribute<float>(verticeList.ToArray(), 3));

            this.SetAttribute("normal", new BufferAttribute<float>(normalList.ToArray(), 3));

            this.SetAttribute("uv", new BufferAttribute<float>(uvList.ToArray(), 2));
        }

        private void GenerateBufferData()
        {
            for (int i = 0; i < tubularSegments; i++)
            {

                GenerateSegment(i);

            }

            // if the geometry is not closed, generate the last row of vertices and normals
            // at the regular position on the given path
            //
            // if the geometry is closed, duplicate the first row of vertices and normals (uvs will differ)

            GenerateSegment((closed == false) ? tubularSegments : 0);

            // uvs are generated in a separate function.
            // this makes it easy compute correct values for closed geometries

            GenerateUVs();

            // finally create faces

            GenerateIndices();
        }

        private void GenerateSegment(int i)
        {
            // we use getPointAt to sample evenly distributed points from the given path

            P = path.GetPointAt(i / (float)tubularSegments, P);

            // retrieve corresponding normal and binormal

            var N = normals[i];
            var B = binormals[i];

            // generate normals and vertices for the current segment

            for (int j = 0; j <= radialSegments; j++)
            {

                var v = j / (float)radialSegments * (float)System.Math.PI * 2;

                var sin = (float)System.Math.Sin(v);
                var cos = -(float)System.Math.Cos(v);

                // normal

                normal.X = (cos * N.X + sin * B.X);
                normal.Y = (cos * N.Y + sin * B.Y);
                normal.Z = (cos * N.Z + sin * B.Z);
                normal.Normalize();

                normalList.Add(normal.X, normal.Y, normal.Z);

                // vertex

                vertex.X = P.X + radius * normal.X;
                vertex.Y = P.Y + radius * normal.Y;
                vertex.Z = P.Z + radius * normal.Z;

                verticeList.Add(vertex.X, vertex.Y, vertex.Z);

            }

        }

        private void GenerateIndices()
        {
            for (int j = 1; j <= tubularSegments; j++)
            {

                for (int i = 1; i <= radialSegments; i++)
                {

                    var a = (radialSegments + 1) * (j - 1) + (i - 1);
                    var b = (radialSegments + 1) * j + (i - 1);
                    var c = (radialSegments + 1) * j + i;
                    var d = (radialSegments + 1) * (j - 1) + i;

                    // faces

                    indexList.Add(a, b, d);
                    indexList.Add(b, c, d);

                }

            }

        }

        private void GenerateUVs()
        {
            for (int i = 0; i <= tubularSegments; i++)
            {
                for (int j = 0; j <= radialSegments; j++)
                {
                    uv.X = i / tubularSegments;
                    uv.Y = j / radialSegments;

                    uvList.Add(uv.X, uv.Y);
                }
            }
        }
    }
   
}

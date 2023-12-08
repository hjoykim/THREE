using System.Collections;
using System.Collections.Generic;

namespace THREE
{
    public class CylinderGeometry : Geometry
    {
        public Hashtable parameters; 

        public CylinderGeometry(float radiusTop, float radiusBottom ,float height, int? radialSegments=null, int? heightSegments=null, bool? openEnded = null, float? thetaStart = null, float? thetaLength = null)
            : base()
        {

            parameters = new Hashtable()
            {
                {"radiusTop",radiusTop },
                {"radiusBottom",radiusBottom },
                {"height",height },
                {"radialSegments",radialSegments },
                {"heightSegments",heightSegments },
                {"openEnded",openEnded },
                {"thetaStart",thetaStart },
                {"thetaLength",thetaLength },
            };

            this.FromBufferGeometry(new CylinderBufferGeometry(radiusTop, radiusBottom, height, radialSegments, heightSegments, openEnded, thetaStart, thetaLength));
            this.MergeVertices();            
        }

    }

    public class CylinderBufferGeometry : BufferGeometry
    {
        public float RadiusTop;

        public float RadiusBottom;

        public float Height;

        public int RadialSegments;

        public int HeightSegments;

        public bool OpenEnded;

        public float ThetaStart;

        public float ThetaLength;

        private List<int> indices = new List<int>();

        private List<float> vertices = new List<float>();

        private List<float> normals = new List<float>();

        private List<float> uvs = new List<float>();

        private List<List<int>> indexArray = new List<List<int>>();

        private float halfHeight;

        private int groupStart = 0;

        private int index = 0;

        public Hashtable parameters;

        public CylinderBufferGeometry(float radiusTop, float radiusBottom, float height, int? radialSegments=null, int? heightSegments=null,  bool? openEnded = null, float? thetaStart = null, float? thetaLength = null)
            : base()
        {
            this.RadiusTop = radiusTop;//==0 ? 1:radiusTop;
            this.RadiusBottom = radiusBottom;
            this.Height = height != 0 ? height : 1;

            this.RadialSegments = radialSegments!=null ? (int)System.Math.Floor((decimal)radialSegments) :8;
            this.HeightSegments = heightSegments != null ? (int)System.Math.Floor((decimal)heightSegments) : 1;

            this.OpenEnded = openEnded != null ? (bool)openEnded : false;
            this.ThetaStart = thetaStart != null ? (float)thetaStart : 0.0f;
            this.ThetaLength = thetaLength != null ? (float)thetaLength : (float)System.Math.PI * 2;

            this.halfHeight = height / 2;

            parameters = new Hashtable()
            {
                {"radiusTop",RadiusTop },
                {"radiusBottom",RadiusBottom },
                {"height",Height },
                {"radialSegments",RadialSegments },
                {"heightSegments",HeightSegments },
                {"openEnded",OpenEnded },
                {"thetaStart",ThetaStart },
                {"thetaLength",ThetaLength },
            };

            GenerateTorso();

            if (this.OpenEnded == false)
            {
                if (this.RadiusTop > 0)  GenerateCap(true);
                if (this.RadiusBottom > 0) GenerateCap(false);
            }

            this.SetIndex(indices);
            this.SetAttribute("position", new BufferAttribute<float>(vertices.ToArray(), 3));
            this.SetAttribute("normal", new BufferAttribute<float>(normals.ToArray(), 3));
            this.SetAttribute("uv", new BufferAttribute<float>(uvs.ToArray(), 2));
        }

        private void GenerateTorso()
        {
            int x, y;
            int groupCount = 0;

            Vector3 normal = Vector3.Zero();
            Vector3 vertex = Vector3.Zero();

            float slope = (this.RadiusBottom - this.RadiusTop) / this.Height;

            for (y = 0; y <= this.HeightSegments; y++)
            {
                List<int> indexRow = new List<int>();

                float v = (float)y / (float)this.HeightSegments;

                float radius = v * (this.RadiusBottom - this.RadiusTop) + this.RadiusTop;

                for (x = 0; x <= this.RadialSegments; x++)
                {
                    float u = (float)x / (float)this.RadialSegments;
                    float theta = u * this.ThetaLength + this.ThetaStart;

                    float sinTheta = (float)System.Math.Sin(theta);
                    float cosTheta = (float)System.Math.Cos(theta);

                    //vertex

                    vertex.X = radius * sinTheta;
                    vertex.Y = -v * this.Height + this.halfHeight;
                    vertex.Z = radius * cosTheta;
                    vertices.Add(vertex.X); vertices.Add(vertex.Y); vertices.Add(vertex.Z);

                    //normal

                    normal.Set(sinTheta, slope, cosTheta).Normalize();
                    normals.Add(normal.X); normals.Add(normal.Y); normals.Add(normal.Z);

                    //uv

                    uvs.Add(u); uvs.Add(1 - v);
                    indexRow.Add(index++);
                }
                indexArray.Add(indexRow);
            }

            // generate indices

            for (x = 0; x < this.RadialSegments; x++)
            {
                for (y = 0; y < this.HeightSegments; y++)
                {
                    int a = indexArray[y][x];
                    int b = indexArray[y + 1][x];
                    int c = indexArray[y + 1][x + 1];
                    int d = indexArray[y][x + 1];

                    indices.Add(a); indices.Add(b); indices.Add(d);
                    indices.Add(b); indices.Add(c); indices.Add(d);

                    groupCount += 6;
                }
            }

            this.AddGroup(groupStart, groupCount, 0);

            groupStart += groupCount;
        }

        private void GenerateCap(bool top)
        {
            int x, centerIndexStart, centerIndexEnd;

            Vector2 uv = new Vector2();
            Vector3 vertex = new Vector3();

            int groupCount = 0;

            float radius = top ? this.RadiusTop : this.RadiusBottom;
            float sign = top ? 1 : -1;

            centerIndexStart = index;

            for (x = 1; x <= this.RadialSegments; x++)
            {
                vertices.Add(0); vertices.Add(this.halfHeight * sign); vertices.Add(0);
                normals.Add(0); normals.Add(sign); normals.Add(0);
                uvs.Add(0.5f); uvs.Add(0.5f);
                index++;
            }

            centerIndexEnd = index;

            for (x = 0; x <= this.RadialSegments; x++)
            {
                float u = (float)x / (float)this.RadialSegments;
                float theta = u * this.ThetaLength + this.ThetaStart;

                float cosTheta = (float)System.Math.Cos(theta);
                float sinTheta = (float)System.Math.Sin(theta);

                //vertex

                vertex.X = radius * sinTheta;
                vertex.Y = this.halfHeight * sign;
                vertex.Z = radius * cosTheta;

                vertices.Add(vertex.X); vertices.Add(vertex.Y); vertices.Add(vertex.Z);

                //normal
                normals.Add(0); normals.Add(sign); normals.Add(0);

                //uv

                uv.X = (cosTheta * 0.5f) + 0.5f;
                uv.Y = (sinTheta * 0.5f * sign) + 0.5f;
                uvs.Add(uv.X); uvs.Add(uv.Y);

                index++;
            }

            //generate indices

            for (x = 0; x < this.RadialSegments; x++)
            {
                int c = centerIndexStart + x;
                int i = centerIndexEnd + x;

                if (top)
                {
                    indices.Add(i); indices.Add(i + 1); indices.Add(c);
                }
                else
                {
                    indices.Add(i + 1); indices.Add(i); indices.Add(c);
                }
                groupCount += 3;
            }

            this.AddGroup(groupStart, groupCount, top ? 1 : 2);

            groupStart += groupCount;
        }

    }
}

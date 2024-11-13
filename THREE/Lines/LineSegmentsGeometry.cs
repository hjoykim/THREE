using Newtonsoft.Json.Linq;
using Rhino.Geometry;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace THREE
{
    [Serializable]
    public class LineSegmentsGeometry : InstancedBufferGeometry
    {
        Box3 _box = new Box3();
        Vector3 _vector = new Vector3();
        public LineSegmentsGeometry() : base()
        {
            float[] positions = new float[] { -1, 2, 0, 1, 2, 0, -1, 1, 0, 1, 1, 0, -1, 0, 0, 1, 0, 0, -1, -1, 0, 1, -1, 0 };
            float[] uvs = new float[] { -1, 2, 1, 2, -1, 1, 1, 1, -1, -1, 1, -1, -1, -2, 1, -2 };
            List<int> index = new List<int> { 0, 2, 1, 2, 3, 1, 2, 4, 3, 4, 5, 3, 4, 6, 5, 6, 7, 5 };

            this.SetIndex(index);
            this.SetAttribute("position", new BufferAttribute<float>(positions, 3));
            this.SetAttribute("uv", new BufferAttribute<float>(uvs, 2));
        }
        public new LineSegmentsGeometry ApplyMatrix(Matrix4 matrix)
        {
            return this.ApplyMatrix4(matrix);
        }

        public new LineSegmentsGeometry ApplyMatrix4(Matrix4 matrix )
        {

            var start = this.Attributes["instanceStart"] as InterleavedBufferAttribute<float>;
            var end = this.Attributes["instanceEnd"] as InterleavedBufferAttribute<float>;
            if (start != null && end!=null)
            {

                start.ApplyMatrix4(matrix);

                end.ApplyMatrix4(matrix);

                start.NeedsUpdate = true;

            }

            if (this.BoundingBox != null)
            {

                this.ComputeBoundingBox();

            }

            if (this.BoundingSphere != null)
            {

                this.ComputeBoundingSphere();

            }
            return this;

        }
        public virtual LineSegmentsGeometry SetPositions(float[] array )
        {
            var instanceBuffer = new InstancedInterleavedBuffer<float>(array, 6, 1); // xyz, xyz

            this.SetAttribute("instanceStart", new InterleavedBufferAttribute<float>(instanceBuffer, 3, 0)); // xyz
            this.SetAttribute("instanceEnd", new InterleavedBufferAttribute<float>(instanceBuffer, 3, 3)); // xyz

            //

            this.ComputeBoundingBox();
            this.ComputeBoundingSphere();

            return this;
        }
        public LineSegmentsGeometry SetColors(float[] array )
        {

            var instanceColorBuffer = new InstancedInterleavedBuffer<float>(array, 6, 1); // rgb, rgb

            this.SetAttribute("instanceColorStart", new InterleavedBufferAttribute<float>(instanceColorBuffer, 3, 0)); // rgb
            this.SetAttribute("instanceColorEnd", new InterleavedBufferAttribute<float>(instanceColorBuffer, 3, 3)); // rgb

            return this;

        }

        public LineSegmentsGeometry FromWireframeGeometry(BufferGeometry geometry )
        {

            this.SetPositions((geometry.Attributes["position"] as BufferAttribute<float>).Array);

            return this;

        }
        public LineSegmentsGeometry FromEdgesGeometry(BufferGeometry geometry)
        {

            this.SetPositions((geometry.Attributes["position"] as BufferAttribute<float>).Array);

            return this;

        }
        public LineSegmentsGeometry FromMesh(Mesh mesh)
        {

            this.FromWireframeGeometry(mesh.Geometry as BufferGeometry);

            return this;

        }
        public LineSegmentsGeometry FromLineSegments(LineSegments lineSegments )
        {

            var geometry = lineSegments.Geometry as BufferGeometry;
            this.SetPositions((geometry.Attributes["position"] as BufferAttribute<float>).Array); // assumes non-indexed
            // set colors, maybe
            return this;
        }
        public new void ComputeBoundingBox()
        {

            if (this.BoundingBox == null)
            {

                this.BoundingBox = new Box3();

            }

            var start = this.Attributes["instanceStart"] as InterleavedBufferAttribute<float>;
            var end = this.Attributes["instanceEnd"] as InterleavedBufferAttribute<float>;

            if (start != null && end != null)
            {

                this.BoundingBox.SetFromBufferAttribute(start);

                _box.SetFromBufferAttribute(end);

                this.BoundingBox.Union(_box);

            }

        }
        public new void ComputeBoundingSphere()
        {

            if (this.BoundingSphere == null)
            {

                this.BoundingSphere = new Sphere();

            }

            if (this.BoundingBox == null)
            {

                this.ComputeBoundingBox();

            }

            var start = this.Attributes["instanceStart"] as InterleavedBufferAttribute<float>;
            var end = this.Attributes["instanceEnd"] as InterleavedBufferAttribute<float>;

            if (start != null && end != null)
            {

                var center = this.BoundingSphere.Center;

                this.BoundingBox.GetCenter(center);

                float maxRadiusSq = 0;

                for (int i = 0, il = start.count; i < il; i++)
                {

                    _vector.FromBufferAttribute(start, i);
                    maxRadiusSq = (float)Math.Max(maxRadiusSq, center.DistanceToSquared(_vector));

                    _vector.FromBufferAttribute(end, i);
                    maxRadiusSq = (float)Math.Max(maxRadiusSq, center.DistanceToSquared(_vector));

                }

                this.BoundingSphere.Radius = (float)Math.Sqrt(maxRadiusSq);

                if (this.BoundingSphere.Radius == float.NaN)
                {
                    Debug.WriteLine("THREE.LineSegmentsGeometry.computeBoundingSphere(): Computed radius is NaN. The instanced position data is likely to have NaN values.");
                    System.Environment.Exit(-1);
                }

            }

        }
    }
}

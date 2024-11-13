using System;
using System.Collections.Generic;
using System.Text;

namespace THREE
{
    [Serializable]
    public class Wireframe : Mesh
    {
        Vector3 _start = new Vector3();
        Vector3 _end = new Vector3();

        public Wireframe() : base()
        {           
            this.InitGeometry(null,null);
        }
        public override void InitGeometry(Geometry geometry, Material material)
        {
            this.type = "Wireframe";

            if (geometry == null)
                this.Geometry = new LineSegmentsGeometry();
            else
                this.Geometry = geometry;

            if (material == null)
            {
                this.Material = new LineMaterial() { Color = new Color().SetHex(0xffffff) };

            }
            else
            {
                Materials.Clear();
                this.Material = material;
            }

            this.Materials.Add(Material);

            this.UpdateMorphTargets();
        }
        public Wireframe(Geometry geometry=null,Material material=null) : base(geometry, material)
        {

        }
        public Wireframe ComputeLineDistances()
        {
            var geometry = this.Geometry as BufferGeometry;

            var instanceStart = geometry.Attributes["instanceStart"] as InterleavedBufferAttribute<float>;
            var instanceEnd = geometry.Attributes["instanceEnd"] as InterleavedBufferAttribute<float>;
            
            float[] lineDistances = new float[2 * instanceStart.count];

            for (int i = 0, j = 0, l = instanceStart.count; i < l; i++, j += 2)
            {

                _start.FromBufferAttribute(instanceStart, i);
                _end.FromBufferAttribute(instanceEnd, i);

                lineDistances[j] = (j == 0) ? 0 : lineDistances[j - 1];
                lineDistances[j + 1] = lineDistances[j] + _start.DistanceTo(_end);

            }

            var instanceDistanceBuffer = new InstancedInterleavedBuffer<float>(lineDistances, 2, 1); // d0, d1

            geometry.SetAttribute("instanceDistanceStart", new InterleavedBufferAttribute<float>(instanceDistanceBuffer, 1, 0)); // d0
            geometry.SetAttribute("instanceDistanceEnd", new InterleavedBufferAttribute<float>(instanceDistanceBuffer, 1, 1)); // d1

            return this;
        }
    }
}

using System;
using System.Collections.Generic;

namespace THREE
{
    public class Line : Object3D
    {
        public int LineStrip = 0;

        public int LinePieces = 1;

        public Vector3 Start = Vector3.Zero();

        public Vector3 End = Vector3.Zero();

        public int Mode;


        public Line()
        {

        }

        public Line(Geometry geometry = null, Material material = null, int? type = null) : base()
        {
            this.type = "Line";

            this.Geometry = geometry ?? new Geometry();
            this.Material = material ?? new LineBasicMaterial { Color = new Color().SetHex(0xFFFFFF) };

            this.Mode = (int)Constants.LineStrip;
            if (null != type) this.Mode = type.Value;
        }

        public Line(Geometry geometry = null, List<Material> materials = null, int? type = null) : base()
        {
            this.type = "Line";

            this.Geometry = geometry ?? new Geometry();

            if (materials == null)
            {
                this.Material = new MeshBasicMaterial() { Color = new Color().SetHex(0xffffff) };
                this.Materials.Add(Material);
            }
            else
            {
                this.Materials = materials;
                if (this.Materials.Count > 0)
                    this.Material = this.Materials[0];
            }

            this.Mode = (int)Constants.LineStrip;
            if (null != type) this.Mode = type.Value;
        }
        public void InitGeometry(Geometry geometry, Material material)
        {
            this.type = "Line";

            this.Geometry = geometry ?? new Geometry();

            if (material == null)
            {
                this.Material = new MeshBasicMaterial() { Color = new Color().SetHex(0xffffff) };
                this.Materials.Add(Material);
            }
            else
            {
                this.Material = material;
                this.Materials.Add(material);
            }

            this.Mode = (int)Constants.LineStrip;
        }
        public void InitGeometry(Geometry geometry, List<Material> materials)
        {
            this.type = "Line";

            this.Geometry = geometry ?? new Geometry();

            if (materials == null)
            {
                this.Material = new MeshBasicMaterial() { Color = new Color().SetHex(0xffffff) };
                this.Materials.Add(Material);
            }
            else
            {
                this.Materials = materials;
                if (this.Materials.Count > 0)
                    this.Material = this.Materials[0];
            }

            this.Mode = (int)Constants.LineStrip;
        }

        public virtual Line ComputeLineDistances()
        {
            var geometry = this.Geometry;

            var _start = new Vector3();
            var _end = new Vector3();
            if (geometry is BufferGeometry)
            {

                // we assume non-indexed geometry

                if ((geometry as BufferGeometry).Index == null)
                {

                    BufferAttribute<float> positionAttribute = (BufferAttribute<float>)((geometry as BufferGeometry).Attributes["position"]);
                    var lineDistances = new List<float>();
                    lineDistances.Add(0);

                    for (var i = 1;i< positionAttribute.count; i++)
                    {

                        _start.FromBufferAttribute(positionAttribute, i - 1);
                        _end.FromBufferAttribute(positionAttribute, i);

                        //lineDistances[i] = lineDistances[i - 1];

                        lineDistances.Add(lineDistances[i - 1]);

                        lineDistances[i] += _start.DistanceTo(_end);

                    }

                    (geometry as BufferGeometry).SetAttribute("lineDistance", new BufferAttribute<float>(lineDistances.ToArray(), 1));

                }
                else
                {

                    Console.WriteLine("THREE.Line.computeLineDistances(): Computation only possible with non-indexed BufferGeometry.");

                }

            }
            else if (geometry is Geometry)
            {

                var vertices = geometry.Vertices;
                var lineDistances = geometry.LineDistances;
                
                lineDistances.Clear();

                lineDistances.Add(0);

                for (var i = 1;i< vertices.Count;i++)
                {

                    lineDistances.Add(lineDistances[i - 1]);

                    lineDistances[i] += vertices[i - 1].DistanceTo(vertices[i]);

                }

            }
            return this;
        }

        public void RayCast()
        {
            throw new NotImplementedException();
        }
        
    }
}

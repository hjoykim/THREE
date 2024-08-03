using System.Collections.Generic;
using System.Runtime.Serialization;

namespace THREE
{
    [Serializable]
    public class InstancedMesh : Mesh
    {
        private Mesh _mesh = new Mesh();
        private Matrix4 _instanceLocalMatrix = new Matrix4();
        private Matrix4 _instanceWorldMatrix = new Matrix4();
        private List<Intersection> _instanceIntersects = new List<Intersection>();

        public BufferAttribute<float> InstanceMatrix;

        public BufferAttribute<float> InstanceColor;

        public int InstanceCount;
        public InstancedMesh()  : base() { }
        public InstancedMesh(Geometry geometry, Material material, int count) : base(geometry, material)
        {
            this.type = "InstancedMesh";
            this.InstanceMatrix = new BufferAttribute<float>(new float[count * 16], 16);
            this.InstanceColor = null;
            this.InstanceCount = count;
        }

        public InstancedMesh(SerializationInfo info, StreamingContext context) : base(info, context) { }
        public override object Clone()
        {
            return this.DeepCopy();
        }
        public InstancedMesh Copy(InstancedMesh mesh)
        {
            return mesh.DeepCopy();
        }
        public Color GetColorAt(int index, Color color)
        {
            return color.FromArray(InstanceColor.Array, index * 3);
        }

        public Matrix4 GetMatrixAt(int index, Matrix4 matrix)
        {
            return matrix.FromArray(InstanceMatrix.Array, index * 16);
        }

        public void SetColorAt(int index, Color color)
        {
            if (InstanceColor == null)
            {
                InstanceColor = new InstancedBufferAttribute<float>(new float[InstanceCount * 3], 3);
            }
            color.ToArray(InstanceColor.Array, index * 3);
        }

        public void SetMatrixAt(int index, Matrix4 matrix)
        {
            matrix.ToArray(InstanceMatrix.Array, index * 16);
        }

        public override void Raycast(Raycaster raycaster, List<Intersection> intersects)
        {

            Matrix4 matrixWorld = this.MatrixWorld;
            int raycastTimes = this.InstanceCount;

            _mesh.Geometry = this.Geometry;
            _mesh.Material = this.Material;

            if (_mesh.Material == null) return;

            for (int instanceId = 0; instanceId < raycastTimes; instanceId++)
            {

                // calculate the world matrix for each instance

                this.GetMatrixAt(instanceId, _instanceLocalMatrix);

                _instanceWorldMatrix.MultiplyMatrices(matrixWorld, _instanceLocalMatrix);

                // the mesh represents this single instance

                _mesh.MatrixWorld = _instanceWorldMatrix;

                _mesh.Raycast(raycaster, _instanceIntersects);

                // process the result of raycast

                for (int i = 0, l = _instanceIntersects.Count; i < l; i++)
                {

                    Intersection intersect = _instanceIntersects[i];
                    intersect.instanceId = instanceId;
                    intersect.object3D = this;
                    intersects.Add(intersect);

                }

                _instanceIntersects.Clear();

            }

        }
    }
}

using System.Collections.Generic;

namespace THREE
{
    public class InstancedMesh : Mesh
    {
        public BufferAttribute<float> InstanceMatrix;

        public BufferAttribute<float> InstanceColor;

        public int count;

        public InstancedMesh(Geometry geometry, List<Material> material, int count) : base(geometry,material)
        {
            this.type = "InstancedMesh";

            this.count = count;
        }

        public Color getColorAt(int index, Color color)
        {
            return color.FromArray(InstanceColor.Array, index * 3);
        }

        public Matrix4 getMatrixAt(int index, Matrix4 matrix)
        {
            return matrix.FromArray(InstanceMatrix.Array, index * 16);
        }

        public void setColorAt(int index, Color color)
        {
            if (InstanceColor == null)
            {
                InstanceColor = new BufferAttribute<float>(new float[count * 3], 3);
            }
            color.ToArray(InstanceColor.Array, index * 3);
        }

        public void setMatrixAt(int index, Matrix4 matrix)
        {
            matrix.ToArray(InstanceMatrix.Array, index * 16);
        }
    }
}

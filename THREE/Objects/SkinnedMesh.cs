using System.Collections.Generic;

namespace THREE
{
    public class SkinnedMesh : Mesh
    {
        public string BindMode;

        public Matrix4 BindMatrix;

        public Matrix4 BindMatrixInverse;

        public Skeleton Skeleton;

        public SkinnedMesh(Geometry geometry, List<Material> material, bool? useVertexTexture = null) : base(geometry,material)
        {
            this.type = "SkinnedMesh";

            this.BindMode = "attached";

            this.BindMatrix = Matrix4.Identity();

            this.BindMatrixInverse = Matrix4.Identity();
        }

        public void Bind(Skeleton skeleton, Matrix4 bindMatrix)
        {
            this.Skeleton = skeleton;
            
            if (bindMatrix == null)
            {
            
                this.UpdateMatrixWorld(true);
            
                this.Skeleton.CalculateInverses();
            
                bindMatrix = this.MatrixWorld;
            
            }
            
            this.BindMatrix.Copy(bindMatrix);
            this.BindMatrixInverse.GetInverse(bindMatrix);
        }

        public void Pose()
        {
            Skeleton.Pose();
        }

        public void NormalizeSkinWeights()
        {
            Vector4 vector = new Vector4();
            BufferAttribute<float> skinWeight = (this.Geometry as BufferGeometry).Attributes["skinWeight"] as BufferAttribute<float>;

            for (int i = 0; i < skinWeight.count; i++)
            {
                vector.X = skinWeight.getX(i);
                vector.Y = skinWeight.getY(i);
                vector.Z = skinWeight.getZ(i);
                vector.W = skinWeight.getW(i);
            
                float scale = 1f / vector.ManhattanLength();
            
                if (scale != float.PositiveInfinity)
                {
                    vector.MultiplyScalar(scale);
                }
                else
                {
                    vector.Set(1, 0, 0, 0); // do something reasonable
                }
            
                skinWeight.setXYZW(i, vector.X, vector.Y, vector.Z, vector.W);
            }
        }

        public override void UpdateMatrixWorld(bool force = false)
        {
            base.UpdateMatrixWorld(force);
            if (BindMode.Equals("attached"))
            {
                BindMatrixInverse.GetInverse(MatrixWorld);
            }
            else if (BindMode.Equals("detached"))
            {
                BindMatrixInverse.GetInverse(BindMatrix);
            }
            else
            {
                // Unrecognized BindMode
            }
        }

        public Vector4 BoneTransform(int index,Vector4 target)
        {
            Vector3 basePosition = new Vector3();
            Vector4 skinIndex = new Vector4();
            Vector4 skinWeight = new Vector4();
            Vector4 vector = new Vector4();
            Matrix4 matrix = new Matrix4();
            
            skinIndex.FromBufferAttribute((this.Geometry as BufferGeometry).Attributes["skinIndex"] as BufferAttribute<float>, index);
            skinWeight.FromBufferAttribute((this.Geometry as BufferGeometry).Attributes["skinWeight"] as BufferAttribute<float>, index);
            basePosition.FromBufferAttribute((this.Geometry as BufferGeometry).Attributes["position"] as BufferAttribute<float>, index).ApplyMatrix4(this.BindMatrix);


            Vector4 basePosition1 = new Vector4();
            for(int i = 0; i < 4; i++)
            {
                var weight = skinWeight.GetComponent(i);
                if(weight !=0)
                {
                    int boneIndex = (int)skinIndex.GetComponent(i);
                    matrix.MultiplyMatrices(Skeleton.Bones[boneIndex].MatrixWorld,Skeleton.BoneInverses[boneIndex]);
                    vector.Set(basePosition.X, basePosition.Y, basePosition.Z, 1);
                    target.AddScaledVector(vector.ApplyMatrix4(matrix), weight);
                }
            }

            return target.ApplyMatrix4(this.BindMatrixInverse);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using THREE.Core;
using THREE.Materials;
using THREE.Math;
namespace THREE.Objects
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
        }

        public void Pose()
        {

        }

        public void NormalizeSkinWeights()
        {
        }

        public void UpdateMatrixWorld(bool? force = false)
        {
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

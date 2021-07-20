using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using THREE.Core;
using THREE.Textures;
using THREE.Math;
namespace THREE.Objects
{
    public class Skeleton : Object3D
    {
        public bool UseVertexTexture;

        public Matrix4 IdentityMatrix;

        public Bone[] Bones;

        public int BoneTextureWidth;

        public int BoneTextureHeight;

        public float[] BoneMatrices;

        public DataTexture BoneTexture;

        public int BoneTextureSize;

        public Matrix4[] BoneInverses;

        public int Frame = -1;

        public Skeleton(Bone[] bones, Matrix4[] boneInverses = null)
        {
        }

        public void CalculateInverses(Bone bone)
        {
        }

        public void Pose()
        {
        }

        public void Update()
        {
        }


    }
}

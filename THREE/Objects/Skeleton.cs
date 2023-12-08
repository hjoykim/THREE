using System;

namespace THREE
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
            this.Bones = bones;
            BoneMatrices = new float[bones.Length * 16];
            if (boneInverses != null)
            {
                CalculateInverses();
            }
            else
            {
                //if (this.Bones.Length == boneInverses.Length)
                //{
                //    Array.Copy(boneInverses, 0, this.BoneInverses, 0, boneInverses.Length);
                //}
                //else
                //{
                    this.BoneInverses = new Matrix4[this.Bones.Length];
                    int bCount = 0;
                    for (int i = 0; i < this.Bones.Length; i++)
                    {
                        this.BoneInverses[bCount++] = new Matrix4();
                    }
                //}
            }
        }

        public void CalculateInverses()
        {
            this.BoneInverses = new Matrix4[Bones.Length];
            int bCount = 0;
            for (int i = 0; i < Bones.Length; i++)
            {
                Matrix4 inverse = new Matrix4();
                if (Bones[i] != null)
                {
                    inverse.GetInverse(Bones[i].MatrixWorld);
                }
                BoneInverses[bCount++] = inverse;
            }
        }

        public void Pose()
        {
            Bone bone;
            for (int i = 0; i < Bones.Length; i++)
            {
                bone = Bones[i];
                if (bone != null)
                {
                    bone.MatrixWorld.GetInverse(BoneInverses[i]);
                }
            }
            for (int i = 0; i < Bones.Length; i++)
            {
                bone = Bones[i];
                if (bone != null)
                {
                    if (bone.Parent != null && bone.Parent is Bone)
                    {
                        // Unsure is Bone.Matrix is the Right variable
                        bone.Matrix.GetInverse(bone.Parent.MatrixWorld);
                        bone.Matrix.Multiply(bone.MatrixWorld);
                    }
                    else
                    {
                        // Unsure is Bone.Matrix is the Right variable
                        bone.Matrix.Copy(bone.MatrixWorld);
                    }
                }

                // Unsure is Bone.Matrix is the Right variable
                bone.Matrix.Decompose(bone.Position, bone.Quaternion, bone.Scale);
            }
        }

        public void Update()
        {
            Matrix4 offsetMatrix = new Matrix4();
            Matrix4 identityMatrix = new Matrix4();

            for (int i = 0; i < Bones.Length; i++)
            {
                Matrix4 matrix = Bones[i] != null ? Bones[i].MatrixWorld : identityMatrix;
                offsetMatrix.MultiplyMatrices(matrix, BoneInverses[i]);
                offsetMatrix.ToArray(BoneMatrices, i * 16);
            }
            if (BoneTexture != null)
            {
                BoneTexture.NeedsUpdate = true;
            }
        }


        public Skeleton clone()
        {
            return new Skeleton(Bones, BoneInverses);
        }

        public Bone getBoneByName(String name)
        {
            for (int i = 0; i < Bones.Length; i++)
            {
                Bone bone = Bones[i];
                if (bone.Name.Equals(name))
                {
                    return bone;
                }
            }
            return null;
        }

    }
}

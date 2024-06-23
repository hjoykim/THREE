using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace THREE
{
    [Serializable]
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

        public Skeleton(SerializationInfo info, StreamingContext context) : base(info, context) { }

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


        public Skeleton Clone()
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
        private byte[] ToByteArray(float[] floatArray)
        {
            byte[] byteArray = new byte[floatArray.Length];
            for (int i = 0; i < floatArray.Length; i++)
                byteArray[i] = (byte)(floatArray[i] * 255.0f);

            return byteArray;
        }
        public Skeleton ComputeBoneTexture()
        {

            // layout (1 matrix = 4 pixels)
            //      RGBA RGBA RGBA RGBA (=> column1, column2, column3, column4)
            //  with  8x8  pixel texture max   16 bones * 4 pixels =  (8 * 8)
            //       16x16 pixel texture max   64 bones * 4 pixels = (16 * 16)
            //       32x32 pixel texture max  256 bones * 4 pixels = (32 * 32)
            //       64x64 pixel texture max 1024 bones * 4 pixels = (64 * 64)

            int size = (int)Math.Sqrt(this.Bones.Length * 4); // 4 pixels needed for 1 matrix
            size = (int)Math.Ceiling((decimal)size / 4) * 4;
            size = Math.Max(size, 4);

            float[] boneMatrices = new float[size * size * 4]; // 4 floats per RGBA pixel
            Array.Copy(this.BoneMatrices, boneMatrices, this.BoneMatrices.Length); // copy current values

            Bitmap im = new Bitmap(size, size,size,System.Drawing.Imaging.PixelFormat.Format8bppIndexed,Marshal.UnsafeAddrOfPinnedArrayElement(ToByteArray(boneMatrices),0));
            DataTexture boneTexture = new DataTexture(im, size, size, Constants.RGBAFormat, Constants.FloatType);
            boneTexture.NeedsUpdate = true;

            this.BoneMatrices = boneMatrices;
            this.BoneTexture = boneTexture;

            return this;

        }
    }
}

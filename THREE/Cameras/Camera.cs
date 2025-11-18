using System;
using System.Collections;
using System.Runtime.Serialization;

namespace THREE
{
    [Serializable]
    public struct View
    {
        public bool Enabled;

        public float FullWidth;

        public float FullHeight;

        public float OffsetX;

        public float OffsetY;

        public float Width;

        public float Height;
    }
    [Serializable]
    public class Camera : Object3D
    {
        public View View;

        public Matrix4 MatrixWorldInverse = Matrix4.Identity();

        public Matrix4 ProjectionMatrix = Matrix4.Identity();

        public Matrix4 ProjectionMatrixInverse = Matrix4.Identity();

        public float Fov;
        
        public float Aspect = 1.0f;
        
        public float Far = 2000.0f;
        
        public float Near = 0.1f;

        public float Bottom;

        public float Left;

        public float Top;

        public float CameraRight;

        public Vector4 Viewport;

        public Camera()
        {
            View = new View()
            {
                Enabled = false,
                FullWidth = 1,
                FullHeight = 1,
                OffsetX = 0,
                OffsetY = 0,
                Width = 1,
                Height = 1
            };
        }
        public Camera(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }


        public override Vector3 GetWorldDirection(Vector3 target)
        {
            if (target == null)
            {
                target = new Vector3();
            }

            this.UpdateWorldMatrix(true, false);

            var e = this.MatrixWorld.Elements;

            return target.Set(-e[8], -e[9], -e[10]).Normalize();
        }

        public override void UpdateMatrixWorld(bool force = false)
        {
            base.UpdateMatrixWorld(force);

            this.MatrixWorldInverse.Copy(this.MatrixWorld).Invert();

        }
        public override void UpdateWorldMatrix(bool updateParents, bool updateChildren)
        {
            base.UpdateWorldMatrix(updateParents, updateChildren);

            MatrixWorldInverse.Copy(this.MatrixWorld).Invert();
        }

        public override object Clone()
        {
           return FastDeepCloner.DeepCloner.Clone(this);
        }

        public virtual void UpdateProjectionMatrix()
        {
            //this.MatrixWorldInverse.GetInverse(this.MatrixWorld);
            this.UpdateWorldMatrix(false, true);
        }

    }
}

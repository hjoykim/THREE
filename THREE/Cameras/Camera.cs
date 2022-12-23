using System;
using System.Collections;

namespace THREE
{
    public struct View
    {
        public Boolean Enabled;

        public float FullWidth;

        public float FullHeight;

        public float OffsetX;

        public float OffsetY;

        public float Width;

        public float Height;
    }
    public class Camera : Object3D
    {
        public View View;

        public Matrix4 MatrixWorldInverse = Matrix4.Identity();

        public Matrix4 ProjectionMatrixInverse = Matrix4.Identity();
       
        public Matrix4 ProjectionMatrix = Matrix4.Identity();

        public float Fov;
        public float Aspect = 1.0f;
        public float Far = 2000.0f;
        public float Near = 0.1f;

        public bool NeedsUpdate = false;

        public float X = -1;

        public float Y = -1;

        public float FullWidth = -1;

        public float FullHeight = -1;

        public float Width = -1;

        public float Height = -1;

        public float Zoom = 1;

        public float Bottom;

        public float Left;

        public float Top;

        public float CameraRight;

        public Vector4 Viewport;

        public Camera()
        {
            this.IsCamera = true;
            this.type = "Camera";

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
        protected Camera(Camera source, bool recursive = true) : base(source,recursive)
        {
            this.IsCamera = true;
            this.type = "Camera";

            MatrixWorldInverse.Copy(source.MatrixWorldInverse);

            ProjectionMatrix.Copy(source.ProjectionMatrix);

            ProjectionMatrixInverse.Copy(source.ProjectionMatrixInverse);

            this.View = source.View;
        }

        public override Vector3 GetWorldDirection(Vector3 target)
        {
            this.UpdateMatrixWorld(true);

            var e = this.MatrixWorld.Elements;

            return target.Set(-e[8], -e[9], -e[10]).Normalize();
        }
        
        public override void UpdateMatrixWorld(bool force = false)
        {
            base.UpdateMatrixWorld(force);

            this.MatrixWorldInverse.GetInverse(this.MatrixWorld);
        }
        public override void UpdateWorldMatrix(bool updateParents,bool updateChildren)
        {
            base.UpdateWorldMatrix(updateParents, updateChildren);

            MatrixWorldInverse.GetInverse(MatrixWorld);
        }

        public void SetViewOffset(int fullWidth, int fullHeight, int x, int y, int width, int height)
        {
            View.Enabled = true;
            View.FullWidth = fullWidth;
            View.FullHeight = fullHeight;
            View.OffsetX = x;
            View.OffsetY = y;
            View.Width = width;
            View.Height = height;

            this.UpdateProjectionMatrix();
        }
        public void ClearViewOffset()
        {
            this.View.Enabled = false;

            this.UpdateProjectionMatrix();
        }

        public virtual void UpdateProjectionMatrix()
        {
            //this.MatrixWorldInverse.GetInverse(this.MatrixWorld);
            this.UpdateWorldMatrix(false, true);
        }

        public override object Clone()
        {

            Object3D object3D = base.Clone() as Object3D;           
           
            Camera cloned = new Camera(this);

            foreach (DictionaryEntry item in object3D)
            {
                cloned.Add(item.Key, item.Value);
            }

            return cloned;
        }

    }
}

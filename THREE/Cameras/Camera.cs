using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using THREE.Core;
using THREE.Math;
namespace THREE.Cameras
{
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
    public class Camera : Object3D
    {
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

        public Camera()
        {
            this.IsCamera = true;
            this.type = "Camera";
        }
        protected Camera(Camera source, bool recursive = true) : base(source,recursive)
        {
            this.IsCamera = true;
            this.type = "Camera";

            MatrixWorldInverse.Copy(source.MatrixWorldInverse);

            ProjectionMatrix.Copy(source.ProjectionMatrix);

            ProjectionMatrixInverse.Copy(source.ProjectionMatrixInverse);
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

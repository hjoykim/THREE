using FastDeepCloner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace THREE
{
    [Serializable]
    public class OrthographicCamera : Camera
    {
        public float Zoom = 1;

        public float Left=-1;

        public float CameraRight=1; // 'Right' is a reserved word in C#

        public float Top=1;

        public float Bottom=-1;

        public float Near=0.1f;

        public float Far=2000.0f;

        public OrthographicCamera()
        {            
        }
        public OrthographicCamera(float left = -1f, float right = 1f, float bottom = -1f, float top = 1f, float near = 0.1f, float far = 2000f)
        {
            this.Zoom = 1f;

            this.Left = left;
            this.CameraRight = right;
            this.Top = top;
            this.Bottom = bottom;
            this.Near = near;
            this.Far = far;

            this.UpdateProjectionMatrix();
        }
        public OrthographicCamera(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
   

        /// <summary>
        /// Sets the view offset for multi-view / multi-viewport rendering.
        /// Translated from JS: setViewOffset(fullWidth, fullHeight, x, y, width, height)
        /// </summary>
        public void SetViewOffset(float fullWidth, float fullHeight, float x, float y, float width, float height)
        {
            this.View.Enabled = true;
            this.View.FullWidth = fullWidth;
            this.View.FullHeight = fullHeight;
            this.View.OffsetX = x;
            this.View.OffsetY = y;
            this.View.Width = width;
            this.View.Height = height;

            this.UpdateProjectionMatrix();
        }
        public override object Copy(Object3D source, bool recursive = true)
        {
            base.Copy(source, recursive);
            var otherCamera = source as OrthographicCamera;
            this.Zoom = otherCamera.Zoom;
            this.Left = otherCamera.Left;
            this.CameraRight = otherCamera.CameraRight;
            this.Top = otherCamera.Top;
            this.Bottom = otherCamera.Bottom;
            this.Near = otherCamera.Near;
            this.Far = otherCamera.Far;
            this.View = otherCamera.View.DeepCopy();
            this.UpdateProjectionMatrix();

            return this;     

        }
        /// <summary>
        /// </summary>
        public void UpdateProjectionMatrix()
        {
            var dx = (this.CameraRight - this.Left) / (2 * this.Zoom);
            var dy = (this.Top - this.Bottom) / (2 * this.Zoom);
            var cx = (this.CameraRight + this.Left) /2;
            var cy = (this.Top + this.Bottom) /2;

            var left = cx - dx;
            var right = cx + dx;
            var top = cy + dy;
            var bottom = cy - dy;

            if (this.View.Enabled)
            {
                var view = this.View;

                var scaleW = (this.CameraRight - this.Left) / view.FullWidth / this.Zoom;
                var scaleH = (this.Top - this.Bottom) / view.FullHeight / this.Zoom;

                left += scaleW * view.OffsetX;
                right = left + scaleW * view.Width;
                top -= scaleH * view.OffsetY;
                bottom = top - scaleH * view.Height;
            }

            this.ProjectionMatrix = Matrix4.Identity().MakeOrthographic(left, right, top, bottom, this.Near, this.Far);

            this.ProjectionMatrixInverse.Copy(ProjectionMatrix).Invert();
        }
    }
}

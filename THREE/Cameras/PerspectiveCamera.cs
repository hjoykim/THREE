using THREE.Math;

namespace THREE.Cameras
{
    using System;
    public class PerspectiveCamera : Camera
    {
        public PerspectiveCamera(float fov=50,float aspect=1,float near=0.1f,float far = 2000)
        {
            this.Fov = fov;
            this.Aspect = aspect;
            this.Near = near;
            this.Far = far;

            this.UpdateProjectionMatrix();
        }

        protected PerspectiveCamera(PerspectiveCamera other) : base(other)
        {
            this.Fov = other.Fov;
            this.Aspect = other.Aspect;
            this.Near = other.Near;
            this.Far = other.Far;
            this.UpdateProjectionMatrix();
        }

        public override void UpdateProjectionMatrix()
        {
            base.UpdateProjectionMatrix();

            float near = this.Near,
            top = near * (float)Math.Tan(TMath.DEG2RAD * 0.5 * this.Fov) / this.Zoom,
            height = 2 * top,
            width = this.Aspect * height,
            left = -0.5f * width;

            if (this.View.Enabled)
            {
                left += (float)View.OffsetX * width / (float)View.FullWidth;
                top -= (float)View.OffsetY * height / (float)View.FullHeight;
                width *= (float)View.Width / (float)View.FullWidth;
                height *= (float)View.Height / (float)View.FullHeight;
            }

            this.ProjectionMatrix = this.ProjectionMatrix.MakePerspective(left, left + width, top, top - height, near, this.Far);
		    this.ProjectionMatrixInverse.GetInverse(this.ProjectionMatrix);

        }

        public override object Clone()
        {
            return new PerspectiveCamera(this);
        }
    }
}

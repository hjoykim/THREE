using System;
using System.Runtime.Serialization;

namespace THREE
{
    [Serializable]
    public class PerspectiveCamera : Camera
    {  
        public float Fov=50; // vertical field of view in degrees

        public float Zoom = 1;      
        
        public float Near=0.1f;
        
        public float Far=2000;
        
        public float focus=10;

        public float Aspect=1;

        public int FilmGauge=35;

        public int FilmOffset=0;

        public PerspectiveCamera() { }
        public PerspectiveCamera(float fov = 50, float aspect = 1, float near = 0.1f, float far = 2000)
        {
            this.Fov = fov;
            this.Aspect = aspect;
            this.Near = near;
            this.Far = far;
            this.focus = 10;

            this.Aspect = aspect;

            this.FilmGauge = 35; // width of the film (default in millimeters)
            this.FilmOffset = 0;

            this.UpdateProjectionMatrix();
        }

        public PerspectiveCamera(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public override object Copy(Object3D source, bool recursive = true)
        {
            base.Copy(source, recursive);
            var otherCamera = source as PerspectiveCamera;

            this.Fov = otherCamera.Fov;
            this.Zoom = otherCamera.Zoom;
            this.Near = otherCamera.Near;
            this.Far = otherCamera.Far;
            this.focus = otherCamera.focus;
            this.Aspect = otherCamera.Aspect;
            this.FilmGauge = otherCamera.FilmGauge;
            this.FilmOffset = otherCamera.FilmOffset;
            this.View = otherCamera.View.DeepCopy();
            this.UpdateProjectionMatrix();
            return this;
        }

        public void SetFocalLength(float focalLength)
        {
            float vExtentSlope = 0.5f * this.GetFilmHeight() / focalLength;
            this.Fov = MathUtils.RAD2DEG * 2 * (float)System.Math.Atan(vExtentSlope);

            this.UpdateProjectionMatrix();
        }

        public float GetFocalLength()
        {
            float vExtentSlope = (float)System.Math.Tan(MathUtils.DEG2RAD * 0.5f * this.Fov);

            return 0.5f * this.GetFilmHeight() / vExtentSlope;
        }

        public float GetEffectiveFOV()
        {
            return MathUtils.RAD2DEG * 2 * (float)Math.Atan(Math.Tan(MathUtils.DEG2RAD * 0.5 * this.Fov) / this.Zoom);
        }
        public float GetFilmWidth()
        {
            return this.FilmGauge * (float)Math.Min(this.Aspect, 1);
        }

        public float GetFilmHeight()
        {
            return this.FilmGauge / (float)Math.Max(this.Aspect, 1);
        }

        public void SetViewOffset(float fullWidth, float fullHeight, float x, float y, float width, float height)
        {
            this.Aspect = fullWidth / (1.0f * fullHeight);

            this.View.Enabled = true;
            this.View.FullWidth = fullWidth;
            this.View.FullHeight = fullHeight;
            this.View.OffsetX = x;
            this.View.OffsetY = y;
            this.View.Width = width;
            this.View.Height = height;

            this.UpdateProjectionMatrix();

        }
        
        public void ClearViewOffset()
        {
            this.View.Enabled = false;
            this.UpdateProjectionMatrix();
        }
        public void UpdateProjectionMatrix()
        {
            //base.UpdateProjectionMatrix();

            float near = this.Near,
            top = near * (float)Math.Tan(MathUtils.DEG2RAD *0.5 * this.Fov) / this.Zoom,

            height =2 * top,
            width = this.Aspect * height,
            left = -0.5f * width;

            if (this.View.Enabled)

            {
                var view = this.View;

                left += view.OffsetX * width / view.FullWidth;
                top -= view.OffsetY * height / view.FullHeight;
                width *= view.Width / view.FullWidth;
                height *= view.Height / view.FullHeight;
            }

            var skew = this.FilmOffset;
            if (skew !=0) left += near * skew / this.GetFilmWidth();

            this.ProjectionMatrix = this.ProjectionMatrix.MakePerspective(left, left + width, top, top - height, near, this.Far);

            this.ProjectionMatrixInverse.Copy(ProjectionMatrix).Invert();

        }
    }
}

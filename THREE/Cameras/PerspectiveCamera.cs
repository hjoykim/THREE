namespace THREE
{
    using System;
    public class PerspectiveCamera : Camera
    {
        public int FilmGauge = 35;

        public int FilmOffset = 0;

        public float focus = 10.0f;


        public PerspectiveCamera(float fov=50,float aspect=1,float near=0.1f,float far = 2000)
        {
            this.Fov = fov;
            this.Aspect = aspect;
            this.Near = near;
            this.Far = far;

            this.UpdateProjectionMatrix();

            //View.Enabled = true;
            //View.FullWidth = 1;
            //View.FullHeight = 1;
            //View.OffsetX = 0;
            //View.OffsetY = 0;
            //View.Width = 1;
            //View.Height = 1;
        }

        protected PerspectiveCamera(PerspectiveCamera other) : base(other)
        {
            this.Fov = other.Fov;
            this.Aspect = other.Aspect;
            this.Near = other.Near;
            this.Far = other.Far;
            this.focus = other.focus;
            this.Zoom = other.Zoom;
            this.FilmGauge = other.FilmGauge;
            this.FilmOffset = other.FilmOffset;
            this.View = other.View;
            //this.UpdateProjectionMatrix();
        }

        public override void UpdateProjectionMatrix()
        {
            //base.UpdateProjectionMatrix();

            float near = this.Near,
            top = near * (float)Math.Tan(MathUtils.DEG2RAD * 0.5 * this.Fov) / this.Zoom,

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
            
            var skew = this.FilmOffset;
            if (skew != 0) left += near * skew / this.GetFilmWidth();

            this.ProjectionMatrix = this.ProjectionMatrix.MakePerspective(left, left + width, top, top - height, near, this.Far);
            
		    this.ProjectionMatrixInverse.GetInverse(this.ProjectionMatrix);

        }

        public void SetViewOffset(float fullWidth,float fullHeight,float x,float y,float width,float height)
        {
            this.Aspect = fullWidth / (1.0f * fullHeight);
            View.Enabled = true;
            View.FullWidth = fullWidth;
            View.FullHeight = fullHeight;
            View.OffsetX = x;
            View.OffsetY = y;
            View.Width = width;
            View.Height = height;

            this.UpdateProjectionMatrix();

        }

        public override object Clone()
        {
            return new PerspectiveCamera(this);
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
    }
}

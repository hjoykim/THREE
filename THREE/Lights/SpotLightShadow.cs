namespace THREE
{
    public class SpotLightShadow : LightShadow
    {
        public SpotLightShadow()
            : base(new PerspectiveCamera(50, 1, 0.5f, 500))
        {
        }

        public override void UpdateMatrices(Light light)
        {
            var fov = 180 / System.Math.PI * 2 * light.Angle;
            var aspect = this.MapSize.X / this.MapSize.Y;
            var far = light.Distance!=0? light.Distance:Camera.Far;

            if (fov != Camera.Fov || aspect != Camera.Aspect || far != Camera.Far)
            {
                Camera.Fov = (float)fov;
                Camera.Aspect = aspect;
                Camera.Far = far;
                Camera.UpdateProjectionMatrix();
            }
            base.UpdateMatrices(light);
        }
    }
}

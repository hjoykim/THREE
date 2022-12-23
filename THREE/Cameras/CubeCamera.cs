
namespace THREE
{
    public class CubeCamera : Camera
    {
        public GLRenderTarget RenderTarget;
		PerspectiveCamera cameraPX, cameraNX, cameraPY, cameraNY,cameraPZ,cameraNZ;
     
        public CubeCamera(float near,float far,GLRenderTarget renderTarget) : base()
        {
			this.Fov = 90.0f;
			this.Aspect = 1.0f;
            this.RenderTarget = renderTarget;

            cameraPX = new PerspectiveCamera(Fov, Aspect, near, far);
            cameraPX.Layers = this.Layers;
            cameraPX.Up.Set(0, -1, 0);
            cameraPX.LookAt(new Vector3(1, 0, 0));
            Add(cameraPX);

			cameraNX = new PerspectiveCamera(Fov, Aspect, near, far);
			cameraNX.Layers = this.Layers;
			cameraNX.Up.Set(0, -1, 0);
			cameraNX.LookAt(new Vector3(-1, 0, 0));
			Add(cameraNX);

			cameraPY = new PerspectiveCamera(Fov, Aspect, near, far);
			cameraPY.Layers = this.Layers;
			cameraPY.Up.Set(0, 0, 1);
			cameraPY.LookAt(new Vector3(0, 1, 0));
			Add(cameraPY);

			cameraNY = new PerspectiveCamera(Fov, Aspect, near, far);
			cameraNY.Layers = this.Layers;
			cameraNY.Up.Set(0, 0, -1);
			cameraNY.LookAt(new Vector3(0, -1, 0));
			Add(cameraNY);

			cameraPZ = new PerspectiveCamera(Fov, Aspect, near, far);
			cameraPZ.Layers = this.Layers;
			cameraPZ.Up.Set(0, -1, 0);
			cameraPZ.LookAt(new Vector3(0, 0, 1));
			Add(cameraPZ);

			cameraNZ = new PerspectiveCamera(Fov, Aspect, near, far);
			cameraNZ.Layers = this.Layers;
			cameraNZ.Up.Set(0, -1, 0);
			cameraNZ.LookAt(new Vector3(0, 0, -1));
			Add(cameraNZ);
		}

		public void Update(GLRenderer renderer,Scene scene)
        {
			if (this.Parent == null) this.UpdateMatrixWorld();

			//var currentXrEnabled = renderer.xr.enabled;
			var currentRenderTarget = renderer.GetRenderTarget();

			//renderer.xr.enabled = false;

			var generateMipmaps = RenderTarget.Texture.GenerateMipmaps;

			RenderTarget.Texture.GenerateMipmaps = false;

			renderer.SetRenderTarget(RenderTarget, 0);
			renderer.Render(scene, cameraPX);

			renderer.SetRenderTarget(RenderTarget, 1);
			renderer.Render(scene, cameraNX);

			renderer.SetRenderTarget(RenderTarget, 2);
			renderer.Render(scene, cameraPY);

			renderer.SetRenderTarget(RenderTarget, 3);
			renderer.Render(scene, cameraNY);

			renderer.SetRenderTarget(RenderTarget, 4);
			renderer.Render(scene, cameraPZ);

			RenderTarget.Texture.GenerateMipmaps = generateMipmaps;

			renderer.SetRenderTarget(RenderTarget, 5);
			renderer.Render(scene, cameraNZ);

			renderer.SetRenderTarget(currentRenderTarget);

			//renderer.xr.enabled = currentXrEnabled;

			
        }
	}
}

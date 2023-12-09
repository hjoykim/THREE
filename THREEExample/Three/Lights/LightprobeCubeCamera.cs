
using System.Collections.Generic;
using THREE;

namespace THREEExample.Three.Lights
{
    [Example("cubecamera", ExampleCategory.ThreeJs, "lightprobe")]
    public class LightprobeCubeCamera : ExampleTemplate
    {
        LightProbe lightProbe;
        CubeCamera cubeCamera;
        public override void InitRenderer()
        {
            renderer.outputEncoding = Constants.sRGBEncoding;
        }
        public override void InitCamera()
        {
            base.InitCamera();
            camera.Fov = 40;
            camera.Near = 1;
            camera.Far = 1000;
            camera.Position.Set(0, 0, 30);
        }
    
        public override void Init()
        {
            base.Init();

            var cubeRenderTarget = new GLCubeRenderTarget(256) {
                    Encoding = Constants.sRGBEncoding, // since gamma is applied during rendering, the cubeCamera renderTarget texture encoding must be sRGBEncoding
					Format = Constants.RGBAFormat

                };

            cubeCamera = new CubeCamera(1, 1000, cubeRenderTarget);

            lightProbe = new LightProbe();
            scene.Add(lightProbe);

            var urls = GenCubeUrls("../../../../assets/textures/cube/pisa/", ".png");

            var cubeTexture = CubeTextureLoader.Load(urls);

            cubeTexture.Encoding = Constants.sRGBEncoding;

            scene.Background = cubeTexture;

            cubeCamera.Update(renderer, scene);

            lightProbe.Copy(LightProbeGenerator.FromCubeRenderTarget(renderer, cubeRenderTarget));

            scene.Add(new LightProbeHelper(lightProbe, 5));
        }
        private List<string> GenCubeUrls(string prefix, string postfix)
        {

            return new List<string>(){
                prefix + "px" + postfix, prefix + "nx" + postfix,
                prefix + "py" + postfix, prefix + "ny" + postfix,
                prefix + "pz" + postfix, prefix + "nz" + postfix
            };

        }
    }
}

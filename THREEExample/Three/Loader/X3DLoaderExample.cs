using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using THREE;

namespace THREEExample.Three.Loader
{
    [Example("loader_x3d", ExampleCategory.ThreeJs, "loader")]
    public class X3DLoaderExample : ExampleTemplate
    {
        Vector3 cameraTarget;
        public X3DLoaderExample():base() 
        {

        }
        public override void InitRenderer()
        {
            base.InitRenderer();
            renderer.SetClearColor(0x000000);
            renderer.outputEncoding = Constants.sRGBEncoding;
        }
        public override void InitCamera()
        {
            base.InitCamera();
            camera.Fov = 35;
            camera.Near = 1;
            camera.Far = 31000;
            camera.Position.Set(3, 1150, 1100);

        }

        public override void InitLighting()
        {
            scene.Add(new AmbientLight(0x888888));

            var keyLight = new SpotLight(Color.Hex(0xffffff));
            keyLight.Position.Set(00, 80, 80);
            keyLight.Intensity = 2;
            keyLight.LookAt(new Vector3(0, 15, 0));
            keyLight.CastShadow = true;
            keyLight.Shadow.MapSize.Height = 4096;
            keyLight.Shadow.MapSize.Width = 4096;

            scene.Add(keyLight);

            AddShadowedLight(1, 1, 1, 0xffffff, 1.35f);
            AddShadowedLight(0.5f, 1, -1, 0x888888, 1);

        }
        private void AddShadowedLight(float x, float y, float z, int color, float intensity)
        {
            var directionalLight = new DirectionalLight(color, intensity);
            directionalLight.Position.Set(x, y, z);
            scene.Add(directionalLight);
           
        }
        public override void Init()
        {
            base.Init();

            LoadObject();
        }
        private void LoadObject()
        {
            X3DLoader loader = new X3DLoader();
            loader.Load("../../../assets/models/x3d/as1_pe_203.x3d");
            var rootObject = loader.GetX3DObject();
            scene.Add(rootObject);
        }
    }
}

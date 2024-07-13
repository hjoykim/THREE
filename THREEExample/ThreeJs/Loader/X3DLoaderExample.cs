using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using THREE;
using THREEExample.Learning.Utils;
using Color = THREE.Color;
namespace THREEExample.Three.Loader
{
    [Example("loader_x3d", ExampleCategory.ThreeJs, "loader")]
    public class X3DLoaderExample : Example
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
            camera.Position.Set(13000, 11500, 11000);

        }

        public override void InitLighting()
        {
            scene.Add(new AmbientLight(0x353535));

            var spotLight = new THREE.SpotLight(0xffffff);
            spotLight.Position.Set(-10, 3320, -50);
            spotLight.CastShadow = true;
            scene.Add(spotLight);

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
            var plane = DemoUtils.AddLargeGroundPlane(scene);
            plane.Material.Color = Color.Hex(0xcccccc);
            plane.Position.Y = -1000;

            LoadObject();
        }
        private void LoadObject()
        {
            X3DLoader loader = new X3DLoader();
            loader.Load("../../../../assets/models/x3d/as1_pe_203.x3d");
            var rootObject = loader.GetX3DObject();
            scene.Add(rootObject);
        }
    }
}

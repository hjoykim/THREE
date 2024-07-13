using OpenTK.Windowing.Common;
using System;
using THREE;
using THREEExample.Learning.Utils;

namespace THREEExample.Learning.Chapter09
{
    [Example("03-Trackball-controls-camera", ExampleCategory.LearnThreeJS, "Chapter09")]
    public class TrackballControlsCameraExample : Example
    {
        Mesh mesh;

        public TrackballControlsCameraExample() : base()
        {
        }
        public override void InitRenderer()
        {
            base.InitRenderer();
            this.renderer.SetClearColor(THREE.Color.Hex(0x000000));
            this.renderer.ShadowMap.Enabled = true;
            this.renderer.ShadowMap.type = Constants.PCFSoftShadowMap;
        }

        public override void Init()
        {
            base.Init();
            DemoUtils.InitDefaultLighting(scene);

            OBJLoader loader = new OBJLoader();

            var city = loader.Load(@"../../../../assets/models/city/city.obj");
            DemoUtils.SetRandomColors(city);
            scene.Add(city);
        }
    }
}

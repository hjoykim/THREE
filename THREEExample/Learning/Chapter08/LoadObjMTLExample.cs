using OpenTK;
using System;
using System.Collections.Generic;
using THREE;
using Vector3 = THREE.Vector3;

namespace THREEExample.Learning.Chapter08
{
    [Example("07-Load-obj-MTL", ExampleCategory.LearnThreeJS, "Chapter08")]
    public class LoadObjMTLExample : ExampleTemplate
    {      

        public LoadObjMTLExample() : base() 
        { 
        }
        public override void InitRenderer()
        {
            base.InitRenderer();
            this.renderer.SetClearColor(new THREE.Color().SetHex(0x000000));
        }
        public override void InitCamera()
        {
            base.InitCamera();
            camera.LookAt(new Vector3(0, 15, 0));
        }
        private void buildScene()
        {
            OBJLoader loader = new OBJLoader();

            var mesh = loader.Parse(@"../../../../assets/models/butterfly/butterfly.obj");

            List<int> wingsposition = new List<int> { 0, 2, 4, 6 };

            wingsposition.ForEach(i =>
            {
                mesh.Children[i].Rotation.Z = 0.3f * (float)Math.PI;
            });

            List<int> wingsposition1 = new List<int> { 1, 3, 5, 7 };

            wingsposition1.ForEach(i =>
            {
                mesh.Children[i].Rotation.Z = -0.3f * (float)Math.PI;
            });

            //configure the wings

            var wing2 = mesh.Children[5];
            var wing1 = mesh.Children[4];

            wing1.Material.Opacity = 0.9f;
            wing1.Material.Transparent = true;
            wing1.Material.DepthTest = false;
            wing1.Material.Side = Constants.DoubleSide;

            wing2.Material.Opacity = 0.9f;
            wing2.Material.Transparent = true;
            wing2.Material.DepthTest = false;
            wing2.Material.Side = Constants.DoubleSide;

            mesh.Scale.Set(140, 140, 140);

            mesh.Rotation.X = 0.2f;
            mesh.Rotation.Y = -1.3f;

            scene.Add(mesh);

            var keyLight = new SpotLight(Color.Hex(0xffffff));
            keyLight.Position.Set(00, 80, 80);
            keyLight.Intensity = 2;
            keyLight.LookAt(new Vector3(0, 15, 0));
            keyLight.CastShadow = true;
            keyLight.Shadow.MapSize.Height = 4096;
            keyLight.Shadow.MapSize.Width = 4096;

            scene.Add(keyLight);

            var backlight1 = new SpotLight(Color.Hex(0xaaaaaa));
            backlight1.Position.Set(150, 40, -20);
            backlight1.Intensity = 0.5f;
            backlight1.LookAt(new Vector3(0, 15, 0));

            scene.Add(backlight1);

            var backlight2 = new SpotLight(Color.Hex(0xaaaaaa));
            backlight2.Position.Set(-150, 40, -20);
            backlight2.Intensity = 0.5f;
            backlight2.LookAt(new Vector3(0, 15, 0));
            scene.Add(backlight2);
        }
        public override void Load(GLControl control)
        {
            base.Load(control);

            buildScene();
        }       
    }
}

using System;
using THREE;
using THREE.Silk;
using Color = THREE.Color;
namespace THREE.Silk.Example
{
    [Example("03-Materials-Light", ExampleCategory.LearnThreeJS, "Chapter01")]
    public class MaterialsLightExample : Example
    {

        public Mesh cube,plane,sphere;
       
        public MaterialsLightExample() : base()
        {

        }
        public override void InitLighting()
        {
            base.InitLighting();
            var spotLight = new SpotLight(new Color().SetHex(0xffffff));

            spotLight.Position.Set(-40, 60, -10);
            spotLight.CastShadow = true;
            spotLight.Shadow.MapSize = new THREE.Vector2(1024, 1024);
            spotLight.Shadow.Camera.Far = 130;
            spotLight.Shadow.Camera.Near = 40;
            scene.Add(spotLight);

            var ambienLight = new THREE.AmbientLight(0x353535);
            scene.Add(ambienLight);

        }

        public override void Init()
        {
            base.Init();

            scene.Background = Color.Hex(0xffffff);

            var planeGeometry = new PlaneGeometry(60, 20);
            var planeMaterial = new MeshLambertMaterial() { Color = Color.Hex(0xcccccc) };
            plane = new Mesh(planeGeometry, planeMaterial);
            plane.ReceiveShadow = true;
            plane.Rotation.X = (float)(-0.5 * Math.PI);
            plane.Position.Set(15, 0, 0);

            scene.Add(plane);

            // create a cube
            var cubeGeometry = new BoxGeometry(4, 4, 4);
            var cubeMaterial = new MeshLambertMaterial() { Color = Color.Hex(0xff0000) };
            cube = new Mesh(cubeGeometry, cubeMaterial);
            cube.CastShadow = true;
            // position the cube
            cube.Position.Set(-4, 3, 0);

            // add the cube to the scene

            scene.Add(cube);

            //      // create a sphere
            var sphereGeometry = new SphereGeometry(4, 20, 20);
            var sphereMaterial = new MeshLambertMaterial() { Color = Color.Hex(0x7777ff) };
            sphere = new Mesh(sphereGeometry, sphereMaterial);
            sphere.CastShadow = true;
            //      // position the sphere
            sphere.Position.Set(20, 4, 2);

            //      // add the sphere to the scene
            scene.Add(sphere);

        }
        public override void Render()
        {
            base.Render();
        }
        public override void OnResize(ResizeEventArgs clientSize)
        {
            base.OnResize(clientSize);
            camera.Aspect = this.AspectRatio;
            camera.UpdateProjectionMatrix();
        }
    }
}

using System;
using THREE;
using OpenTK;
using OpenTK.Windowing.Common;

namespace THREEExample.Learning.Chapter01
{
    [Example("02-First-Scene", ExampleCategory.LearnThreeJS, "Chapter01")]
    public class FirstSceneExample : Example
    {


        public FirstSceneExample() : base()
        {
        }
     
        public override void Load(IThreeWindow glControl)
        {
            base.Load(glControl);



            scene.Background = Color.Hex(0xffffff);

            var axes = new AxesHelper(20);

            scene.Add(axes);

            var planeGeometry = new PlaneGeometry(60, 20);
            var planeMaterial = new MeshBasicMaterial() { Color = Color.Hex(0xcccccc) };
            var plane = new Mesh(planeGeometry, planeMaterial);

            plane.Rotation.X = (float)(-0.5 * Math.PI);
            plane.Position.Set(15, 0, 0);

            scene.Add(plane);

            // create a cube
            var cubeGeometry = new BoxGeometry(4, 4, 4);
            var cubeMaterial = new MeshBasicMaterial(){ Color=Color.Hex(0xff0000), Wireframe= true};
            var cube = new Mesh(cubeGeometry, cubeMaterial);

            // position the cube
            cube.Position.Set(-4, 3, 0);

            // add the cube to the scene
        
		    scene.Add(cube);

      //      // create a sphere
            var sphereGeometry = new SphereGeometry(4, 20, 20);
            var sphereMaterial = new MeshBasicMaterial() { Color = Color.Hex(0x7777ff), Wireframe = true };
            var sphere = new Mesh(sphereGeometry, sphereMaterial);

      //      // position the sphere
            sphere.Position.Set(20, 4, 2);

      //      // add the sphere to the scene
            scene.Add(sphere);
        }
        public override void Render()
        {
            controls.Update();
            this.renderer.Render(scene, camera);
        }
        public override void OnResize(ResizeEventArgs clientSize)
        {
            base.OnResize(clientSize);
            camera.Aspect = this.glControl.AspectRatio;
            camera.UpdateProjectionMatrix();
        }
    }
}

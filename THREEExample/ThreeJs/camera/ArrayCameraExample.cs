using OpenTK.Windowing.Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using THREE;
using Color = THREE.Color;
namespace THREEExample.Three.camera
{
    [Example("arraycamera", ExampleCategory.ThreeJs, "camera")]
    public class ArrayCameraExample : Example
    {
        Mesh mesh;
        int AMOUNT = 6;

        public ArrayCameraExample() : base()
        {

        }

        public override void InitCamera()
        {

            var ASPECT_RATIO = glControl.AspectRatio;

            var WIDTH = (glControl.Width / (float)AMOUNT);
            var HEIGHT = (glControl.Height / (float)AMOUNT);

            var cameras = new List<Camera>();
            for (var y = 0; y < AMOUNT; y++)
            {

                for (var x = 0; x < AMOUNT; x++)
                {

                    var subcamera = new PerspectiveCamera(40, ASPECT_RATIO, 0.1f, 10);
                    subcamera.Viewport = new Vector4((float)Math.Floor((decimal)x) * WIDTH, (float)Math.Floor((decimal)y) * HEIGHT, (float)Math.Ceiling((decimal)WIDTH), (float)Math.Ceiling((decimal)HEIGHT));
                    subcamera.Position.X = (x / AMOUNT) - 0.5f;
                    subcamera.Position.Y = 0.5f - (y / AMOUNT);
                    subcamera.Position.Z = 1.5f;
                    subcamera.Position.MultiplyScalar(2);
                    subcamera.LookAt(0, 0, 0);
                    subcamera.UpdateMatrixWorld();
                    cameras.Add(subcamera);

                }

            }
            camera = new ArrayCamera(cameras);
            camera.Position.Z = 3;
        }
        public override void InitCameraController()
        {
        }
        public override void InitLighting()
        {
            scene.Add(new AmbientLight(0x222244));
            var light = new DirectionalLight(0xffffff);
            light.Position.Set(0.5f, 0.5f, 1);
            light.CastShadow = true;
            light.Shadow.Camera.Zoom = 4; // tighter shadow map
            scene.Add(light);
        }

        public void createObject()
        {
            var geometry = new PlaneBufferGeometry(100, 100);
            var material = new MeshPhongMaterial() { Color = THREE.Color.Hex(0x000066) };

            var background = new Mesh(geometry, material);
            background.ReceiveShadow = true;
            background.Position.Set(0, 0, -1);
            scene.Add(background);

            var geometry1 = new CylinderBufferGeometry(0.5f, 0.5f, 1, 32);
            var material1 = new MeshPhongMaterial() { Color = Color.Hex(0xff0000) };

            mesh = new Mesh(geometry1, material1);
            mesh.CastShadow = true;
            mesh.ReceiveShadow = true;
            scene.Add(mesh);
        }
        public override void Init()
        {
            base.Init();

            createObject();
        }
        public override void Render()
        {
            mesh.Rotation.X += 0.005f;
            mesh.Rotation.Z += 0.01f;
            renderer.Render(scene, camera);
        }
        public override void OnResize(ResizeEventArgs clientSize)
        {
            var ASPECT_RATIO = glControl.AspectRatio;
            var WIDTH = (glControl.Width / (float)AMOUNT);
            var HEIGHT = (glControl.Height / (float)AMOUNT);

            camera.Aspect = ASPECT_RATIO;
            (camera as ArrayCamera).UpdateProjectionMatrix();

            for (var y = 0; y < AMOUNT; y++)
            {

                for (var x = 0; x < AMOUNT; x++)
                {

                    var subcamera = (camera as ArrayCamera).Cameras[AMOUNT * y + x];

                    subcamera.Viewport.Set(
                        (float)Math.Floor((float)x * WIDTH),
                        (float)Math.Floor((float)y * HEIGHT),
                        (float)Math.Ceiling((float)WIDTH),
                        (float)Math.Ceiling((float)HEIGHT));

                    subcamera.Aspect = ASPECT_RATIO;
                    (subcamera as PerspectiveCamera).UpdateProjectionMatrix();

                }

            }
            
            base.OnResize(clientSize);
        }
    }
}

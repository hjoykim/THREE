using System;
using System.Collections.Generic;
using THREE;
using Color = THREE.Color;
namespace THREE.Silk.Example.Geometry
{
    [Example("geometries1", ExampleCategory.ThreeJs, "Geometry")]
    public class GeometriesExample : Example
    {

        public GeometriesExample() : base()
        {

        }
        public override void InitCamera()
        {
            camera = new PerspectiveCamera(45, this.AspectRatio, 0.1f, 1000); ;
            camera.Position.Set(0, 50, 60);
            camera.LookAt(THREE.Vector3.Zero());
        }
        public override void InitLighting()
        {
            scene.Add(new AmbientLight(Color.Hex(0xcccccc), 0.4f));

            camera.Add(new PointLight(Color.Hex(0xffffff), 0.8f));

            scene.Add(camera);

        }
        public override void Init()
        {
            base.Init();

            camera.Position.Y = 400;

            CreateObject();
        }
        private float GetDelta()
        {
            return stopWatch.ElapsedMilliseconds / 10000.0f;
        }
        public override void Render()
        {
            var timer = GetDelta();

            camera.Position.X = (float)Math.Cos(timer) * 800;
            camera.Position.Y = (float)Math.Sin(timer) * 800;

            camera.LookAt(scene.Position);

            scene.Traverse(object3d => {
                if (object3d is Mesh)
                {
                    object3d.Rotation.X = timer * 5;
                    object3d.Rotation.Y = timer * 2.5f;
                }
            });

            base.Render();
        }
        private void CreateObject()
        {
            var map = TextureLoader.Load(@"../../../../assets/textures/uv_grid_opengl.jpg");

            map.WrapS = map.WrapT = Constants.RepeatWrapping;

            map.Anisotropy = 16;

            var material = new MeshPhongMaterial()
            {
                Map = map,

                Side = Constants.DoubleSide
            };

            //SphereBufferGeometry
            var object3d = new Mesh(new SphereBufferGeometry(75, 20, 10), material);

            object3d.Position.Set(-300, 0, 200);
            scene.Add(object3d);

            //IcosahedronBufferGeometry
            object3d = new Mesh(new IcosahedronBufferGeometry(75, 1), material);

            object3d.Position.Set(-100, 0, 200);
            scene.Add(object3d);

            //OctahedronBufferGeometry
            object3d = new Mesh(new OctahedronBufferGeometry(75, 2), material);

            object3d.Position.Set(100, 0, 200);
            scene.Add(object3d);

            //TetrahedronBufferGeometry
            object3d = new Mesh(new TetrahedronBufferGeometry(75, 0), material);

            object3d.Position.Set(300, 0, 200);
            scene.Add(object3d);

            //PlaneBufferGeometry
            object3d = new Mesh(new PlaneBufferGeometry(100, 100, 4, 4), material);

            object3d.Position.Set(-300, 0, 0);
            scene.Add(object3d);

            //BoxBufferGeometry
            object3d = new Mesh(new BoxBufferGeometry(100, 100, 100, 1, 1, 1), material);

            object3d.Position.Set(-100, 0, 0);
            scene.Add(object3d);

            //CircleBufferGeometry
            object3d = new Mesh(new CircleBufferGeometry(50, 50, 0, (float)Math.PI * 2), material);

            object3d.Position.Set(100, 0, 0);
            scene.Add(object3d);

            //RingBufferGeometry
            object3d = new Mesh(new RingBufferGeometry(10, 50, 50, 5, 0, (float)Math.PI * 2), material);

            object3d.Position.Set(300, 0, 0);
            scene.Add(object3d);

            //CylinderBufferGeometry
            object3d = new Mesh(new CylinderBufferGeometry(25, 75, 100, 40, 5), material);

            object3d.Position.Set(-300, 0, -200);
            scene.Add(object3d);

            List<Vector3> points = new List<Vector3>();
            for (var i = 0; i < 50; i++)
            {
                points.Add(new Vector3((float)Math.Sin(i * 0.2) * (float)Math.Sin(i * 0.1) * 15 + 50, (i - 5) * 2, 0));
            }

            //LatheBufferGeometry
            object3d = new Mesh(new LatheBufferGeometry(points.ToArray(), 20), material);

            object3d.Position.Set(-100, 0, -200);
            scene.Add(object3d);

            //TorusBufferGeometry
            object3d = new Mesh(new TorusBufferGeometry(50, 20, 20, 20), material);

            object3d.Position.Set(100, 0, -200);
            scene.Add(object3d);

            //TorusKnotBufferGeometry
            object3d = new Mesh(new TorusKnotBufferGeometry(50, 10, 50, 20), material);

            object3d.Position.Set(300, 0, -200);
            scene.Add(object3d);
        }
    }
}

using System;
using THREE;
using Vector2 = THREE.Vector2;
using Color = THREE.Color;
namespace THREE.Silk.Example
{
    [Example("Interactive_cubes", ExampleCategory.ThreeJs, "Interactive")]
    public class InteractiveCubesExample : Example
    {
        Raycaster raycaster;

        Vector2 pointer = new Vector2();
        public InteractiveCubesExample() : base()
        {
            scene = new Scene();
        }
        public override void InitRenderer()
        {
            base.InitRenderer();
            this.renderer.SetClearColor(Color.Hex(0x000000));
            this.renderer.ShadowMap.Enabled = true;
            this.renderer.ShadowMap.type = Constants.PCFSoftShadowMap;
        }

        public override void InitCamera()
        {
            base.InitCamera();
            camera = new PerspectiveCamera(50, this.AspectRatio, 1f, 10000); ;
            //camera.Position.Set(1000, 500, 1000);
            //camera.LookAt(0, 200, 0);
        }

        public override void InitLighting()
        {
            base.InitLighting();
            var light = new DirectionalLight(Color.Hex(0xffffff), 1);
            light.Position.Set(1, 1, 1).Normalize();
            scene.Add(light);
        }

        public override void Init()
        {
            base.Init();

            raycaster = new Raycaster();

            var geometry = new BoxGeometry(20, 20, 20);

            for (int i = 0; i < 2000; i++)
            {

                var object3d = new Mesh(geometry, new MeshLambertMaterial() { Color = new Color().Random() });

                object3d.Position.X = (float)MathUtils.random.NextDouble() * 800 - 400;
                object3d.Position.Y = (float)MathUtils.random.NextDouble() * 800 - 400;
                object3d.Position.Z = (float)MathUtils.random.NextDouble() * 800 - 400;

                object3d.Rotation.X = (float)(MathUtils.random.NextDouble() * 2 * Math.PI);
                object3d.Rotation.Y = (float)(MathUtils.random.NextDouble() * 2 * Math.PI);
                object3d.Rotation.Z = (float)(MathUtils.random.NextDouble() * 2 * Math.PI);

                object3d.Scale.X = (float)(MathUtils.random.NextDouble() + 0.5);
                object3d.Scale.Y = (float)(MathUtils.random.NextDouble() + 0.5);
                object3d.Scale.Z = (float)(MathUtils.random.NextDouble() + 0.5);

                scene.Add(object3d);

            }

            this.MouseMove += OnPointerMove;

        }
        Object3D INTERSECTED;


        private void OnPointerMove(object sender, MouseEventArgs e)
        {
            pointer.X = e.X / (this.Width * 1.0f) * 2 - 1;
            pointer.Y = -e.Y / (this.Height * 1.0f) * 2 + 1;
        }

        public override void Render()
        {

            camera.LookAt(scene.Position);
            camera.UpdateMatrixWorld();
            raycaster.SetFromCamera(pointer, camera);
            var intersects = raycaster.IntersectObjects(scene.Children, true);
            if(intersects.Count>0)
            {
                if(!intersects[0].object3D.Equals(INTERSECTED))
                {
                    if (INTERSECTED != null&& INTERSECTED["currentHex"] != null)
                    {
                        Color currentHex = (Color)INTERSECTED["currentHex"];
                        INTERSECTED.Material.Emissive = currentHex;
                    }

                    INTERSECTED = intersects[0].object3D as Mesh;
                    INTERSECTED["currentHex"] = INTERSECTED.Material.Emissive.Value;
                    INTERSECTED.Material.Emissive = Color.Hex(0xff0000);
                }
            }
            else
            {
                if (INTERSECTED != null) 
                    if(INTERSECTED["currentHex"]!=null)
                        INTERSECTED.Material.Emissive = (Color)INTERSECTED["currentHex"];
                INTERSECTED = null;
            }

            base.Render();
        }
    }
}

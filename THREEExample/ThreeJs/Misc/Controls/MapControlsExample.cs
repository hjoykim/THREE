using ImGuiNET;
using System;
using System.Collections.Generic;
using THREE;

namespace THREEExample.ThreeJs.Misc.Controls
{
    [Example("Controls-Map", ExampleCategory.Misc, "Controls")]
    public class MapControlsExample : Example
    {
        MapControls mapControls;
        public MapControlsExample() : base()
        {
        }
        public override void InitCamera()
        {
            camera = new PerspectiveCamera(60, glControl.AspectRatio, 1, 1000);
            camera.Position.Set(0, 200, -400);
        }
        public override void InitLighting()
        {
            base.InitLighting();
            var dirLight1 = new THREE.DirectionalLight(0xffffff, 3);
            dirLight1.Position.Set(1, 1, 1);
            scene.Add(dirLight1);

            var dirLight2 = new THREE.DirectionalLight(0x002288, 3);
            dirLight2.Position.Set(-1, -1, -1);
            scene.Add(dirLight2);

            var ambientLight = new THREE.AmbientLight(0x555555);
            scene.Add(ambientLight);
        }
        public override void InitCameraController()
        {
            mapControls = new MapControls(this,camera);


            mapControls.EnableDamping = true; // an animation loop is required when either damping or auto-rotation are enabled
            mapControls.DampingFactor = 0.05f;

            mapControls.ScreenSpacePanning = false;

            mapControls.MinDistance = 100;
            mapControls.MaxDistance = 500;

            mapControls.MaxPolarAngle = (float)Math.PI / 2;
            
        }
        public virtual void BuildScene()
        {
            scene.Background = THREE.Color.Hex(0xcccccc);
            scene.Fog = new Fog(0xcccccc, 0.002f);
            var geometry = new THREE.BoxBufferGeometry();
            geometry.Translate(0, 0.5f, 0);
            var material = new THREE.MeshPhongMaterial() { Color = THREE.Color.Hex(0xeeeeee), FlatShading= true };

				for (var i = 0; i< 500; i ++ ) {

					var mesh = new THREE.Mesh(geometry, material);
                    mesh.Position.X = MathUtils.NextFloat()* 1600 - 800;
					mesh.Position.Y = 0;
					mesh.Position.Z = MathUtils.NextFloat()* 1600 - 800;
					mesh.Scale.X = 20;
					mesh.Scale.Y = MathUtils.NextFloat()* 80 + 10;
					mesh.Scale.Z = 20;
					mesh.UpdateMatrix();
					mesh.MatrixAutoUpdate = false;
					scene.Add(mesh );

				}
}
        public override void Init()
        {
            base.Init();
            BuildScene();

            AddGuiControlsAction = () =>
            {
                ImGui.Checkbox("screenSpacePanning", ref mapControls.ScreenSpacePanning);
            };
        }

        public override void Render()
        {
            mapControls?.Update();
            renderer?.Render(scene, camera);
        }


    }
}

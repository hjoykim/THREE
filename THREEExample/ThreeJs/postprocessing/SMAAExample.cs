using OpenTK.Windowing.Common;
using System;
using System.Collections.Generic;
using System.Text;
using THREE;
using Color = THREE.Color;
namespace THREEExample.ThreeJs.postprocessing
{
    [Example("SMAA", ExampleCategory.ThreeJs, "PostPorcessing")]
    public class SMAAExample : Example
    {
        EffectComposer composer;
        public SMAAExample() :base()
        {
            scene.Background = Color.Hex(0x000000);
        }
        public override void InitCamera()
        {
            base.InitCamera();
            camera.Fov = 70;
            camera.Position.Z = 300;
            camera.UpdateProjectionMatrix();
        }
        public override void Init()
        {
            base.Init();
            InitGeometry();
        }
        private void InitGeometry()
        {
            var geometry = new THREE.BoxGeometry(120, 120, 120);
            var material1 = new THREE.MeshBasicMaterial() { Color = Color.Hex(0xffffff), Wireframe = true };

			var mesh1 = new THREE.Mesh(geometry, material1);
            mesh1.Position.X = - 100;
			scene.Add(mesh1 );

			var texture = TextureLoader.Load("../../../../assets/textures/brick_diffuse.jpg");
            texture.Anisotropy = 4;

			var material2 = new THREE.MeshBasicMaterial() { Map = texture };

			var mesh2 = new THREE.Mesh(geometry, material2);
            mesh2.Position.X = 100;
			scene.Add(mesh2 );

				// postprocessing

			composer = new EffectComposer(renderer );
            composer.AddPass( new RenderPass(scene, camera ) );

			var pass = new SMAAPass(glControl.Width,glControl.Height);
            composer.AddPass(pass );
        }
        public override void OnResize(ResizeEventArgs clientSize)
        {            
            base.OnResize(clientSize);
            composer.SetSize(clientSize.Width,clientSize.Height);
        }
        public override void Render()
        {
            if (!imGuiManager.ImWantMouse) controls.Enabled = true;
            else controls.Enabled = false;
            controls?.Update();

            foreach (var item in scene.Children)
            {
                item.Rotation.X += 0.005f;
                item.Rotation.Y += 0.01f;
            }
           
            composer.Render();
        }   
    }
}

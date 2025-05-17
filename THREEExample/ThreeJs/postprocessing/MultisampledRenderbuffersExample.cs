using OpenTK.Windowing.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using THREE;
using Color = THREE.Color;
namespace THREEExample.ThreeJs.postprocessing
{
    [Example("Multisampled_Renderbuffers", ExampleCategory.ThreeJs, "PostPorcessing")]
    public class MultisampledRenderbuffersExample : Example
    {
        EffectComposer composer1, composer2;
        Group group;
        public MultisampledRenderbuffersExample() :base() 
        {
            scene.Background = Color.Hex(0xffffff);
            scene.Fog = new Fog(0xcccccc, 100, 1500);
        }
        public override void InitRenderer()
        {
            base.InitRenderer();
           
        }
        public override void InitCamera()
        {
            camera = new THREE.PerspectiveCamera(45, glControl.Width / glControl.Height, 1, 2000);
            camera.Position.Z = 500;
            camera.LookAt(THREE.Vector3.Zero());
        }
        public override void InitLighting()
        {
            base.InitLighting();
            var hemiLight = new HemisphereLight(Color.Hex(0xffffff), Color.Hex(0x444444));
            hemiLight.Position.Set(0, 1000, 0);
            scene.Add(hemiLight);

            var dirLight = new THREE.DirectionalLight(0xffffff, 0.8f);
            dirLight.Position.Set(-3000, 1000, -1000);
            scene.Add(dirLight);
        }
        public override void Init()
        {
            base.Init();
            InitGeometry();
        }
        private void InitGeometry()
        {
            group = new THREE.Group();

            var geometry = new THREE.IcosahedronBufferGeometry(10, 2);
            var material = new THREE.MeshStandardMaterial() { Color = Color.Hex(0xee0808), FlatShading= true };

			for (var i = 0; i< 100; i ++ ) {
				var mesh = new THREE.Mesh(geometry, material);
                mesh.Position.X = MathUtils.NextFloat() * 500 - 250;
				mesh.Position.Y = MathUtils.NextFloat() * 500 - 250;
				mesh.Position.Z = MathUtils.NextFloat() * 500 - 250;
				mesh.Scale.SetScalar(MathUtils.NextFloat()* 2 + 1 );
				group.Add(mesh );

			}

            scene.Add(group);
            var parameters = new Hashtable{
                    { "format", Constants.RGBFormat },
                    {"stencilBuffer", false }
                };
            var renderTarget = new GLMultisampleRenderTarget(renderer.Width, renderer.Height, parameters);
            var renderPass = new THREE.RenderPass(scene, camera);

            var copyPass = new THREE.ShaderPass(new CopyShader());

            //

            composer1 = new THREE.EffectComposer(renderer,renderTarget);
            composer1.AddPass(renderPass);
            composer1.AddPass(copyPass);

            //

            composer2 = new THREE.EffectComposer(renderer);
            composer2.AddPass(renderPass);
            composer2.AddPass(copyPass);
        }
        public override void OnResize(ResizeEventArgs clientSize)
        {
            base.OnResize(clientSize);
        }
        public override void Render()
        {

            if (!imGuiManager.ImWantMouse) controls.Enabled = true;
            else controls.Enabled = false;
            controls?.Update();

            var halfWidth = glControl.Width / 2;

            group.Rotation.Y += 0.001f;

            renderer.AutoClear = false;
            renderer.Clear();

            renderer.SetViewport(0, 0, halfWidth, glControl.Height);           
            camera.Aspect = glControl.Width*0.5f/glControl.Height;
            camera.UpdateProjectionMatrix();
            composer1.Render();

            renderer.SetViewport(halfWidth, 0, halfWidth, glControl.Height);

            composer2.Render();

            renderer.SetViewport(0,0, glControl.Width,glControl.Height);
        }
    }
}

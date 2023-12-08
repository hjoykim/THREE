using OpenTK;
using System;
using System.Diagnostics;
using THREE;
using THREEExample.Learning.Utils;
using THREEExample.ThreeImGui;

namespace THREEExample.Learning.Chapter10
{
    [Example("16-specular-map", ExampleCategory.LearnThreeJS, "Chapter10")]
    public class SpecularMapExample : TemplateExample
    {
        Mesh sphere1;
        public SpecularMapExample() : base()
        {

        }
        public override void Load(GLControl control)
        {
            Debug.Assert(null != control);

            glControl = control;

            this.renderer = new THREE.GLRenderer();

            this.renderer.Context = control.Context;
            this.renderer.Width = control.Width;
            this.renderer.Height = control.Height;

            this.renderer.Init();

            stopWatch.Start();

            InitRenderer();

            InitCamera();

            InitCameraController();

            imguiManager = new ImGuiManager(this.glControl);

            DemoUtils.InitDefaultLighting(scene);

            (scene.GetObjectByName("ambientLight") as AmbientLight).Color.SetHex(0x050505);

            var earthMaterial = new MeshPhongMaterial{
                Map = TextureLoader.Load("../../../../assets/textures/earth/Earth.png"),
                NormalMap = TextureLoader.Load("../../../../assets/textures/earth/EarthNormal.png"),
                SpecularMap = TextureLoader.Load("../../../../assets/textures/earth/EarthSpec.png"),
                NormalScale = new THREE.Vector2(6, 6)
            };

            var sphere = new SphereBufferGeometry(9, 50, 50);
            sphere1 = AddGeometryWithMaterial(scene, sphere, "sphere",earthMaterial);
            sphere1.Rotation.Y = 1 / 6 * (float)Math.PI;
        }

        public override void Render()
        {
            if (!imguiManager.ImWantMouse)
                controls.Enabled = true;
            else
                controls.Enabled = false;
            controls.Update();

            this.renderer.Render(scene, camera);
            
            sphere1.Rotation.Y -= 0.01f;

            //ShowGUIControls();

            this.renderer.state.currentProgram = -1;
            this.renderer.bindingStates.currentState = this.renderer.bindingStates.defaultState;
        }       
    }
}

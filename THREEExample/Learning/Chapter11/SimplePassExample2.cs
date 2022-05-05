using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using THREE;
using THREE.Cameras;
using THREE.Cameras.Controlls;
using THREE.Math;
using THREE.Objects;
using THREE.Postprocessing;
using THREE.Scenes;
using THREE.Shaders;
using THREEExample.ThreeImGui;
using THREE.Core;
using ImGuiNET;
using System.Collections;

namespace THREEExample.Learning.Chapter11
{
    [Example("03-SimplePass2", ExampleCategory.LearnThreeJS, "Chapter11")]
    public class SimplePassExample2 : EffectComposerTemplate
    {
   
        public SimplePassExample2() : base()
        {
           
        }
        
       
        public override void Load(GLControl control)
        {
            base.Load(control);
           
            // setup effects
            renderPass = new RenderPass(scene, camera);
            glitchPass = new GlitchPass();
           

            halftonePass = new HalftonePass();
            List<Object3D> selectLists = new List<Object3D>();
            selectLists.Add(earth);
            outlinePass = new OutlinePass(new THREE.Math.Vector2(control.Width, control.Height), scene, camera, selectLists);
            unrealBloomPass = new UnrealBloomPass();

            var effectCopy = new ShaderPass(new CopyShader());
            effectCopy.RenderToScreen = true;

            // define the composers
            composer1 = new EffectComposer(renderer);
            composer1.AddPass(renderPass);
            composer1.AddPass(glitchPass);
            composer1.AddPass(effectCopy);

            composer2 = new EffectComposer(renderer);
            composer2.AddPass(renderPass);
            composer2.AddPass(halftonePass);
            composer2.AddPass(effectCopy);

            composer3 = new EffectComposer(renderer);
            composer3.AddPass(renderPass);
            composer3.AddPass(outlinePass);
            composer3.AddPass(effectCopy);

            composer4 = new EffectComposer(renderer);
            composer4.AddPass(renderPass);
            composer4.AddPass(unrealBloomPass);
            composer4.AddPass(effectCopy);      
            
        }
        public override void Render()
        {
            base.Render();

            renderer.AutoClear = false;
            renderer.Clear();

           

            renderer.SetViewport(0, 0, halfWidth, halfHeight);
            composer1.Render();

            renderer.SetViewport(0, halfHeight, halfWidth, halfHeight);
            composer2.Render();

            renderer.SetViewport(halfWidth, 0, halfWidth, halfHeight);
            composer3.Render();

            renderer.SetViewport(halfWidth, halfHeight, halfWidth, halfHeight);
            composer4.Render();

            renderer.SetViewport(0, 0, renderer.Width, renderer.Height);
            ShowControls();


        }
        public override void ShowControls()
        {
            ImGui.NewFrame();
            ImGui.Begin("Controls");
            AddGiltchPassControl("GlitchPass", glitchPass);
            AddHalftonePassControl("HalfTonePass", halftonePass);
            AddOutlinePassControls("OutlinePass", outlinePass);
            ImGui.End();
            ImGui.Render();
            imGuiManager.ImGui_ImplOpenGL3_RenderDrawData(ImGui.GetDrawData());
        }
    }
}

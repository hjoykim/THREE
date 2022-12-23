using ImGuiNET;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using THREE;
using THREEExample.Learning.Utils;
using THREEExample.ThreeImGui;
using Vector2 = THREE.Vector2;
namespace THREEExample.Learning.Chapter10
{
    [Example("20-uv-mapping-manual", ExampleCategory.LearnThreeJS, "Chapter10")]
    public class UVMappingManualExample : TemplateExample
    {
        BoxGeometry geom;
        public UVMappingManualExample() : base()
        {

        }

        public override void Load(GLControl control)
        {
            Debug.Assert(null != control);

            glControl = control;

            this.renderer = new GLRenderer();

            this.renderer.Context = control.Context;
            this.renderer.Width = control.Width;
            this.renderer.Height = control.Height;

            this.renderer.Init();

            stopWatch.Start();

            InitRenderer();

            InitCamera();

            InitCameraController();

            imguiManager = new ImGuiManager(this.glControl);

            var groundPlane = DemoUtils.AddLargeGroundPlane(scene,true);
            groundPlane.Position.Y = -8;
            groundPlane.ReceiveShadow = true;

            DemoUtils.InitDefaultLighting(scene);

            var material = new MeshStandardMaterial{
                Map = TextureLoader.Load("../../../assets/textures/uv/ash_uvgrid01.jpg"),
                Metalness = 0.02f,
                Roughness = 0.07f,
                Color = new Color(0xffffff)
            };

            geom = new BoxGeometry(14, 14, 14);
            geom.FaceVertexUvs.Add(new List<List<Vector2>>());
            geom.FaceVertexUvs[0].Add(new List<Vector2>());
            geom.FaceVertexUvs[0][0].Add(new Vector2());
            geom.FaceVertexUvs[0][0].Add(new Vector2());
            geom.FaceVertexUvs[0][0].Add(new Vector2());

            geom.FaceVertexUvs[0][0][0].X = 0.5f;
            geom.FaceVertexUvs[0][0][0].Y = 0.7f;
            geom.FaceVertexUvs[0][0][1].X = 0.4f;
            geom.FaceVertexUvs[0][0][1].Y = 0.1f;
            geom.FaceVertexUvs[0][0][2].X = 0.4f;
            geom.FaceVertexUvs[0][0][2].Y = 0.5f;

            var mesh = new Mesh(geom, material);
            mesh.Rotation.Y = 1.7f * (float)Math.PI;
            scene.Add(mesh);
                       
        }

        public override void Render()
        {
            if (!imguiManager.ImWantMouse)
                controls.Enabled = true;
            else
                controls.Enabled = false;
            controls.Update();
            this.renderer.Render(scene, camera);

            ShowGUIControls();
        }
        public override void ShowGUIControls()
        {
            ImGui.NewFrame();
            ImGui.Begin("Controls");
            if(ImGui.SliderFloat("uv1", ref geom.FaceVertexUvs[0][0][0].X,0,1))
                geom.UvsNeedUpdate=true;
            if(ImGui.SliderFloat("uv2", ref geom.FaceVertexUvs[0][0][0].Y,0,1))
                geom.UvsNeedUpdate = true;
            if(ImGui.SliderFloat("uv3", ref geom.FaceVertexUvs[0][0][1].X,0,1))
                geom.UvsNeedUpdate = true;
            if(ImGui.SliderFloat("uv4", ref geom.FaceVertexUvs[0][0][1].Y,0,1))
                geom.UvsNeedUpdate = true;
            if(ImGui.SliderFloat("uv5", ref geom.FaceVertexUvs[0][0][2].X,0,1))
                geom.UvsNeedUpdate = true;
            if(ImGui.SliderFloat("uv6", ref geom.FaceVertexUvs[0][0][2].Y,0,1))
                geom.UvsNeedUpdate = true;
            ImGui.End();
            ImGui.Render();

            imguiManager.ImGui_ImplOpenGL3_RenderDrawData(ImGui.GetDrawData());
        }
    }
}

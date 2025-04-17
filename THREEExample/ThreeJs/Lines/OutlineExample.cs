using System;
using System.Collections.Generic;
using THREE.Objects;
using THREE;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Common.Input;
using ImGuiNET;
namespace THREEExample.ThreeJs.Lines
{
    [Example("outline", ExampleCategory.ThreeJs, "lines")]
    public class OutlineExample : Example
    {
        OutlineMesh po, co, to;
        Mesh pm, cm, tm;
        Vector2 resulution = new Vector2();
        OutlineMaterial mat;
        Material m;
        float angleThreshold;
        float opacity;
        public OutlineExample() : base()
        {

        }
        public override void InitRenderer()
        {
            base.InitRenderer();
            renderer.SetClearColor(0xffffff);
        }
        public override void InitCamera()
        {
            base.InitCamera();
            camera.Fov = 60;
            camera.Near = 1;
            camera.Far = 1000;
            camera.Position.Z = 20;
            camera.UpdateProjectionMatrix();
        }
        public override void OnResize(ResizeEventArgs clientSize)
        {
            base.OnResize(clientSize);
            resulution.Set(clientSize.Width, clientSize.Height);
        }
        private void CreateObjects()
        {
            var bg = new BoxBufferGeometry(2, 2, 2);
            var cg = new CylinderBufferGeometry(2, 4, 4, 32, 2, false);
            var tg = new TorusKnotBufferGeometry(2, 0.1f, 128, 32, 4, 6);

            m = new MeshBasicMaterial {
                Transparent = true,
                Opacity = 0.5f,
                PolygonOffset = true,
                PolygonOffsetUnits = 2,
                PolygonOffsetFactor = 1,
            };

            pm = new Mesh(bg, m);
            cm = new Mesh(cg, m);
            tm = new Mesh(tg, m);

        

            mat = new OutlineMaterial(60, true, THREE.Color.Hex(0x000));
            angleThreshold = mat.AngleThreshold;
            opacity = mat.Opacity;

            po = new OutlineMesh(pm, mat);
            co = new OutlineMesh(cm, mat);
            to = new OutlineMesh(tm, mat);

            po.Add(pm);
            co.Add(cm);
            to.Add(tm);
            po.Position.X = 8;
            co.Position.X = -8;

            scene.Add(po);
            scene.Add(co);
            scene.Add(to);
        }
        public override void Init()
        {
            base.Init();
            CreateObjects();

            AddGuiControlsAction = () =>
            {
                if (ImGui.SliderFloat("angleThreshold", ref angleThreshold, 0, 180)) mat.AngleThreshold = angleThreshold;
                ImGui.SliderFloat("opacity", ref m.Opacity, 0.0f, 1.0f);
            };
        }
        public override void Render()
        {
            float delta = GetDelta() / 10000;
            base.Render();
            to.Rotation.X += delta;
            to.Rotation.Z += delta;
        }
    }
}

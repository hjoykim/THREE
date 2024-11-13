using ImGuiNET;
using OpenTK.Windowing.Common;
using System;
using System.Collections.Generic;
using THREE;
using Color = THREE.Color;
namespace THREEExample
{
    [Example("line fat", ExampleCategory.ThreeJs, "lines")]
    public class LinesFatExample : Example
    {
        Camera camera2;
        int insetWidth, insetHeight;
        Material matLine,matLineBasic, matLineDashed;
        LineSegments2 line;
        Line line1;
        int lineType = 0;
        float lineWidth;
        bool alphaToCoverage = true;
        bool dashed = false;
        float dashScale = 1;
        int dashGap = 1;
        public LinesFatExample() : base() { }
        public override void InitCamera()
        {
            base.InitCamera();
            camera.Fov = 40;
            camera.Position.Set(-40, 0, 60);

            camera2 = new THREE.PerspectiveCamera(50, 1, 1, 1000);
            camera2.Position.Copy(camera.Position);
        }
        public override void InitRenderer()
        {
            base.InitRenderer();
            camera.Far = 1000;


        }
        private void CreateObjects()
        {
            insetWidth = glControl.Width/4;
            insetHeight = glControl.Height/4;

            List<float> positions = new List<float>();
            List<float> colors = new List<float>();

            var points = GeometryUtils.Hilbert3D(new THREE.Vector3(0, 0, 0), 20, 1, 0, 1, 2, 3, 4, 5, 6, 7);

            var spline = new THREE.CatmullRomCurve3(points);
            var divisions = (int)Math.Round((decimal)(12 * points.Count));
            var point = new THREE.Vector3();
            var color = new THREE.Color();

            for (int i = 0, l = divisions; i < l; i++)
            {

                var t = 1.0f*i / l;

                spline.GetPoint(t, point);
                positions.Add(point.X, point.Y, point.Z);

                color.SetHSL(t, 1.0f, 0.5f);
                colors.Add(color.R, color.G, color.B);

            }


            // Line2 ( LineGeometry, LineMaterial )

            var geometry = new LineGeometry();
            geometry.SetPositions(positions.ToArray());
            geometry.SetColors(colors.ToArray());

            matLine = new LineMaterial() {

                    Color = Color.Hex(0xffffff),
                    LineWidth = 5, // in pixels
					VertexColors = true,
					//resolution:  // to be set by renderer, eventually
					Dashed = false,
                    AlphaToCoverage = true
            };
            lineWidth = 5.0f;
            line = new Line2(geometry, matLine);
            line.ComputeLineDistances();
            line.Scale.Set(1, 1, 1);
            line.Visible = true;
            scene.Add(line);


            // THREE.Line ( THREE.BufferGeometry, THREE.LineBasicMaterial ) - rendered with gl.LINE_STRIP

            var geo = new THREE.BufferGeometry();
            geo.SetAttribute("position", new BufferAttribute<float>(positions.ToArray(), 3));
            geo.SetAttribute("color", new BufferAttribute<float>(colors.ToArray(), 3));

            matLineBasic = new THREE.LineBasicMaterial() { Color = Color.Hex(0xffffff),VertexColors = true } ;
            matLineDashed = new THREE.LineDashedMaterial() { VertexColors = true, Scale = 2, DashSize = 1, GapSize = 1 };

            line1 = new THREE.Line(geo, matLineBasic);
            line1.ComputeLineDistances();
            line1.Visible = false;
            scene.Add(line1);
        }
        public override void Init()
        {
            base.Init();        
            CreateObjects();
        }

        public override void OnResize(ResizeEventArgs clientSize)
        {
            base.OnResize(clientSize);

            insetWidth = glControl.Width / 4; // square
            insetHeight = glControl.Height / 4;

            camera2.Aspect = insetWidth / insetHeight;
            camera2.UpdateProjectionMatrix();
        }
        private void GuiControls()
        {
            LineMaterial mat = matLine as LineMaterial;
            if (ImGui.Combo("line Type", ref lineType, "LineGeometry\0gl.Line"))
            {
                switch (lineType)
                {
                    case 0: line.Visible = true; line1.Visible = false; break;
                    case 1: line.Visible = false; line1.Visible = true; break;
                }
            }

            if (ImGui.SliderFloat("width(px)", ref lineWidth, 0.0f, 10.0f))
                mat.LineWidth = lineWidth;

            if (ImGui.Checkbox("alphaToCoverage", ref alphaToCoverage))
                mat.AlphaToCoverage = alphaToCoverage;

            if (ImGui.Checkbox("dashed", ref dashed))
            {               
                mat.Dashed = dashed;
                line1.Material = dashed ? matLineDashed : matLineBasic;
            }

            if (ImGui.SliderFloat("dash scale", ref dashScale, 0.5f, 2.0f))
            {
                mat.DashScale = dashScale;
                (matLineDashed as LineDashedMaterial).Scale = dashScale;
            }

            if(ImGui.Combo("dash/gap",ref dashGap,"2 : 1\01 : 1\01 : 2"))
            {
                switch (dashGap)
                {
                    case 0:
                        mat.DashSize = 2;
                        mat.GapSize = 1;
                        (matLineDashed as LineDashedMaterial).DashSize = 2;
                        (matLineDashed as LineDashedMaterial).GapSize = 1;
                        break;
                    case 1:
                        mat.DashSize = 1;
                        mat.GapSize = 1;
                        (matLineDashed as LineDashedMaterial).DashSize = 1;
                        (matLineDashed as LineDashedMaterial).GapSize = 1;
                        break;
                    case 2:
                        mat.DashSize = 1;
                        mat.GapSize = 2;
                        (matLineDashed as LineDashedMaterial).DashSize = 1;
                        (matLineDashed as LineDashedMaterial).GapSize = 2;
                        break;

                }
            }
        }
        public override void Render()
        {
            if (!imGuiManager.ImWantMouse)
                controls.Enabled = true;
            else
                controls.Enabled = false;

            controls?.Update();

            renderer.SetClearColor(0x000000, 0.0f);
            renderer.SetViewport(0, 0, glControl.Width, glControl.Height);

            (matLine as LineMaterial).Resolution.Set(glControl.Width, glControl.Height);         
            renderer?.Render(scene, camera);

            ImGui.NewFrame();
            GuiControls();
            ImGui.Render();
            imGuiManager.ImGui_ImplOpenGL3_RenderDrawData(ImGui.GetDrawData());

            renderer.SetClearColor(0x222222, 1);

            renderer.ClearDepth(); // important!

            renderer.SetScissorTest(true);

            renderer.SetScissor(20, 20, insetWidth, insetHeight);

            renderer.SetViewport(20, 20, insetWidth, insetHeight);

            camera2.Position.Copy(camera.Position);
            camera2.Quaternion.Copy(camera.Quaternion);

            // renderer will set this eventually
            (matLine as LineMaterial).Resolution.Set(insetWidth, insetHeight); // resolution of the inset viewport

            renderer.Render(scene, camera2);

            renderer.SetScissorTest(false);
        }
    }
}

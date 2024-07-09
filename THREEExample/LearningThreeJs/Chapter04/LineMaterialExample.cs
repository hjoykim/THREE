using System;
using System.Collections.Generic;
using THREE;

using Color = THREE.Color;
namespace THREEExample.Learning.Chapter04
{
    [Example("12.Line-Material", ExampleCategory.LearnThreeJS, "Chapter04")]
    public class LineMaterialExample : MeshLambertMaterialExample
    {
        public Line line;
        public LineMaterialExample() : base()
        {

        }
        public override void Init()
        {
            base.Init();
            renderer.SetClearColor(0x000000);
            AddGuiControlsAction = null;
        }
        public override void BuildMeshMaterial()
        {
            meshMaterial = new LineBasicMaterial()
            {
                Opacity = 1.0f,
                LineWidth = 1,
                VertexColors = true
            };
        }
        public override void BuildMesh()
        {
            var points = Gosper(4, 60);

            var lines = new Geometry();
            List<Color> colors = new List<Color>();

            points.ForEach(p =>
            {
                lines.Vertices.Add(new Vector3(p.X, p.Z, p.Y));
                colors.Add(Color.Hex(0xffffff).SetHSL(p.X / 100 + 0.5f, (p.Y * 20) / 300, 0.8f));

            });

            lines.Colors = colors;

            line = new Line(lines, meshMaterial);
            line.Position.Set(25, -30, -60);

            scene.Add(line);
        }
        public override void BuildGeometry()
        {
            BuildMeshMaterial();

            BuildMesh();

        }
        public override void Render()
        {
            if (!imGuiManager.ImWantMouse)
                controls.Enabled = true;
            else
                controls.Enabled = false;

            controls?.Update();
            renderer?.Render(scene, camera);
            line.Rotation.Z = step += 0.001f;
        }
        public List<Vector3> Gosper(float a, float b)
        {
            var turtle = new float[] { 0, 0, 0 };
            var points = new List<Vector3>();
            var count = 0;

            rg(a, b, turtle);

            void rt(float x)
            {
                turtle[2] += x;
            }

            void lt(float x)
            {
                turtle[2] -= x;
            }

            void fd(float dist)
            {
                points.Add(new Vector3(turtle[0], turtle[1], (float)Math.Sin(count) * 5));

                var dir = turtle[2] * (float)Math.PI / 180;
                turtle[0] += (float)Math.Cos(dir) * dist;
                turtle[1] += (float)Math.Sin(dir) * dist;

                points.Add(new Vector3(turtle[0], turtle[1], (float)Math.Sin(count) * 5));
            }

            void rg(float st, float ln, float[] turtle1)
            {
                st--;
                ln = ln / 2.6457f;
                if (st > 0)
                {
                    //                    ctx.strokeStyle = '#111';
                    rg(st, ln, turtle1);
                    rt(60);
                    gl(st, ln, turtle1);
                    rt(120);
                    gl(st, ln, turtle1);
                    lt(60);
                    rg(st, ln, turtle1);
                    lt(120);
                    rg(st, ln, turtle1);
                    rg(st, ln, turtle1);
                    lt(60);
                    gl(st, ln, turtle1);
                    rt(60);
                }
                if (st == 0)
                {
                    fd(ln);
                    rt(60);
                    fd(ln);
                    rt(120);
                    fd(ln);
                    lt(60);
                    fd(ln);
                    lt(120);
                    fd(ln);
                    fd(ln);
                    lt(60);
                    fd(ln);
                    rt(60);
                }
            }
            void gl(float st, float ln, float[] turtle1)
            {
                st--;
                ln = ln / 2.6457f;
                if (st > 0)
                {
                    //                    ctx.strokeStyle = '#555';
                    lt(60);
                    rg(st, ln, turtle1);
                    rt(60);
                    gl(st, ln, turtle1);
                    gl(st, ln, turtle1);
                    rt(120);
                    gl(st, ln, turtle1);
                    rt(60);
                    rg(st, ln, turtle1);
                    lt(120);
                    rg(st, ln, turtle1);
                    lt(60);
                    gl(st, ln, turtle1);
                }
                if (st == 0)
                {
                    lt(60);
                    fd(ln);
                    rt(60);
                    fd(ln);
                    fd(ln);
                    rt(120);
                    fd(ln);
                    rt(60);
                    fd(ln);
                    lt(120);
                    fd(ln);
                    lt(60);
                    fd(ln);
                }
            }

            return points;
        }
    }
}

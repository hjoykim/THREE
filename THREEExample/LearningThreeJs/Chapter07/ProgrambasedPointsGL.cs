using ImGuiNET;
using System.IO;
using THREE;
using static THREE.GLInfo;

namespace THREEExample.Learning.Chapter07
{
    [Example("05.ProgramBased-OpenGL", ExampleCategory.LearnThreeJS, "Chapter07")]
    public class ProgrambasedPointsGL : Example
    {
        Points cloud = null;
        int range = 500;
        int _size = 15;
        float opacity = 0.6f;
        bool transparent = true;
        bool sizeAttenuation = true;
        bool rotate = true;
        System.Numerics.Vector3 color = new System.Numerics.Vector3(1,1,1);
        float step = 0;
        public ProgrambasedPointsGL() { }

        public override void InitCamera()
        {
            base.InitCamera();
        }
        public override void Load(GLControl control)
        {
            base.Load(control);
            AddGuiControlsAction = ShowControls;
            CreatePoints(_size, transparent, opacity, sizeAttenuation, color);
        }
        public override void Render()
        {
            if (!imGuiManager.ImWantMouse)
                controls.Enabled = true;
            else
                controls.Enabled = false;

            base.Render();
            if (rotate)
            {
                step += 0.01f;
                cloud.Rotation.X = step;
                cloud.Rotation.Z = step;
            }
        }
        private void Redraw()
        {
            CreatePoints(_size, transparent, opacity, sizeAttenuation, color);
        }
        private void ShowControls()
        {
            bool redraw = false;
           if(ImGui.SliderInt("size",ref _size,0,20))
            {
                redraw = true;
            }
           if(ImGui.Checkbox("transparent",ref transparent))
            {
                redraw = true;
            }
            if (ImGui.SliderFloat("opacity", ref opacity,0.0f,1.0f))
            {
                redraw = true;
            }
   
            if (ImGui.ColorPicker3("color", ref color))
            {
                redraw = true;
            }
            if (ImGui.Checkbox("sizeAttenuation", ref transparent))
            {
                redraw = true;
            }
            ImGui.Checkbox("rotate", ref rotate);
   
            if(redraw)
            {
                if (cloud != null)
                {
                    scene.Remove(cloud);
                }
                Redraw();
            }
        }

        private void CreatePoints(int size, bool transparent, float opacity, bool sizeAttenuation, System.Numerics.Vector3 color)
        {

            var geom = new THREE.Geometry();

            var material = new PointsMaterial() {
                Size = size,
                Transparent = transparent,
                Opacity = opacity,
                Map = CreateGhostTexture(),
                SizeAttenuation = sizeAttenuation,
                Color = new THREE.Color(color.X, color.Y, color.Z)
            };


            for (var i = 0; i < 5000; i++)
            {
                var particle = new THREE.Vector3(MathUtils.NextFloat() * range - range / 2, MathUtils.NextFloat() * range - range / 2,
                  MathUtils.NextFloat() * range - range / 2);
                geom.Vertices.Add(particle);
            }

            cloud = new Points(geom, material);
            cloud.Name = "Points";
            scene.Add(cloud);
        }
        private Texture CreateGhostTexture()
        {
            Canvas canvas = new Canvas();

            canvas.Width = 32;
            canvas.Height = 32;

            var path = new System.Windows.Shapes.Path();
            Canvas.SetTop(path, -84);
            Canvas.SetLeft(path, -81);
            path.Fill = System.Windows.Media.Brushes.Orange;

            var pathGeometry = new System.Windows.Media.PathGeometry();
            var pathFigure = new System.Windows.Media.PathFigure();


            pathFigure.StartPoint = new System.Windows.Point(83, 116);
            pathFigure.Segments.Add(new System.Windows.Media.LineSegment() { Point = new System.Windows.Point(83, 102) });
            pathFigure.Segments.Add(new BezierSegment() { Point1 = new System.Windows.Point(83, 94), Point2 = new System.Windows.Point(89, 88), Point3 = new System.Windows.Point(97, 88) });
            pathFigure.Segments.Add(new BezierSegment() { Point1 = new System.Windows.Point(105, 88), Point2 = new System.Windows.Point(111, 94), Point3 = new System.Windows.Point(111, 102) });
            pathFigure.Segments.Add(new LineSegment() { Point = new System.Windows.Point(111, 116) });
            pathFigure.Segments.Add(new LineSegment() { Point = new System.Windows.Point(106.333, 111.333) });
            pathFigure.Segments.Add(new LineSegment() { Point = new System.Windows.Point(101.666, 116) });
            pathFigure.Segments.Add(new LineSegment() { Point = new System.Windows.Point(97, 111.333) });
            pathFigure.Segments.Add(new LineSegment() { Point = new System.Windows.Point(92.333, 116) });
            pathFigure.Segments.Add(new LineSegment() { Point = new System.Windows.Point(87.666, 111.333) });
            pathFigure.Segments.Add(new LineSegment() { Point = new System.Windows.Point(83, 116) });

            pathGeometry.Figures.Add(pathFigure);
            path.Data = pathGeometry;
            canvas.Children.Add(path);



            path = new System.Windows.Shapes.Path();
            Canvas.SetTop(path, -84);
            Canvas.SetLeft(path, -81);
            path.Fill = System.Windows.Media.Brushes.White;


            pathGeometry = new System.Windows.Media.PathGeometry();

            pathFigure = new System.Windows.Media.PathFigure();
            pathFigure.StartPoint = new System.Windows.Point(91, 96);
            pathFigure.Segments.Add(new BezierSegment() { Point1 = new System.Windows.Point(88, 96), Point2 = new System.Windows.Point(87, 99), Point3 = new System.Windows.Point(87, 101) });
            pathFigure.Segments.Add(new BezierSegment() { Point1 = new System.Windows.Point(87, 103), Point2 = new System.Windows.Point(88, 106), Point3 = new System.Windows.Point(91, 106) });
            pathFigure.Segments.Add(new BezierSegment() { Point1 = new System.Windows.Point(94, 106), Point2 = new System.Windows.Point(95, 103), Point3 = new System.Windows.Point(95, 101) });
            pathFigure.Segments.Add(new BezierSegment() { Point1 = new System.Windows.Point(95, 99), Point2 = new System.Windows.Point(94, 96), Point3 = new System.Windows.Point(91, 96) });
            pathGeometry.Figures.Add(pathFigure);
            path.Data = pathGeometry;
            canvas.Children.Add(path);


            path = new System.Windows.Shapes.Path();
            Canvas.SetTop(path, -84);
            Canvas.SetLeft(path, -81);
            path.Fill = System.Windows.Media.Brushes.White;

            pathGeometry = new System.Windows.Media.PathGeometry();
            pathFigure = new System.Windows.Media.PathFigure();
            pathFigure.StartPoint = new System.Windows.Point(103, 96);
            pathFigure.Segments.Add(new BezierSegment() { Point1 = new System.Windows.Point(100, 96), Point2 = new System.Windows.Point(99, 99), Point3 = new System.Windows.Point(99, 101) });
            pathFigure.Segments.Add(new BezierSegment() { Point1 = new System.Windows.Point(99, 103), Point2 = new System.Windows.Point(100, 106), Point3 = new System.Windows.Point(103, 106) });
            pathFigure.Segments.Add(new BezierSegment() { Point1 = new System.Windows.Point(106, 106), Point2 = new System.Windows.Point(107, 103), Point3 = new System.Windows.Point(107, 101) });
            pathFigure.Segments.Add(new BezierSegment() { Point1 = new System.Windows.Point(107, 99), Point2 = new System.Windows.Point(106, 96), Point3 = new System.Windows.Point(103, 96) });
            pathGeometry.Figures.Add(pathFigure);
            path.Data = pathGeometry;
            canvas.Children.Add(path);


            path = new System.Windows.Shapes.Path();
            Canvas.SetTop(path, -84);
            Canvas.SetLeft(path, -81);
            path.Fill = System.Windows.Media.Brushes.White;

            pathGeometry = new System.Windows.Media.PathGeometry();
            pathFigure = new System.Windows.Media.PathFigure();
            pathFigure.StartPoint = new System.Windows.Point(103, 102);
            pathFigure.Segments.Add(new ArcSegment() { Size = new System.Windows.Size(2.5, 2.5), Point = new System.Windows.Point(102, 103), RotationAngle = 360, IsLargeArc = true });
            pathGeometry.Figures.Add(pathFigure);
            path.Data = pathGeometry;
            canvas.Children.Add(path);

            path = new System.Windows.Shapes.Path();
            Canvas.SetTop(path, -84);
            Canvas.SetLeft(path, -81);
            path.Fill = System.Windows.Media.Brushes.White;

            pathGeometry = new System.Windows.Media.PathGeometry();
            pathFigure = new System.Windows.Media.PathFigure();
            pathFigure.StartPoint = new System.Windows.Point(90, 103);
            pathFigure.Segments.Add(new ArcSegment() { Size = new System.Windows.Size(2.5, 2.5), Point = new System.Windows.Point(89, 103), RotationAngle = 360, IsLargeArc = true });
            pathGeometry.Figures.Add(pathFigure);
            path.Data = pathGeometry;
            canvas.Children.Add(path);

            canvas.Measure(new System.Windows.Size(32, 32));
            canvas.Arrange(new System.Windows.Rect(0, 0, 32, 32));

            RenderTargetBitmap rbitmap = new RenderTargetBitmap((int)canvas.ActualWidth, (int)canvas.ActualHeight, 71d, 71d, PixelFormats.Default);
            rbitmap.Render(canvas);

            MemoryStream stream = new MemoryStream();
            BitmapEncoder encoder = new BmpBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(rbitmap));
            encoder.Save(stream);
            
            Texture texture = new Texture(image: new Bitmap(stream));
            texture.NeedsUpdate = true;

            return texture;

        }
    }
}

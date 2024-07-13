using System.IO;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using THREE;

namespace THREEExample.Learning.Chapter07
{
    [Example("04.ProgramBased-Sprites", ExampleCategory.LearnThreeJS, "Chapter07")]
    public class ProgramBasedSpritesExample : ExampleTemplate
    {
        float step = 0.0f;
        Points cloud;
        public ProgramBasedSpritesExample()
        {

        }
        public override void InitCamera()
        {
            base.InitCamera();

            camera.Position.Set(20, 0, 150);
        }
        public override void Load(GLControl control)
        {
            base.Load(control);

           

            CreateParticles();

        }

        public override void Render()
        {
            step += 0.01f;
            cloud.Rotation.X = step;
            cloud.Rotation.Z = step;

            base.Render();
        }

        private void CreateParticles()
        {
            Random random = new Random();

            var geom = new THREE.Geometry();

            var material = new PointsMaterial() {Size=15,Transparent=true,Opacity=0.6f,Color=Color.Hex(0xffffff),SizeAttenuation=true,Map=GetTexture() };
            
            material.Rotation = (float)Math.PI;

            int range = 500;
            for(int i = 0; i < 5000; i++)
            {
                var particle = new Vector3((float)random.NextDouble() * range - range / 2.0f, (float)random.NextDouble() * range - range / 2.0f, (float)random.NextDouble() * range - range / 2.0f);
                geom.Vertices.Add(particle);               
            }

            cloud = new Points(geom, material);

            scene.Add(cloud);
        }

        private Texture GetTexture()
        {
            Canvas canvas = new Canvas();

            canvas.Width = 32;
            canvas.Height = 32;



            //canvas.Background = System.Windows.Media.Brushes.White;
            // the body
            //canvas.TranslatePoint(new System.Windows.Point(-81, -84),canvas);

    
            var path = new System.Windows.Shapes.Path();
            Canvas.SetTop(path, -84);
            Canvas.SetLeft(path, -81);
            path.Fill = System.Windows.Media.Brushes.Orange;

            var pathGeometry = new System.Windows.Media.PathGeometry();              
            var pathFigure = new System.Windows.Media.PathFigure();         
          

            pathFigure.StartPoint = new System.Windows.Point(83, 116);
            pathFigure.Segments.Add(new System.Windows.Media.LineSegment() { Point = new System.Windows.Point(83, 102) });
            pathFigure.Segments.Add(new BezierSegment() { Point1= new System.Windows.Point(83, 94) ,Point2 = new System.Windows.Point(89, 88) ,Point3= new System.Windows.Point(97, 88) });
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
            pathFigure.Segments.Add(new ArcSegment() { Size= new System.Windows.Size(2.5,2.5), Point = new System.Windows.Point(102, 103),RotationAngle=360,IsLargeArc=true });
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

            canvas.Measure(new System.Windows.Size(32,32));
            canvas.Arrange(new System.Windows.Rect(0,0,32,32));

            RenderTargetBitmap rbitmap = new RenderTargetBitmap((int)canvas.ActualWidth, (int)canvas.ActualHeight, 71d, 71d, PixelFormats.Default);
            rbitmap.Render(canvas);

            MemoryStream stream = new MemoryStream();
            BitmapEncoder encoder = new BmpBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(rbitmap));
            //encoder.Save(stream);

            //using(var fs = System.IO.File.OpenWrite("sprite.png"))
            //{
            //    encoder.Save(fs);
            //}
            //Texture texture = new Texture(image: new Bitmap(stream));
            var texture = TextureLoader.Load(@"../../../../assets\textures\particles\sprite.png"); 
            return texture;
           
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using THREE;
using THREE.Cameras;
using THREE.Core;
using THREE.Extras.Core;
using THREE.Geometries;
using THREE.Lights;
using THREE.Loaders;
using THREE.Materials;
using THREE.Math;
using THREE.Objects;
using THREE.Textures;
using Vector3 = THREE.Math.Vector3;

namespace THREEExample.Three.Geometries
{
    [Example("shapes", ExampleCategory.ThreeJs, "geometry")]
    public class GeometryShapeExample : ExampleTemplate
    {
        Group group = new Group();

        float targetRotation = 0;
        float targetRotationOnPointerDown = 0;

        float pointerX = 0;
        float pointerXOnPointerDown = 0;

        float windowHalfX;
        Texture texture;

        public GeometryShapeExample() : base()
        {
            scene.Background = Color.Hex(0xf0f0f0);
        }
        public override void InitCamera()
        {
            camera = new PerspectiveCamera(50, glControl.AspectRatio, 1, 1000);
            camera.Position.Set(0, 150, 500);
            scene.Add(camera);
        }
        public override void InitLighting()
        {
            var light = new PointLight(0xffffff, 0.8f);
            camera.Add(light);
        }

        public override void Init()
        {
            base.Init();

            windowHalfX = glControl.Width / 2;

            group.Position.Y = 50;
            scene.Add(group);

            texture = TextureLoader.Load(@"../../../assets/textures/uv_grid_opengl.jpg");
            texture.WrapS = texture.WrapS = Constants.RepeatWrapping;
            texture.Repeat.Set(0.008f, 0.008f);

            InitGeometry();

            glControl.MouseDown += OnMouseDown;
            glControl.MouseUp += OnMouseUp;

        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            pointerX = e.X - windowHalfX;
            targetRotationOnPointerDown = targetRotation;
            glControl.MouseMove += OnMouseMove;
            glControl.MouseUp += OnMouseUp;

        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            glControl.MouseMove -= OnMouseMove;
            glControl.MouseUp -= OnMouseUp;
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            pointerX = e.X - windowHalfX;
            targetRotation = targetRotationOnPointerDown + (pointerX - pointerXOnPointerDown) * 0.02f;
        }

        public override void Render()
        {
            group.Rotation.Y += (targetRotation - group.Rotation.Y) * 0.05f;
            renderer.Render(scene, camera);
        }


        public override void Resize(System.Drawing.Size clientSize)
        {
            windowHalfX = glControl.Width / 2;
            base.Resize(clientSize);
          
        }


        private void InitGeometry()
        {
            // California

            var californiaPts = new List<Vector3>();

            californiaPts.Add(new Vector3(610, 320, 0));
            californiaPts.Add(new Vector3(450, 300, 0));
            californiaPts.Add(new Vector3(392, 392, 0));
            californiaPts.Add(new Vector3(266, 438, 0));
            californiaPts.Add(new Vector3(190, 570, 0));
            californiaPts.Add(new Vector3(190, 600, 0));
            californiaPts.Add(new Vector3(160, 620, 0));
            californiaPts.Add(new Vector3(160, 650, 0));
            californiaPts.Add(new Vector3(180, 640, 0));
            californiaPts.Add(new Vector3(165, 680, 0));
            californiaPts.Add(new Vector3(150, 670, 0));
            californiaPts.Add(new Vector3(90, 737, 0));
            californiaPts.Add(new Vector3(80, 795, 0));
            californiaPts.Add(new Vector3(50, 835, 0));
            californiaPts.Add(new Vector3(64, 870, 0));
            californiaPts.Add(new Vector3(60, 945, 0));
            californiaPts.Add(new Vector3(300, 945, 0));
            californiaPts.Add(new Vector3(300, 743, 0));
            californiaPts.Add(new Vector3(600, 473, 0));
            californiaPts.Add(new Vector3(626, 425, 0));
            californiaPts.Add(new Vector3(600, 370, 0));
            californiaPts.Add(new Vector3(610, 320, 0));

            for (var i = 0; i < californiaPts.Count; i++) californiaPts[i].MultiplyScalar(0.25f);

            var californiaShape = new Shape(californiaPts);


            // Triangle

            var triangleShape = new Shape()
                .MoveTo(80, 20)
                .LineTo(40, 80)
                .LineTo(120, 80)
                .LineTo(80, 20); // close path


            // Heart

            var x = 0;
            var y = 0;

            var heartShape = new Shape() // From http://blog.burlock.org/html5/130-paths
                .MoveTo(x + 25, y + 25)
                .BezierCurveTo(x + 25, y + 25, x + 20, y, x, y)
                .BezierCurveTo(x - 30, y, x - 30, y + 35, x - 30, y + 35)
                .BezierCurveTo(x - 30, y + 55, x - 10, y + 77, x + 25, y + 95)
                .BezierCurveTo(x + 60, y + 77, x + 80, y + 55, x + 80, y + 35)
                .BezierCurveTo(x + 80, y + 35, x + 80, y, x + 50, y)
                .BezierCurveTo(x + 35, y, x + 25, y + 25, x + 25, y + 25);


            // Square

            var sqLength = 80;

            var squareShape = new Shape()
                .MoveTo(0, 0)
                .LineTo(0, sqLength)
                .LineTo(sqLength, sqLength)
                .LineTo(sqLength, 0)
                .LineTo(0, 0);

            // Rounded rectangle

            var roundedRectShape = new Shape();

            roundRect(roundedRectShape, 0, 0, 50, 50, 20);


            // Track

            var trackShape = new Shape()
                .MoveTo(40, 40)
                .LineTo(40, 160)
                .AbsArc(60, 160, 20, (float)Math.PI, 0, true)
                .LineTo(80, 40)
                .AbsArc(60, 40, 20, 2 * (float)Math.PI, (float)Math.PI, true);


            // Circle

            var circleRadius = 40;
            var circleShape = new Shape()
                .MoveTo(0, circleRadius)
                .QuadraticCurveTo(circleRadius, circleRadius, circleRadius, 0)
                .QuadraticCurveTo(circleRadius, -circleRadius, 0, -circleRadius)
                .QuadraticCurveTo(-circleRadius, -circleRadius, -circleRadius, 0)
                .QuadraticCurveTo(-circleRadius, circleRadius, 0, circleRadius);


            // Fish

            x = y = 0;

            var fishShape = new Shape()
                .MoveTo(x, y)
                .QuadraticCurveTo(x + 50, y - 80, x + 90, y - 10)
                .QuadraticCurveTo(x + 100, y - 10, x + 115, y - 40)
                .QuadraticCurveTo(x + 115, y, x + 115, y + 40)
                .QuadraticCurveTo(x + 100, y + 10, x + 90, y + 10)
                .QuadraticCurveTo(x + 50, y + 80, x, y);


            // Arc circle

            var arcShape = new Shape()
                .MoveTo(50, 10)
                .AbsArc(10, 10, 40, 0, (float)Math.PI * 2, false);

            var holePath = new Path()
                .MoveTo(20, 10)
                .AbsArc(10, 10, 10, 0, (float)Math.PI * 2, true);

            (arcShape as Shape).Holes.Add(holePath);


            // Smiley

            var smileyShape = new Shape()
                .MoveTo(80, 40)
                .AbsArc(40, 40, 40, 0, (float)Math.PI * 2, false);

            var smileyEye1Path = new Path()
                .MoveTo(35, 20)
                .AbsEllipse(25, 20, 10, 10, 0, (float)Math.PI * 2, true);

            var smileyEye2Path = new Path()
                .MoveTo(65, 20)
                .AbsArc(55, 20, 10, 0, (float)Math.PI * 2, true);

            var smileyMouthPath = new Path()
                .MoveTo(20, 40)
                .QuadraticCurveTo(40, 60, 60, 40)
                .BezierCurveTo(70, 45, 70, 50, 60, 60)
                .QuadraticCurveTo(40, 80, 20, 60)
                .QuadraticCurveTo(5, 50, 20, 40);

            (smileyShape as Shape).Holes.Add(smileyEye1Path);
            (smileyShape as Shape).Holes.Add(smileyEye2Path);
            (smileyShape as Shape).Holes.Add(smileyMouthPath);


            // Spline shape

            var splinepts = new List<Vector3>();
            splinepts.Add(new Vector3(70, 20, 0));
            splinepts.Add(new Vector3(80, 90, 0));
            splinepts.Add(new Vector3(-30, 70, 0));
            splinepts.Add(new Vector3(0, 0, 0));

            var splineShape = new Shape()
                .MoveTo(0, 0)
                .SplineThru(splinepts);

            var extrudeSettings = new Hashtable { { "depth", 8 }, { "bevelEnabled", true }, { "bevelSegments", 2 }, { "steps", 2 }, { "bevelSize", 1.0f }, { "bevelThickness", 1 } };

            // addShape( shape, color, x, y, z, rx, ry,rz, s );

            AddShape(californiaShape, extrudeSettings, 0xf08000, -300, -100, 0, 0, 0, 0, 1);
            AddShape(triangleShape as Shape, extrudeSettings, 0x8080f0, -180, 0, 0, 0, 0, 0, 1);
            AddShape(roundedRectShape, extrudeSettings, 0x008000, -150, 150, 0, 0, 0, 0, 1);
            AddShape(trackShape as Shape, extrudeSettings, 0x008080, 200, -100, 0, 0, 0, 0, 1);
            AddShape(squareShape as Shape, extrudeSettings, 0x0040f0, 150, 100, 0, 0, 0, 0, 1);
            AddShape(heartShape as Shape, extrudeSettings, 0xf00000, 60, 100, 0, 0, 0, (float)Math.PI, 1);
            AddShape(circleShape as Shape, extrudeSettings, 0x00f000, 120, 250, 0, 0, 0, 0, 1);
            AddShape(fishShape as Shape, extrudeSettings, 0x404040, -60, 200, 0, 0, 0, 0, 1);
            AddShape(smileyShape as Shape, extrudeSettings, 0xf000f0, -200, 250, 0, 0, 0, (float)Math.PI, 1);
            AddShape(arcShape as Shape, extrudeSettings, 0x804000, 150, 0, 0, 0, 0, 0, 1);
            AddShape(splineShape as Shape, extrudeSettings, 0x808080, -50, -100, 0, 0, 0, 0, 1);

            AddLineShape((arcShape as Shape).Holes[0], 0x804000, 150, 0, 0, 0, 0, 0, 1);

            for (var i = 0; i < (smileyShape as Shape).Holes.Count; i += 1)
            {

                AddLineShape((smileyShape as Shape).Holes[i], 0xf000f0, -200, 250, 0, 0, 0, (float)Math.PI, 1);

            }
        }

        private void roundRect(Shape ctx,float x,float y,float width,float height,float radius)
        {
            ctx.MoveTo(x, y + radius,0);
            ctx.LineTo(x, y + height - radius,0);
            ctx.QuadraticCurveTo(x, y + height, x + radius, y + height);
            ctx.LineTo(x + width - radius, y + height,0);
            ctx.QuadraticCurveTo(x + width, y + height, x + width, y + height - radius);
            ctx.LineTo(x + width, y + radius,0);
            ctx.QuadraticCurveTo(x + width, y, x + width - radius, y);
            ctx.LineTo(x + radius, y,0);
            ctx.QuadraticCurveTo(x, y, x, y + radius);
        }
        private void AddShape(Shape shape, Hashtable extrudeSettings, int color, float x, float y, float z, float rx, float ry, float rz, float s)
        {
            // flat shape with texture
            // note: default UVs generated by THREE.ShapeBufferGeometry are simply the x- and y-coordinates of the vertices

            var geometry = new ShapeBufferGeometry(shape);

            var mesh = new Mesh(geometry, new MeshPhongMaterial() { Side = Constants.DoubleSide, Map = texture });
            mesh.Position.Set(x, y, z - 175);
            mesh.Rotation.Set(rx, ry, rz);
            mesh.Scale.Set(s, s, s);
            group.Add(mesh);

            // flat shape

            var shapeGeometry = new ShapeBufferGeometry(shape);

            mesh = new Mesh(shapeGeometry, new MeshPhongMaterial() { Color = Color.Hex(color), Side = Constants.DoubleSide });
            mesh.Position.Set(x, y, z - 125);
            mesh.Rotation.Set(rx, ry, rz);
            mesh.Scale.Set(s, s, s);
            group.Add(mesh);

            // extruded shape

            var extrudeGeometry = new ExtrudeBufferGeometry(shape, extrudeSettings);

            mesh = new Mesh(extrudeGeometry, new MeshPhongMaterial() { Color = Color.Hex(color) });
            mesh.Position.Set(x, y, z - 75);
            mesh.Rotation.Set(rx, ry, rz);
            mesh.Scale.Set(s, s, s);
            group.Add(mesh);

            AddLineShape(shape, color, x, y, z, rx, ry, rz, s);
        }
        private void AddLineShape(Path shape, int color, float x, float y, float z, float rx, float ry, float rz, float s)
        {
            // lines

            shape.AutoClose = true;

            var points = shape.GetPoints();
            var spacedPoints = shape.GetSpacedPoints(50);

            var geometryPoints = new BufferGeometry().SetFromPoints(points);
            var geometrySpacedPoints = new BufferGeometry().SetFromPoints(spacedPoints);

            // solid line

            var line = new Line(geometryPoints, new LineBasicMaterial() { Color = Color.Hex(color) });
            line.Position.Set(x, y, z - 25);
            line.Rotation.Set(rx, ry, rz);
            line.Scale.Set(s, s, s);
            group.Add(line);

            // line from equidistance sampled points

            line = new Line(geometrySpacedPoints, new LineBasicMaterial() { Color = Color.Hex(color) });
            line.Position.Set(x, y, z + 25);
            line.Rotation.Set(rx, ry, rz);
            line.Scale.Set(s, s, s);
            group.Add(line);

            // vertices from real points

            var particles = new Points(geometryPoints, new PointsMaterial() { Color = Color.Hex(color), Size = 4 });
            particles.Position.Set(x, y, z + 75);
            particles.Rotation.Set(rx, ry, rz);
            particles.Scale.Set(s, s, s);
            group.Add(particles);

            // equidistance sampled points

            particles = new Points(geometrySpacedPoints, new PointsMaterial() { Color = Color.Hex(color), Size = 4 });
            particles.Position.Set(x, y, z + 125);
            particles.Rotation.Set(rx, ry, rz);
            particles.Scale.Set(s, s, s);
            group.Add(particles);
        }
    }
}

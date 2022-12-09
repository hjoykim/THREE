using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using THREE;
using THREE.Cameras;
using THREE.Core;
using THREE.Extras.Core;
using THREE.Geometries;
using THREE.Loaders;
using THREE.Materials;
using THREE.Math;
using THREE.Objects;

namespace THREEExample.Three.Geometries
{
	[Example("Text Shapes",ExampleCategory.ThreeJs,"geometry")]
    public class GeometryTextShapeExample : ExampleTemplate
    {
        public GeometryTextShapeExample() : base() 
        {
            scene.Background = Color.Hex(0xf0f0f0);
        }

        public override void InitCamera()
        {
            camera = new PerspectiveCamera(45, glControl.AspectRatio, 1, 10000);
            camera.Position.Set(0, -400, 600);
        }
        public override void Init()
        {
            base.Init();

            InitTextShape();
        }
		private void InitTextShape()
		{
			var font = Font.Load(@"../../../assets/fonts/helvetiker_regular.typeface.json");

			float xMid;
			Mesh text;

			var color = Color.Hex(0x006699);

			var matDark = new LineBasicMaterial()
			{
				Color = color,
				Side = Constants.DoubleSide
			};

			var matLite = new MeshBasicMaterial()
			{
				Color = color,
				Transparent = true,
				Opacity = 0.4f,
				Side = Constants.DoubleSide
			};

			var message = "   Three.js\nSimple text.";

			var shapes = font.GenerateShapes(message, 100);

			var geometry = new ShapeBufferGeometry(shapes);

			geometry.ComputeBoundingBox();

			xMid = -0.5f * (geometry.BoundingBox.Max.X - geometry.BoundingBox.Min.X);

			geometry.Translate(xMid, 0, 0);

			// make shape ( N.B. edge view not visible )

			text = new Mesh(geometry, matLite);
			text.Position.Z = -150;
			scene.Add(text);

			// make line shape ( N.B. edge view remains visible )

			var holeShapes = new List<Path>();

			for (var i = 0; i < shapes.Count; i++)
			{

				var shape = shapes[i];

				if (shape.Holes != null && shape.Holes.Count > 0)
				{

					for (var j = 0; j < shape.Holes.Count; j++)
					{

						var hole = shape.Holes[j];
						holeShapes.Add(hole);

					}

				}

			}
			List<Path> allShapes = new List<Path>(shapes);
			allShapes = allShapes.Concat(holeShapes);

			var lineText = new Object3D();

			for (var i = 0; i < allShapes.Count; i++)
			{

				var shape = allShapes[i];

				var points = shape.GetPoints();
				var shapeGeometry = new BufferGeometry().SetFromPoints(points);

				shapeGeometry.Translate(xMid, 0, 0);

				var lineMesh = new Line(shapeGeometry, matDark);
				lineText.Add(lineMesh);

			}

			scene.Add(lineText);
		}
    }
}

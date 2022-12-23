using System.Collections;
using System.Collections.Generic;


namespace THREE
{
	public class ShapePath
	{

		public List<Path> SubPaths = new List<Path>();

		public Path CurrentPath = null;

		public Color Color;

		public string Type;
		public ShapePath()
		{
			this.Type = "ShapePath";

			this.Color = Color.Hex(0x000000);

		}

		public ShapePath MoveTo(float x, float y)
		{
			CurrentPath = new Path();
			SubPaths.Add(CurrentPath);
			CurrentPath.MoveTo(x, y, 0);

			return this;
		}
		public ShapePath LineTo(float x, float y)
		{

			this.CurrentPath.LineTo(x, y, 0);

			return this;

		}

		public ShapePath QuadraticCurveTo(float aCPx, float aCPy, float aX, float aY)
		{

			CurrentPath.QuadraticCurveTo(aCPx, aCPy, aX, aY);

			return this;

		}

		public ShapePath BezierCurveTo(float aCP1x, float aCP1y, float aCP2x, float aCP2y, float aX, float aY)
		{

			CurrentPath.BezierCurveTo(aCP1x, aCP1y, aCP2x, aCP2y, aX, aY);

			return this;

		}

		public ShapePath SplineThru(List<Vector3> pts)
		{

			CurrentPath.SplineThru(pts);

			return this;

		}
		private List<Shape> ToShapesNoHoles(List<Path> inSubpaths)
		{

			var shapes = new List<Shape>();

			for (int i = 0; i < inSubpaths.Count; i++)
			{

				var tmpPath = inSubpaths[i];

				var tmpShape = new Shape();
				tmpShape.Curves = tmpPath.Curves;

				shapes.Add(tmpShape);

			}

			return shapes;

		}

		private bool IsPointInsidePolygon(Vector3 inPt, List<Vector3> inPolygon)
		{

			var polyLen = inPolygon.Count;

			// inPt on polygon contour => immediate success    or
			// toggling of inside/outside at every single! intersection point of an edge
			//  with the horizontal line through inPt, left of inPt
			//  not counting lowerY endpoints of edges and whole edges on that line
			var inside = false;
			for (int p = polyLen - 1, q = 0; q < polyLen; p = q++)
			{

				var edgeLowPt = inPolygon[p];
				var edgeHighPt = inPolygon[q];

				var edgeDx = edgeHighPt.X - edgeLowPt.X;
				var edgeDy = edgeHighPt.Y - edgeLowPt.Y;

				if (System.Math.Abs(edgeDy) > float.Epsilon)
				{

					// not parallel
					if (edgeDy < 0)
					{

						edgeLowPt = inPolygon[q]; edgeDx = -edgeDx;
						edgeHighPt = inPolygon[p]; edgeDy = -edgeDy;

					}
					if ((inPt.Y < edgeLowPt.Y) || (inPt.Y > edgeHighPt.Y)) continue;

					if (inPt.Y == edgeLowPt.Y)
					{

						if (inPt.Z == edgeLowPt.X) return true;        // inPt is on contour ?
																	   // continue;				// no intersection or edgeLowPt => doesn't count !!!

					}
					else
					{

						var perpEdge = edgeDy * (inPt.X - edgeLowPt.X) - edgeDx * (inPt.Y - edgeLowPt.Y);
						if (perpEdge == 0) return true;        // inPt is on contour ?
						if (perpEdge < 0) continue;
						inside = !inside;       // true intersection left of inPt

					}

				}
				else
				{

					// parallel or collinear
					if (inPt.Y != edgeLowPt.Y) continue;           // parallel
																   // edge lies on the same horizontal line as inPt
					if (((edgeHighPt.X <= inPt.X) && (inPt.X <= edgeLowPt.X)) ||
						 ((edgeLowPt.X <= inPt.X) && (inPt.X <= edgeHighPt.X))) return true;    // inPt: Point on contour !
																								// continue;

				}

			}

			return inside;

		}

		public List<Shape> ToShapes(bool isCCW, bool noHoles)
		{


			var subPaths = this.SubPaths;
			if (subPaths.Count == 0) return new List<Shape>();

			if (noHoles == true) return ToShapesNoHoles(subPaths);


			var shapes = new List<Shape>();
			Path tmpPath;
			Shape tmpShape;

			if (subPaths.Count == 1)
			{

				tmpPath = subPaths[0];
				tmpShape = new Shape();
				tmpShape.Curves = tmpPath.Curves;
				shapes.Add(tmpShape);
				return shapes;

			}

			var holesFirst = !ShapeUtils.IsClockWise(SubPaths[0].GetPoints());
			holesFirst = isCCW ? !holesFirst : holesFirst;

			// console.log("Holes first", holesFirst);

			var betterShapeHoles = new List<Hashtable>();
			var newShapes = new List<Hashtable>();
			var newShapeHoles = new List<Hashtable>();
			var mainIdx = 0;


			newShapes.Add(null);//[mainIdx] = undefined;
			newShapeHoles.Add(new Hashtable());

			for (int i = 0; i < SubPaths.Count; i++)
			{

				tmpPath = SubPaths[i];
				var tmpPoints = tmpPath.GetPoints();
				var solid = ShapeUtils.IsClockWise(tmpPoints);
				solid = isCCW ? !solid : solid;

				if (solid)
				{

					if ((!holesFirst) && (newShapes[mainIdx] != null)) mainIdx++;

					if ((newShapes.Count - 1) < mainIdx) newShapes.Add(null);					

					newShapes[mainIdx] = new Hashtable() { { "s", new Shape() }, { "p", tmpPoints } };

					((newShapes[mainIdx] as Hashtable)["s"] as Shape).Curves = tmpPath.Curves;

					if (holesFirst)
					{
						mainIdx++;
						
					}
					if ((newShapeHoles.Count - 1) < mainIdx) newShapeHoles.Add(null);

					newShapeHoles[mainIdx] = new Hashtable();

					//console.log('cw', i);

				}
				else
				{

					newShapeHoles[mainIdx] =
						new Hashtable()
						{
							{ mainIdx,
								new Hashtable()
								{
									{ "h", tmpPath }, { "p", tmpPoints[0] }
								}
							}
						};
					
					

					//console.log('ccw', i);

				}

			}

			// only Holes? -> probably all Shapes with wrong orientation
			if (newShapes[0] == null) return ToShapesNoHoles(SubPaths);


			if (newShapes.Count > 1) {

				var ambiguous = false;
				var toChange = new List<Hashtable>();

				for (int sIdx = 0, sLen = newShapes.Count; sIdx < sLen; sIdx++) {

					betterShapeHoles.Add(
						new Hashtable() { { sIdx, new Hashtable() } }
					);//[sIdx] = [];

				}

				for (int sIdx = 0, sLen = newShapes.Count; sIdx < sLen; sIdx++) {

					var sho = newShapeHoles[sIdx] as Hashtable;

					for (var hIdx = 0; hIdx < sho.Count; hIdx++) {

						Hashtable ho = (Hashtable)sho[hIdx];
						var hole_unassigned = true;



						for (var s2Idx = 0; s2Idx < newShapes.Count; s2Idx++)
						{
							if (IsPointInsidePolygon(ho["p"] as Vector3, (List<Vector3>)(newShapes[s2Idx] as Hashtable)["p"]))
							{

								if (sIdx != s2Idx) toChange.Add(new Hashtable() { { "froms", sIdx }, { "tos", s2Idx }, { "hole", hIdx } });
								if (hole_unassigned)
								{

									hole_unassigned = false;
									betterShapeHoles.Add(new Hashtable() { { s2Idx, ho } });

								}
								else
								{

									ambiguous = true;

								}

							}

						}
						
						if (hole_unassigned ) {

							betterShapeHoles.Add(new Hashtable() { { sIdx, ho } });

						}

					}

				}
				// console.log("ambiguous: ", ambiguous);
				if (toChange.Count > 0 ) {

					// console.log("to change: ", toChange);
					if ( ! ambiguous )	newShapeHoles = betterShapeHoles;

				}

			}


			for (int i = 0, il = newShapes.Count; i<il; i ++ ) {

				tmpShape = (newShapes[i] as Hashtable)["s"] as Shape;
				shapes.Add(tmpShape );
				Hashtable tmpHoles = (Hashtable)newShapeHoles[i];

				for (int j = 0, jl = tmpHoles.Count; j<jl; j ++ ) 
				{
					if(tmpHoles[j] is Hashtable)
						tmpShape.Holes.Add((Path)(tmpHoles[j] as Hashtable)["h"]);

				}

			}

			//console.log("shape", shapes);

			return shapes;
		}
	}
}

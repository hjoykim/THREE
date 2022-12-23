using System;
using System.Collections;
using System.Collections.Generic;


namespace THREE
{
   
    public class ConvexHull
    {
		public class VertexNode
		{
			public Vector3 Point = null;

			public VertexNode Prev = null;

			public VertexNode Next = null;

			public Face Face = null;

			public VertexNode(Vector3 point)
			{
				this.Point = point;

			}
		}

		public class VertexList
		{
			public VertexNode Head;

			public VertexNode Tail;

			public VertexList()
			{
				this.Head = null;
				this.Tail = null;
			}

			public VertexNode First()
			{
				return Head;
			}

			public VertexNode Last()
			{
				return Tail;
			}

			public void Clear()
			{
				Head = null;
				Tail = null;
			}

			public VertexList InsertBefore(VertexNode target, VertexNode vertex)
			{
				vertex.Prev = target.Prev;
				vertex.Next = target;

				if (vertex.Prev == null)
					this.Head = vertex;
				else
					vertex.Prev.Next = vertex;

				target.Prev = vertex;

				return this;
			}

			public VertexList InsertAfter(VertexNode target, VertexNode vertex)
			{

				vertex.Prev = target;
				vertex.Next = target.Next;

				if (vertex.Next == null)
				{

					this.Tail = vertex;

				}
				else
				{

					vertex.Next.Prev = vertex;

				}

				target.Next = vertex;

				return this;

			}

			// Appends a vertex to the end of the linked list

			public VertexList Append(VertexNode vertex)
			{

				if (this.Head == null)
				{

					this.Head = vertex;

				}
				else
				{

					this.Tail.Next = vertex;

				}

				vertex.Prev = this.Tail;
				vertex.Next = null; // the tail has no subsequent vertex

				this.Tail = vertex;

				return this;

			}

			// Appends a chain of vertices where 'vertex' is the head.

			public VertexList AppendChain(VertexNode vertex)
			{

				if (this.Head == null)
				{

					this.Head = vertex;

				}
				else
				{

					this.Tail.Next = vertex;

				}

				vertex.Prev = this.Tail;

				// ensure that the 'tail' reference points to the last vertex of the chain

				while (vertex.Next != null)
				{

					vertex = vertex.Next;

				}

				this.Tail = vertex;

				return this;

			}

			// Removes a vertex from the linked list

			public VertexList Remove(VertexNode vertex)
			{

				if (vertex.Prev == null)
				{

					this.Head = vertex.Next;

				}
				else
				{

					vertex.Prev.Next = vertex.Next;

				}

				if (vertex.Next == null)
				{

					this.Tail = vertex.Prev;

				}
				else
				{

					vertex.Next.Prev = vertex.Prev;

				}

				return this;

			}

			// Removes a list of vertices whose 'head' is 'a' and whose 'tail' is b

			public VertexList RemoveSubList(VertexNode a, VertexNode b)
			{

				if (a.Prev == null)
				{

					this.Head = b.Next;

				}
				else
				{

					a.Prev.Next = b.Next;

				}

				if (b.Next == null)
				{

					this.Tail = a.Prev;

				}
				else
				{

					b.Next.Prev = a.Prev;

				}

				return this;

			}

			public bool IsEmpty()
			{

				return this.Head == null;

			}


		}
		public class Face
		{
			public Vector3 Normal;

			public Vector3 Midpoint;

			public float Area = 0;

			public float Constant = 0;

			public VertexNode Outside = null;

			public int Visible;

			public int Mark;

			public HalfEdge Edge = null;

			public Face()
			{
				Normal = new Vector3();

				Midpoint = new Vector3();

			}
			public static Face Create(VertexNode a, VertexNode b, VertexNode c)
			{
				var face = new Face();

				var e0 = new HalfEdge(a, face);
				var e1 = new HalfEdge(b, face);
				var e2 = new HalfEdge(c, face);

				e0.Next = e2.Prev = e1;
				e1.Next = e0.Prev = e2;
				e2.Next = e1.Prev = e0;

				face.Edge = e0;

				return face.Compute();
			}
			public HalfEdge GetEdge(int i)
			{

				var edge = this.Edge;

				while (i > 0)
				{

					edge = edge.Next;
					i--;

				}

				while (i < 0)
				{

					edge = edge.Prev;
					i++;

				}

				return edge;

			}

			public Face Compute()
			{

				var triangle = new Triangle();

				var a = this.Edge.Tail();
				var b = this.Edge.Head();
				var c = this.Edge.Next.Head();

				triangle.Set(a.Point, b.Point, c.Point);

				triangle.GetNormal(this.Normal);
				triangle.GetMidpoint(this.Midpoint);
				this.Area = triangle.GetArea();

				this.Constant = this.Normal.Dot(this.Midpoint);

				return this;
			}


			public float DistanceToPoint(Vector3 point)
			{

				return this.Normal.Dot(point) - this.Constant;

			}
		}


		public class HalfEdge
		{
			public VertexNode Vertex;

			public HalfEdge Prev = null;

			public HalfEdge Next = null;

			public HalfEdge Twin = null;

			public Face Face;

			public HalfEdge(VertexNode vertex, Face face)
			{
				this.Vertex = vertex;
				this.Face = face;

			}

			public VertexNode Head()
			{
				return this.Vertex;
			}
			public VertexNode Tail()
			{
				return this.Prev != null ? this.Prev.Vertex : null;
			}

			public float Length()
			{
				var head = this.Head();
				var tail = this.Tail();

				if (tail != null)
				{
					return tail.Point.DistanceTo(head.Point);
				}

				return -1;
			}

			public float LengthSquared()
			{

				var head = this.Head();
				var tail = this.Tail();

				if (tail != null)
				{

					return tail.Point.DistanceToSquared(head.Point);

				}

				return -1;

			}

			public HalfEdge SetTwin(HalfEdge edge)
			{

				this.Twin = edge;
				edge.Twin = this;

				return this;

			}
		}
		int Visible = 0;
        int Deleted = 1;

        float tolerance = -1;

        public List<Face> faces = new List<Face>();
        public List<Face> newFaces = new List<Face>();

        private Vector3 v1 = new Vector3();

        public VertexList assigned = new VertexList();
        public VertexList unassigned = new VertexList();

        public List<VertexNode> vertices = new List<VertexNode>();

        public ConvexHull()
        {

        }
		public ConvexHull SetFromPoints(Vector3[] points )
		{
			if (points.Length < 4)
			{

				throw new Exception("THREE.ConvexHull: The algorithm needs at least four points.");

			}

			this.MakeEmpty();

			for (int i = 0;i< points.Length;i++)
			{

				this.vertices.Add(new VertexNode(points[i]));

			}

			this.Compute();

			return this;

		}

		public ConvexHull SetFromObject(Object3D object3D)
		{

			List<Vector3> points = new List<Vector3>();

			object3D.UpdateMatrixWorld(true);

			object3D.Traverse(node=> {

				Vector3 point;

				var geometry = node.Geometry;

				if (geometry != null)
				{

					if (geometry.type=="Geometry")
					{

						var vertices = geometry.Vertices;

						for (int i = 0;i< vertices.Count;  i++)
						{

							point = (Vector3)vertices[i].Clone();
							point.ApplyMatrix4(node.MatrixWorld);

							points.Add(point);

						}

					}
					else if (geometry.type=="BufferGeometry")
					{

						var attribute = ((geometry as BufferGeometry).Attributes["position"]) as BufferAttribute<float>;

						if (attribute != null)
						{

							for (int i = 0;i< attribute.count;i++)
							{

								point = new Vector3();

								point.FromBufferAttribute(attribute, i).ApplyMatrix4(node.MatrixWorld);

								points.Add(point);

							}

						}

					}

				}

			} );

			return this.SetFromPoints(points.ToArray());

		}

		public bool ContainsPoint(Vector3 point )
		{

			var faces = this.faces;

			for (int i = 0;i< faces.Count;i++)
			{

				var face = faces[i];

				// compute signed distance and check on what half space the point lies

				if (face.DistanceToPoint(point) > this.tolerance) return false;

			}

			return true;

		}
/*
		intersectRay: function(ray, target )
		{

			// based on "Fast Ray-Convex Polyhedron Intersection"  by Eric Haines, GRAPHICS GEMS II

			var faces = this.faces;

			var tNear = -Infinity;
			var tFar = Infinity;

			for (var i = 0, l = faces.length; i < l; i++)
			{

				var face = faces[i];

				// interpret faces as planes for the further computation

				var vN = face.distanceToPoint(ray.origin);
				var vD = face.normal.dot(ray.direction);

				// if the origin is on the positive side of a plane (so the plane can "see" the origin) and
				// the ray is turned away or parallel to the plane, there is no intersection

				if (vN > 0 && vD >= 0) return null;

				// compute the distance from the ray’s origin to the intersection with the plane

				var t = (vD !== 0) ? (-vN / vD) : 0;

				// only proceed if the distance is positive. a negative distance means the intersection point
				// lies "behind" the origin

				if (t <= 0) continue;

				// now categorized plane as front-facing or back-facing

				if (vD > 0)
				{

					//  plane faces away from the ray, so this plane is a back-face

					tFar = Math.min(t, tFar);

				}
				else
				{

					// front-face

					tNear = Math.max(t, tNear);

				}

				if (tNear > tFar)
				{

					// if tNear ever is greater than tFar, the ray must miss the convex hull

					return null;

				}

			}

			// evaluate intersection point

			// always try tNear first since its the closer intersection point

			if (tNear !== -Infinity)
			{

				ray.at(tNear, target);

			}
			else
			{

				ray.at(tFar, target);

			}

			return target;

		},

		intersectsRay: function(ray )
		{

			return this.intersectRay(ray, v1) !== null;

		},
*/
		public ConvexHull MakeEmpty()
		{

			this.faces.Clear();
			this.vertices.Clear();

			return this;

		}

		// Adds a vertex to the 'assigned' list of vertices and assigns it to the given face

		public ConvexHull AddVertexToFace(VertexNode vertex, Face face )
		{

			vertex.Face = face;

			if (face.Outside == null)
			{

				this.assigned.Append(vertex);

			}
			else
			{

				this.assigned.InsertBefore(face.Outside, vertex);

			}

			face.Outside = vertex;

			return this;

		}

		// Removes a vertex from the 'assigned' list of vertices and from the given face

		public ConvexHull RemoveVertexFromFace(VertexNode vertex, Face face )
		{

			if (vertex == face.Outside)
			{

				// fix face.outside link

				if (vertex.Next != null && vertex.Next.Face == face)
				{

					// face has at least 2 outside vertices, move the 'outside' reference

					face.Outside = vertex.Next;

				}
				else
				{

					// vertex was the only outside vertex that face had

					face.Outside = null;

				}

			}

			this.assigned.Remove(vertex);

			return this;

		}

		// Removes all the visible vertices that a given face is able to see which are stored in the 'assigned' vertext list

		public VertexNode RemoveAllVerticesFromFace(Face face)
		{

			if (face.Outside != null)
			{

				// reference to the first and last vertex of this face

				var start = face.Outside;
				var end = face.Outside;

				while (end.Next != null && end.Next.Face == face)
				{

					end = end.Next;

				}

				this.assigned.RemoveSubList(start, end);

				// fix references

				start.Prev = end.Next = null;
				face.Outside = null;

				return start;

			}
			return null;

		}

		// Removes all the visible vertices that 'face' is able to see

		public ConvexHull DeleteFaceVertices(Face face, Face absorbingFace=null)
		{

			var faceVertices = this.RemoveAllVerticesFromFace(face);

			if (faceVertices != null)
			{

				if (absorbingFace == null)
				{

					// mark the vertices to be reassigned to some other face

					this.unassigned.AppendChain(faceVertices);


				}
				else
				{

					// if there's an absorbing face try to assign as many vertices as possible to it

					var vertex = faceVertices;

					do
					{

						// we need to buffer the subsequent vertex at this point because the 'vertex.next' reference
						// will be changed by upcoming method calls

						var nextVertex = vertex.Next;

						var distance = absorbingFace.DistanceToPoint(vertex.Point);

						// check if 'vertex' is able to see 'absorbingFace'

						if (distance > this.tolerance)
						{

							this.AddVertexToFace(vertex, absorbingFace);

						}
						else
						{

							this.unassigned.Append(vertex);

						}

						// now assign next vertex

						vertex = nextVertex;

					} while (vertex != null);

				}

			}

			return this;

		}

		// Reassigns as many vertices as possible from the unassigned list to the new faces

		public ConvexHull ResolveUnassignedPoints(Face[] newFaces)
		{

			if (this.unassigned.IsEmpty() == false)
			{

				var vertex = this.unassigned.First();

				do
				{

					// buffer 'next' reference, see .deleteFaceVertices()

					var nextVertex = vertex.Next;

					var maxDistance = this.tolerance;

					Face maxFace = null;

					for (var i = 0; i < newFaces.Length; i++)
					{

						var face = newFaces[i];

						if (face.Mark == Visible)
						{

							var distance = face.DistanceToPoint(vertex.Point);

							if (distance > maxDistance)
							{

								maxDistance = distance;
								maxFace = face;

							}

							if (maxDistance > 1000 * this.tolerance) break;

						}

					}

					// 'maxFace' can be null e.g. if there are identical vertices

					if (maxFace != null)
					{

						this.AddVertexToFace(vertex, maxFace);

					}

					vertex = nextVertex;

				} while (vertex != null);

			}

			return this;

		}

		// Computes the extremes of a simplex which will be the initial hull

		public Hashtable ComputeExtremes()
		{

			var min = new Vector3();
			var max = new Vector3();

			var minVertices = new List<VertexNode>();
			var maxVertices = new List<VertexNode>();

			int i, l, j;

			// initially assume that the first vertex is the min/max

			for (i = 0; i < 3; i++)
			{

				minVertices.Add(this.vertices[0]);
				maxVertices.Add(this.vertices[0]);

			}

			min.Copy(this.vertices[0].Point);
			max.Copy(this.vertices[0].Point);

			// compute the min/max vertex on all six directions

			for (i = 0;i< this.vertices.Count;i++)
			{

				var vertex = this.vertices[i];
				var point = vertex.Point;

				// update the min coordinates

				for (j = 0; j < 3; j++)
				{

					if (point.GetComponent(j) < min.GetComponent(j))
					{

						min.SetComponent(j, point.GetComponent(j));
						minVertices[j] = vertex;

					}

				}

				// update the max coordinates

				for (j = 0; j < 3; j++)
				{

					if (point.GetComponent(j) > max.GetComponent(j))
					{

						max.SetComponent(j, point.GetComponent(j));
						maxVertices[j] = vertex;

					}

				}

			}

			// use min/max vectors to compute an optimal epsilon
			
			this.tolerance = 3 * float.Epsilon * (
				System.Math.Max(System.Math.Abs(min.X), System.Math.Abs(max.X)) +
				System.Math.Max(System.Math.Abs(min.Y), System.Math.Abs(max.Y)) +
				System.Math.Max(System.Math.Abs(min.Z), System.Math.Abs(max.Z))

			);

			return new Hashtable(){ { "min", minVertices },{ "max", maxVertices } };

		}

		// Computes the initial simplex assigning to its faces all the points
		// that are candidates to form part of the hull

		public ConvexHull ComputeInitialHull()
		{
			Line3 line3 = new Line3();
			Plane plane = new Plane();
			Vector3 closestPoint = new Vector3();

			VertexNode vertex;

			var vertices = this.vertices;

			var extremes = this.ComputeExtremes();

			var min = (extremes["min"] as List<VertexNode>);

			var max = (extremes["max"] as List<VertexNode>);

			VertexNode v0, v1, v2, v3;
			v0 = v1 = v2 = v3 = null;
			int i, l, j;

			// 1. Find the two vertices 'v0' and 'v1' with the greatest 1d separation
			// (max.x - min.x)
			// (max.y - min.y)
			// (max.z - min.z)

			float distance, maxDistance = 0;
			int index = 0;

			for (i = 0; i < 3; i++)
			{

				distance = max[i].Point.GetComponent(i) - min[i].Point.GetComponent(i);

				if (distance > maxDistance)
				{

					maxDistance = distance;
					index = i;

				}

			}

			v0 = min[index];
			v1 = max[index];

			// 2. The next vertex 'v2' is the one farthest to the line formed by 'v0' and 'v1'

			maxDistance = 0;
			line3.Set(v0.Point, v1.Point);

			for (i = 0;i< this.vertices.Count; i++)
			{

				vertex = vertices[i];

				if (vertex != v0 && vertex != v1)
				{

					line3.ClosestPointToPoint(vertex.Point, true, closestPoint);

					distance = closestPoint.DistanceToSquared(vertex.Point);

					if (distance > maxDistance)
					{

						maxDistance = distance;
						v2 = vertex;

					}

				}

			}

			// 3. The next vertex 'v3' is the one farthest to the plane 'v0', 'v1', 'v2'

			maxDistance = -1;
			plane.SetFromCoplanarPoints(v0.Point, v1.Point, v2.Point);

			for (i = 0;i< this.vertices.Count; i++)
			{

				vertex = vertices[i];

				if (vertex != v0 && vertex != v1 && vertex != v2)
				{

					distance = System.Math.Abs(plane.DistanceToPoint(vertex.Point));

					if (distance > maxDistance)
					{

						maxDistance = distance;
						v3 = vertex;

					}

				}

			}

			List<Face> faces = null;

			if (plane.DistanceToPoint(v3.Point) < 0)
			{

				// the face is not able to see the point so 'plane.normal' is pointing outside the tetrahedron

				faces = new List<Face>(){
					Face.Create(v0, v1, v2),
					Face.Create(v3, v1, v0),
					Face.Create(v3, v2, v1),
					Face.Create(v3, v0, v2)
				};

				// set the twin edge

				for (i = 0; i < 3; i++)
				{

					j = (i + 1) % 3;

					// join face[ i ] i > 0, with the first face

					faces[i + 1].GetEdge(2).SetTwin(faces[0].GetEdge(j));

					// join face[ i ] with face[ i + 1 ], 1 <= i <= 3

					faces[i + 1].GetEdge(1).SetTwin(faces[j + 1].GetEdge(0));

				}

			}
			else
			{

				// the face is able to see the point so 'plane.normal' is pointing inside the tetrahedron

				faces = new List<Face>() { 
					Face.Create(v0, v2, v1),
					Face.Create(v3, v0, v1),
					Face.Create(v3, v1, v2),
					Face.Create(v3, v2, v0)
				};

				// set the twin edge

				for (i = 0; i < 3; i++)
				{

					j = (i + 1) % 3;

					// join face[ i ] i > 0, with the first face

					faces[i + 1].GetEdge(2).SetTwin(faces[0].GetEdge((3 - i) % 3));

					// join face[ i ] with face[ i + 1 ]

					faces[i + 1].GetEdge(0).SetTwin(faces[j + 1].GetEdge(1));

				}

			}

			// the initial hull is the tetrahedron

			for (i = 0; i < 4; i++)
			{

				this.faces.Add(faces[i]);

			}

			// initial assignment of vertices to the faces of the tetrahedron

			for (i = 0;i< vertices.Count; i++)
			{

				vertex = vertices[i];

				if (vertex != v0 && vertex != v1 && vertex != v2 && vertex != v3)
				{

					maxDistance = this.tolerance;
					Face maxFace = null;

					for (j = 0; j < 4; j++)
					{

						distance = this.faces[j].DistanceToPoint(vertex.Point);

						if (distance > maxDistance)
						{

							maxDistance = distance;
							maxFace = this.faces[j];

						}

					}

					if (maxFace != null)
					{

						this.AddVertexToFace(vertex, maxFace);

					}

				}

			}

			return this;

		}


		// Removes inactive faces

		public ConvexHull ReindexFaces()
		{

			List<Face> activeFaces = new List<Face>();

			for (var i = 0; i < this.faces.Count; i++)
			{

				var face = this.faces[i];

				if (face.Mark == Visible)
				{

					activeFaces.Add(face);

				}

			}

			this.faces = activeFaces;

			return this;

		}

		// Finds the next vertex to create faces with the current hull

		public VertexNode NextVertexToAdd()
		{

			// if the 'assigned' list of vertices is empty, no vertices are left. return with 'undefined'

			if (this.assigned.IsEmpty() == false)
			{

				VertexNode eyeVertex = null;
				float maxDistance = 0;

				// grap the first available face and start with the first visible vertex of that face

				var eyeFace = this.assigned.First().Face;
				var vertex = eyeFace.Outside;

				// now calculate the farthest vertex that face can see

				do
				{

					var distance = eyeFace.DistanceToPoint(vertex.Point);

					if (distance > maxDistance)
					{

						maxDistance = distance;
						eyeVertex = vertex;

					}

					vertex = vertex.Next;

				} while (vertex != null && vertex.Face == eyeFace);

				return eyeVertex;

			}
			return null;

		}

		// Computes a chain of half edges in CCW order called the 'horizon'.
		// For an edge to be part of the horizon it must join a face that can see
		// 'eyePoint' and a face that cannot see 'eyePoint'.

		public ConvexHull ComputeHorizon(Vector3 eyePoint, HalfEdge crossEdge, Face face, List<HalfEdge> horizon )
		{

			// moves face's vertices to the 'unassigned' vertex list

			this.DeleteFaceVertices(face);

			face.Mark = Deleted;

			HalfEdge edge;

			if (crossEdge == null)
			{

				edge = crossEdge = face.GetEdge(0);

			}
			else
			{

				// start from the next edge since 'crossEdge' was already analyzed
				// (actually 'crossEdge.twin' was the edge who called this method recursively)

				edge = crossEdge.Next;

			}

			do
			{

				var twinEdge = edge.Twin;
				var oppositeFace = twinEdge.Face;

				if (oppositeFace.Mark == Visible)
				{

					if (oppositeFace.DistanceToPoint(eyePoint) > this.tolerance)
					{

						// the opposite face can see the vertex, so proceed with next edge

						this.ComputeHorizon(eyePoint, twinEdge, oppositeFace, horizon);

					}
					else
					{

						// the opposite face can't see the vertex, so this edge is part of the horizon

						horizon.Add(edge);

					}

				}

				edge = edge.Next;

			} while (edge != crossEdge);

			return this;

		}

		// Creates a face with the vertices 'eyeVertex.point', 'horizonEdge.tail' and 'horizonEdge.head' in CCW order

		public HalfEdge AddAdjoiningFace(VertexNode eyeVertex, HalfEdge horizonEdge )
		{

			// all the half edges are created in ccw order thus the face is always pointing outside the hull

			var face = Face.Create(eyeVertex, horizonEdge.Tail(), horizonEdge.Head());

			this.faces.Add(face);

			// join face.getEdge( - 1 ) with the horizon's opposite edge face.getEdge( - 1 ) = face.getEdge( 2 )

			face.GetEdge(-1).SetTwin(horizonEdge.Twin);

			return face.GetEdge(0); // the half edge whose vertex is the eyeVertex


		}

		//  Adds 'horizon.length' faces to the hull, each face will be linked with the
		//  horizon opposite face and the face on the left/right

		public ConvexHull AddNewFaces(VertexNode eyeVertex, List<HalfEdge> horizon )
		{

			this.newFaces.Clear();

			HalfEdge firstSideEdge = null;
			HalfEdge previousSideEdge = null;

			for (var i = 0; i < horizon.Count; i++)
			{

				var horizonEdge = horizon[i];

				// returns the right side edge

				var sideEdge = this.AddAdjoiningFace(eyeVertex, horizonEdge);

				if (firstSideEdge == null)
				{

					firstSideEdge = sideEdge;

				}
				else
				{

					// joins face.getEdge( 1 ) with previousFace.getEdge( 0 )

					sideEdge.Next.SetTwin(previousSideEdge);

				}

				this.newFaces.Add(sideEdge.Face);
				previousSideEdge = sideEdge;

			}

			// perform final join of new faces

			firstSideEdge.Next.SetTwin(previousSideEdge);

			return this;

		}

		// Adds a vertex to the hull

		public ConvexHull AddVertexToHull(VertexNode eyeVertex )
		{

			var horizon = new List<HalfEdge>();

			this.unassigned.Clear();

			// remove 'eyeVertex' from 'eyeVertex.face' so that it can't be added to the 'unassigned' vertex list

			this.RemoveVertexFromFace(eyeVertex, eyeVertex.Face);

			this.ComputeHorizon(eyeVertex.Point, null, eyeVertex.Face, horizon);

			this.AddNewFaces(eyeVertex, horizon);

			// reassign 'unassigned' vertices to the new faces

			this.ResolveUnassignedPoints(this.newFaces.ToArray());

			return this;

		}

		public ConvexHull Cleanup()
		{

			this.assigned.Clear();
			this.unassigned.Clear();
			this.newFaces.Clear();

			return this;

		}

		public ConvexHull Compute()
		{

			VertexNode vertex;

			this.ComputeInitialHull();

			// add all available vertices gradually to the hull

			while ((vertex = this.NextVertexToAdd()) != null)
			{

				this.AddVertexToHull(vertex);

			}

			this.ReindexFaces();

			this.Cleanup();

			return this;

		}

	}
}

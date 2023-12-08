using System;
using System.Diagnostics;

namespace THREE
{
    
    public class Triangle :ICloneable
    {
        private static Vector3 _v0 = new Vector3();
        private static Vector3 _v1 = new Vector3();
        private static Vector3 _v2 = new Vector3();
        private static Vector3 _v3 = new Vector3();

        private Vector3 _vab = new Vector3();
        private Vector3 _vac = new Vector3();
        private Vector3 _vbc = new Vector3();
        private Vector3 _vap = new Vector3();
        private Vector3 _vbp = new Vector3();
        private Vector3 _vcp = new Vector3();

        public Vector3 a;

        public Vector3 b;

        public Vector3 c;

		public Triangle()
		{
			a = new Vector3();
			b = new Vector3();
			c = new Vector3();
		}
        public Triangle(Vector3 _a,Vector3 _b,Vector3 _c)
        {
            this.a = (_a != null) ? a : new Vector3();
            this.b = (_b != null) ? b : new Vector3();
            this.c = (_c != null) ? c : new Vector3();
        }

		public static Vector3 GetNormal(Vector3 _a, Vector3 _b, Vector3 _c, Vector3 target )
		{

			if (target == null)
			{
				target = new Vector3();
			}

			target.SubVectors(_c, _b);
			_v0.SubVectors(_a, _b);
			target.Cross(_v0);

			var targetLengthSq = target.LengthSq();
			if (targetLengthSq > 0)
			{

				return target.MultiplyScalar(1 / (float)System.Math.Sqrt(targetLengthSq));

			}

			return target.Set(0, 0, 0);

		}

	// static/instance method to calculate barycentric coordinates
	// based on: http://www.blackpawn.com/texts/pointinpoly/default.html
		public static Vector3 GetBarycoord(Vector3 point, Vector3 _a, Vector3 _b, Vector3 _c, Vector3 target)
		{

			_v0.SubVectors(_c, _a);
			_v1.SubVectors(_b, _a);
			_v2.SubVectors(point, _a);

			var dot00 = _v0.Dot(_v0);
			var dot01 = _v0.Dot(_v1);
			var dot02 = _v0.Dot(_v2);
			var dot11 = _v1.Dot(_v1);
			var dot12 = _v1.Dot(_v2);

			var denom = (dot00 * dot11 - dot01 * dot01);

			if (target == null)
			{

				target = new Vector3();

			}

			// collinear or singular triangle
			if (denom == 0)
			{

				// arbitrary location outside of triangle?
				// not sure if this is the best idea, maybe should be returning undefined
				return target.Set(-2, -1, -1);

			}

			var invDenom = 1 / denom;
			var u = (dot11 * dot02 - dot01 * dot12) * invDenom;
			var v = (dot00 * dot12 - dot01 * dot02) * invDenom;

			// barycentric coordinates must always sum to 1
			return target.Set(1 - u - v, v, u);

		}

		public bool ContainsPoint(Vector3 point, Vector3 _a, Vector3 _b, Vector3 _c)
		{

			GetBarycoord(point, _a, _b, _c, _v3);

			return (_v3.X >= 0) && (_v3.Y >= 0) && ((_v3.X + _v3.Y) <= 1);

		}

		public static Vector2 GetUV(Vector3 point, Vector3 p1, Vector3 p2, Vector3 p3, Vector2 uv1, Vector2 uv2, Vector2 uv3, Vector2 target )
		{

			GetBarycoord(point, p1, p2, p3, _v3);

			target.Set(0, 0);
			target.AddScaledVector(uv1, _v3.X);
			target.AddScaledVector(uv2, _v3.Y);
			target.AddScaledVector(uv3, _v3.Z);

			return target;

		}

		public bool IsFrontFacing(Vector3 _a, Vector3 _b, Vector3 _c, Vector3 direction )
		{

			_v0.SubVectors(_c, _b);
			_v1.SubVectors(_a, _b);

			// strictly front facing
			return (_v0.Cross(_v1).Dot(direction) < 0) ? true : false;

		}

		public Triangle Set(Vector3 _a, Vector3 _b, Vector3 _c)
		{

			this.a.Copy(_a);
			this.b.Copy(_b);
			this.c.Copy(_c);

			return this;

		}

		public Triangle SetFromPointsAndIndices(Vector3[] points, int i0, int i1, int i2 )
		{

			this.a.Copy(points[i0]);
			this.b.Copy(points[i1]);
			this.c.Copy(points[i2]);

			return this;

		}

		public object Clone()
		{
			return new Triangle(this.a, this.b, this.c);
		}

		public Triangle Copy(Triangle triangle)
		{

			this.a.Copy(triangle.a);
			this.b.Copy(triangle.b);
			this.c.Copy(triangle.c);

			return this;

		}

		public float GetArea()
		{

			_v0.SubVectors(this.c, this.b);
			_v1.SubVectors(this.a, this.b);

			return _v0.Cross(_v1).Length() * 0.5f;

		}

		public Vector3 GetMidpoint(Vector3 target )
		{

			if (target == null)
			{
				target = new Vector3();
			}

			return target.AddVectors(this.a, this.b).Add(this.c).MultiplyScalar(1 / 3);

		}

		public Vector3 GetNormal(Vector3 target )
		{

			return GetNormal(this.a, this.b, this.c, target);

		}

		public Plane GetPlane(Plane target )
		{

			if (target == null)
			{

				target = new Plane();

			}
			return target.SetFromCoplanarPoints(this.a, this.b, this.c);

		}

		public Vector3 GetBarycoord(Vector3 point, Vector3 target )
		{

			return GetBarycoord(point, this.a, this.b, this.c, target);

		}

		public Vector2 GetUV(Vector3 point, Vector2 uv1, Vector2 uv2, Vector2 uv3, Vector2 target)
		{

			return GetUV(point, this.a, this.b, this.c, uv1, uv2, uv3, target);

		}

		public bool ContainsPoint(Vector3 point )
		{

			return ContainsPoint(point, this.a, this.b, this.c);

		}

		public bool IsFrontFacing(Vector3 direction )
		{

			return IsFrontFacing(this.a, this.b, this.c, direction);

		}

		public bool IntersectsBox(Box3 box )
		{

			return box.IntersectsTriangle(this);

		}

		public Vector3 ClosestPointToPoint(Vector3 p, Vector3 target )
		{

			if (target == null)
			{

				Trace.TraceWarning("THREE.Math.Triangle: .ClosestPointToPoint() target is now required");
				target = new Vector3();

			}

			
			float v, w;

			// algorithm thanks to Real-Time Collision Detection by Christer Ericson,
			// published by Morgan Kaufmann Publishers, (c) 2005 Elsevier Inc.,
			// under the accompanying license; see chapter 5.1.5 for detailed explanation.
			// basically, we're distinguishing which of the voronoi regions of the triangle
			// the point lies in with the minimum amount of redundant computation.

			_vab.SubVectors(b, a);
			_vac.SubVectors(c, a);
			_vap.SubVectors(p, a);
			var d1 = _vab.Dot(_vap);
			var d2 = _vac.Dot(_vap);
			if (d1 <= 0 && d2 <= 0)
			{

				// vertex region of A; barycentric coords (1, 0, 0)
				return target.Copy(a);

			}

			_vbp.SubVectors(p, b);
			var d3 = _vab.Dot(_vbp);
			var d4 = _vac.Dot(_vbp);
			if (d3 >= 0 && d4 <= d3)
			{

				// vertex region of B; barycentric coords (0, 1, 0)
				return target.Copy(b);

			}

			var vc = d1 * d4 - d3 * d2;
			if (vc <= 0 && d1 >= 0 && d3 <= 0)
			{

				v = d1 / (d1 - d3);
				// edge region of AB; barycentric coords (1-v, v, 0)
				return target.Copy(a).AddScaledVector(_vab, v);

			}

			_vcp.SubVectors(p, c);
			var d5 = _vab.Dot(_vcp);
			var d6 = _vac.Dot(_vcp);

			if (d6 >= 0 && d5 <= d6)
			{

				// vertex region of C; barycentric coords (0, 0, 1)
				return target.Copy(c);

			}

			var vb = d5 * d2 - d1 * d6;
			if (vb <= 0 && d2 >= 0 && d6 <= 0)
			{

				w = d2 / (d2 - d6);
				// edge region of AC; barycentric coords (1-w, 0, w)
				return target.Copy(a).AddScaledVector(_vac, w);

			}

			var va = d3 * d6 - d5 * d4;
			if (va <= 0 && (d4 - d3) >= 0 && (d5 - d6) >= 0)
			{

				_vbc.SubVectors(c, b);
				w = (d4 - d3) / ((d4 - d3) + (d5 - d6));
				// edge region of BC; barycentric coords (0, 1-w, w)
				return target.Copy(b).AddScaledVector(_vbc, w); // edge region of BC

			}

			// face region
			var denom = 1 / (va + vb + vc);
			// u = va * denom
			v = vb * denom;
			w = vc * denom;

			return target.Copy(a).AddScaledVector(_vab, v).AddScaledVector(_vac, w);

		}

		public bool Equals(Triangle triangle )
		{

			return triangle.a.Equals(this.a) && triangle.b.Equals(this.b) && triangle.c.Equals(this.c);

		}
	}
}

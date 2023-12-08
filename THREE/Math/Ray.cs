
namespace THREE
{
    public class Ray
    {
        private Vector3 _vector = Vector3.Zero();
        private Vector3 _segCenter = Vector3.Zero();
        private Vector3 _segDir = Vector3.Zero();
        private Vector3 _diff = Vector3.Zero();

        private Vector3 _edge1 = Vector3.Zero();
        private Vector3 _edge2 = Vector3.Zero();
        private Vector3 _normal = Vector3.Zero();
        public Vector3 origin;
        public Vector3 direction;

		public Ray(Vector3 origin=null,Vector3 direction = null)
        {
            this.origin = origin != null ? origin : new Vector3();
            this.direction = direction != null ? direction : new Vector3(0,0,-1);
        }
        public Ray(Ray source)
        {
            this.origin.Copy(source.origin);
            this.direction.Copy(source.direction);
        }
		public Ray Set(Vector3 origin,Vector3 direction)
        {
            this.origin.Copy(origin);
            this.direction.Copy(direction);
			return this;
        }
		public Ray Clone()
        {
			return new Ray(this);
        }
		public Ray copy(Ray source)
        {
            this.origin.Copy(source.origin);
            this.direction.Copy(source.direction);
            return this;
        }
		public Vector3 At(float t,Vector3 target=null)
        {
            Vector3 result;
            if(target==null)
            {
                result = new Vector3();
                return result.Copy(this.direction).MultiplyScalar(t).Add(this.origin);
            }
            else
                return target.Copy(this.direction).MultiplyScalar(t).Add(this.origin);
        }
		public Ray LookAt(Vector3 v)
        {
            this.direction.Copy(v).Sub(this.origin).Normalize();
			return this;
        }
		public Ray Recast(float t)
        {
            this.origin.Copy(this.At(t, _vector));
			return this;
        }
		public Vector3 ClosestPointToPoint(Vector3 point, Vector3 target=null)
        {
            Vector3 result;
            if (target == null)
            {
                result = new Vector3();
            }
            else
                result = target;

            result.SubVectors(point, this.origin);
            float directionDistance = target.Dot(this.direction);

            if(directionDistance<0)
            {
                return result.Copy(this.origin);
            }
            return result.Copy(this.direction).MultiplyScalar(directionDistance).Add(this.origin);
        }
		public float DistanceToPoint(Vector3 point)
        {
            return (float)System.Math.Sqrt(this.DistanceSqToPoint(point));
        }
		public float DistanceSqToPoint(Vector3 point)
        {
            float directionDistance = _vector.SubVectors(point, this.origin).Dot(this.direction);

            if (directionDistance < 0)
            {
                return this.origin.DistanceToSquared(point);
            }
            _vector.Copy(this.direction).MultiplyScalar(directionDistance).Add(this.origin);

			return _vector.DistanceToSquared(point);
        }
		public float DistanceSqToSegment(Vector3 v0,Vector3 v1,Vector3 optionalPointOnRay=null,Vector3 optionalPointOnSegment = null)
        {
            // from http://www.geometrictools.com/GTEngine/Include/Mathematics/GteDistRaySegment.h
            // It returns the min distance between the ray and the segment
            // defined by v0 and v1
            // It can also set two optional targets :
            // - The closest point on the ray
            // - The closest point on the segment

            _segCenter.Copy(v0).Add(v1).MultiplyScalar(0.5f);
            _segDir.Copy(v1).Sub(v0).Normalize();
            _diff.Copy(this.origin).Sub(_segCenter);

            var segExtent = v0.DistanceTo(v1) * 0.5f;
            var a01 = -this.direction.Dot(_segDir);
            var b0 = _diff.Dot(this.direction);
            var b1 = -_diff.Dot(_segDir);
            var c = _diff.LengthSq();
            var det = System.Math.Abs(1 - a01 * a01);
            float s0, s1, sqrDist, extDet;

            if (det > 0)
            {

                // The ray and segment are not parallel.

                s0 = a01 * b1 - b0;
                s1 = a01 * b0 - b1;
                extDet = segExtent * det;

                if (s0 >= 0)
                {

                    if (s1 >= -extDet)
                    {

                        if (s1 <= extDet)
                        {

                            // region 0
                            // Minimum at interior points of ray and segment.

                            var invDet = 1 / det;
                            s0 *= invDet;
                            s1 *= invDet;
                            sqrDist = s0 * (s0 + a01 * s1 + 2 * b0) + s1 * (a01 * s0 + s1 + 2 * b1) + c;

                        }
                        else
                        {

                            // region 1

                            s1 = segExtent;
                            s0 = System.Math.Max(0, -(a01 * s1 + b0));
                            sqrDist = -s0 * s0 + s1 * (s1 + 2 * b1) + c;

                        }

                    }
                    else
                    {

                        // region 5

                        s1 = -segExtent;
                        s0 = System.Math.Max(0, -(a01 * s1 + b0));
                        sqrDist = -s0 * s0 + s1 * (s1 + 2 * b1) + c;

                    }

                }
                else
                {

                    if (s1 <= -extDet)
                    {

                        // region 4

                        s0 = System.Math.Max(0, -(-a01 * segExtent + b0));
                        s1 = (s0 > 0) ? -segExtent : System.Math.Min(System.Math.Max(-segExtent, -b1), segExtent);
                        sqrDist = -s0 * s0 + s1 * (s1 + 2 * b1) + c;

                    }
                    else if (s1 <= extDet)
                    {

                        // region 3

                        s0 = 0;
                        s1 = System.Math.Min(System.Math.Max(-segExtent, -b1), segExtent);
                        sqrDist = s1 * (s1 + 2 * b1) + c;

                    }
                    else
                    {

                        // region 2

                        s0 = System.Math.Max(0, -(a01 * segExtent + b0));
                        s1 = (s0 > 0) ? segExtent : System.Math.Min(System.Math.Max(-segExtent, -b1), segExtent);
                        sqrDist = -s0 * s0 + s1 * (s1 + 2 * b1) + c;

                    }

                }

            }
            else
            {

                // Ray and segment are parallel.

                s1 = (a01 > 0) ? -segExtent : segExtent;
                s0 = System.Math.Max(0, -(a01 * s1 + b0));
                sqrDist = -s0 * s0 + s1 * (s1 + 2 * b1) + c;

            }

            if (optionalPointOnRay!=null)
            {

                optionalPointOnRay.Copy(this.direction).MultiplyScalar(s0).Add(this.origin);

            }

            if (optionalPointOnSegment!=null)
            {

                optionalPointOnSegment.Copy(_segDir).MultiplyScalar(s1).Add(_segCenter);

            }

            return sqrDist;
        }
		
		public Vector3 IntersectSphere(Sphere sphere, Vector3 target)
        {
            _vector.SubVectors(sphere.Center, this.origin);
            float tca = _vector.Dot(this.direction);
            float d2 = _vector.Dot(_vector) - tca * tca;
            float radius2 = sphere.Radius * sphere.Radius;

            if (d2 > radius2) return null;

            float thc = (float)System.Math.Sqrt(radius2 - d2);

            // t0 = first intersect point - entrance on front of sphere
            float t0 = tca - thc;

            // t1 = second intersect point - exit point on back of sphere
            float t1 = tca + thc;

            // test to see if both t0 and t1 are behind the ray - if so, return null
            if (t0 < 0 && t1 < 0) return null;

            // test to see if t0 is behind the ray:
            // if it is, the ray is inside the sphere, so return the second exit point scaled by t1,
            // in order to always return an intersect point that is in front of the ray.
            if (t0 < 0) return this.At(t1, target);

            // else t0 is in front of the ray, so return the first collision point scaled by t0
            return this.At(t0, target);
        }
		public bool IntersectsSphere(Sphere sphere)
        {
            return this.DistanceSqToPoint(sphere.Center) <= (sphere.Radius * sphere.Radius);
        }
		public float? DistanceToPlane(Plane plane)
        {
            float denominator = plane.Normal.Dot(this.direction);

            if (denominator == 0)
            {

                // line is coplanar, return origin
                if (plane.DistanceToPoint(this.origin) == 0)
                {

                    return 0;

                }

                // Null is preferable to undefined since undefined means.... it is undefined

                return null;

            }

            float t = -(this.origin.Dot(plane.Normal) + plane.Constant) / denominator;

            // Return if the ray never intersects the plane

            return t >= 0 ? (float?)t : null;
        }
		public Vector3 IntersectPlane(Plane plane, Vector3 target)
        {
            float? t = this.DistanceToPlane(plane);

            if (t == null)
            {

                return null;

            }

            return this.At(t.Value, target);
        }
		public bool IntersectsPlane(Plane plane)
        {
            // check if the ray lies on the plane first

            float distToPoint = plane.DistanceToPoint(this.origin);

            if (distToPoint == 0)
            {

                return true;

            }

            float denominator = plane.Normal.Dot(this.direction);

            if (denominator * distToPoint < 0)
            {

                return true;

            }

            // ray origin is behind the plane (and is pointing behind it)

            return false;
        }
		public Vector3 IntersectBox(Box3 box, Vector3 target)
        {
            float tmin, tmax, tymin, tymax, tzmin, tzmax;

            float invdirx = 1 / this.direction.X;
            float invdiry = 1 / this.direction.Y;               
            float invdirz = 1 / this.direction.Z;


            if (invdirx >= 0)
            {

                tmin = (box.Min.X - origin.X) * invdirx;
                tmax = (box.Max.X - origin.X) * invdirx;

            }
            else
            {

                tmin = (box.Max.X - origin.X) * invdirx;
                tmax = (box.Min.X - origin.X) * invdirx;

            }

            if (invdiry >= 0)
            {

                tymin = (box.Min.Y - origin.Y) * invdiry;
                tymax = (box.Max.Y - origin.Y) * invdiry;

            }
            else
            {

                tymin = (box.Max.Y - origin.Y) * invdiry;
                tymax = (box.Min.Y - origin.Y) * invdiry;

            }

            if ((tmin > tymax) || (tymin > tmax)) return null;

            // These lines also handle the case where tmin or tmax is NaN
            // (result of 0 * Infinity). x !== x returns true if x is NaN

            if (tymin > tmin) tmin = tymin;

            if (tymax < tmax) tmax = tymax;

            if (invdirz >= 0)
            {

                tzmin = (box.Min.Z - origin.Z) * invdirz;
                tzmax = (box.Max.Z - origin.Z) * invdirz;

            }
            else
            {

                tzmin = (box.Max.Z - origin.Z) * invdirz;
                tzmax = (box.Min.Z - origin.Z) * invdirz;

            }

            if ((tmin > tzmax) || (tzmin > tmax)) return null;

            if (tzmin > tmin || tmin != tmin) tmin = tzmin;

            if (tzmax < tmax || tmax != tmax) tmax = tzmax;

            //return point closest to the ray (positive side)

            if (tmax < 0) return null;

            return this.At(tmin >= 0 ? tmin : tmax, target);
        }
		public bool IntersectsBox(Box3 box)
        {
            return this.IntersectBox(box, _vector) != null;
        }
		public Vector3 IntersectTriangle(Vector3 a,Vector3 b,Vector3 c,bool backfaceCulling,Vector3 target=null)
        {
            // Compute the offset origin, edges, and normal.

            // from http://www.geometrictools.com/GTEngine/Include/Mathematics/GteIntrRay3Triangle3.h

            _edge1.SubVectors(b, a);
            _edge2.SubVectors(c, a);
            _normal.CrossVectors(_edge1, _edge2);

            // Solve Q + t*D = b1*E1 + b2*E2 (Q = kDiff, D = ray direction,
            // E1 = kEdge1, E2 = kEdge2, N = Cross(E1,E2)) by
            //   |Dot(D,N)|*b1 = sign(Dot(D,N))*Dot(D,Cross(Q,E2))
            //   |Dot(D,N)|*b2 = sign(Dot(D,N))*Dot(D,Cross(E1,Q))
            //   |Dot(D,N)|*t = -sign(Dot(D,N))*Dot(Q,N)
            float DdN = this.direction.Dot(_normal);
            float sign;

            if (DdN > 0)
            {

                if (backfaceCulling) return null;
                sign = 1;

            }
            else if (DdN < 0)
            {

                sign = -1;
                DdN = -DdN;

            }
            else
            {

                return null;

            }

            _diff.SubVectors(this.origin, a);
            float DdQxE2 = sign * this.direction.Dot(_edge2.CrossVectors(_diff, _edge2));

            // b1 < 0, no intersection
            if (DdQxE2 < 0)
            {

                return null;

            }

            float DdE1xQ = sign * this.direction.Dot(_edge1.Cross(_diff));

            // b2 < 0, no intersection
            if (DdE1xQ < 0)
            {

                return null;

            }

            // b1+b2 > 1, no intersection
            if (DdQxE2 + DdE1xQ > DdN)
            {

                return null;

            }

            // Line intersects triangle, check if ray does.
            float QdN = -sign * _diff.Dot(_normal);

            // t < 0, no intersection
            if (QdN < 0)
            {

                return null;

            }

            // Ray intersects triangle.
            return this.At(QdN / DdN, target);
        }
		
	    public Ray ApplyMatrix4(Matrix4 matrix4)
        {
            this.origin.ApplyMatrix4(matrix4);
            this.direction.TransformDirection(matrix4);

            return this;
        }
	    public bool Equals(Ray ray)
        {
            return ray.origin.Equals(this.origin) && ray.direction.Equals(this.direction);
        }
    }
}

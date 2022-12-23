using System;
using System.Collections;
using System.Collections.Generic;
using THREE;

namespace THREE
{
    public class Curve : Vector3
    {
        private float ArcLengthDivisions = 200;

		private List<float> cacheArcLengths;

		public bool NeedsUpdate=false;

        public Curve() : base()
        {
            
        }
		protected Curve(Curve other)
		{
			this.ArcLengthDivisions = other.ArcLengthDivisions;
		}
		public virtual Vector3 GetPoint(float t,Vector3 optionalTarget=null)
		{
			throw new NotImplementedException();
		}

		public virtual Vector3 GetPointAt(float u, Vector3 optionalTarget=null )
		{

			var t = this.GetUtoTmapping(u);
			return this.GetPoint(t, optionalTarget);

		}
		public virtual List<Vector3> GetPoints(float? divisions=null)
		{

			if (divisions == null) divisions = 5;

			var points = new List<Vector3>();

			for (var d = 0; d <= divisions; d++)
			{

				points.Add(this.GetPoint(d / divisions.Value));

			}

			return points;

		}

		public virtual List<Vector3> GetSpacedPoints(float? divisions=null )
		{

			if (divisions == null) divisions = 5;

			var points = new List<Vector3>();

			for (var d = 0; d <= divisions; d++)
			{

				points.Add(this.GetPointAt(d / divisions.Value));

			}

			return points;

		}

		public virtual float GetLength()
		{

			var lengths = this.GetLengths();
			return lengths[lengths.Count - 1];

		}
		public List<float> GetLengths(float? divisions=null)
		{
			if (divisions == null) divisions = this.ArcLengthDivisions;

			if (this.cacheArcLengths!=null &&
				(this.cacheArcLengths.Count == divisions + 1) &&
				!this.NeedsUpdate)
			{

				return this.cacheArcLengths;

			}

			this.NeedsUpdate = false;

			var cache = new List<float>();
			var current = this.GetPoint(0);
			var last = this.GetPoint(0);
			var p = 0;
			var sum = 0.0f;

			cache.Add(0);

			for (p = 1; p <= divisions; p++)
			{

				current = this.GetPoint(p / divisions.Value);
				sum += current.DistanceTo(last);
				cache.Add(sum);
				last = current;

			}

			this.cacheArcLengths = cache;

			return cache; // { sums: cache, sum: sum }; Sum is in the last element.
		}

		public virtual void UpdateArcLengths()
		{

			this.NeedsUpdate = true;
			this.GetLengths();

		}
		public float GetUtoTmapping(float u, float? distance=null )
		{

			var arcLengths = this.GetLengths();

			var i = 0;
			var il = arcLengths.Count;

			float targetArcLength; // The targeted u distance value to get

			if (distance!=null)
			{

				targetArcLength = distance.Value;

			}
			else
			{

				targetArcLength = u * arcLengths[il - 1];

			}

			// binary search for the index with largest value smaller than target u distance

			float low = 0;
			float comparison;
			int high = il - 1;
			while (low <= high)
			{

				i = (int)System.Math.Floor(low + (high - low) / 2); // less likely to overflow, though probably not issue here, JS doesn't really have integers, all numbers are floats

				comparison = arcLengths[i] - targetArcLength;

				if (comparison < 0)
				{

					low = i + 1;

				}
				else if (comparison > 0)
				{

					high = i - 1;

				}
				else
				{

					high = i;
					break;

					// DONE

				}

			}

			i = high;

			if (arcLengths[i] == targetArcLength)
			{

				return (float)i /(float) (il - 1);

			}

			// we could get finer grain at lengths, or use simple interpolation between two points

			var lengthBefore = arcLengths[i];
			var lengthAfter = arcLengths[i + 1];

			var segmentLength = lengthAfter - lengthBefore;

			// determine where we are between the 'before' and 'after' points

			var segmentFraction = (targetArcLength - lengthBefore) / segmentLength;

			// add that fractional amount to t

			var t = (i + segmentFraction) /(float) (il - 1);

			return t;

		}

		public Vector3 GetTangent(float t)
		{

			var delta = 0.0001f;
			var t1 = t - delta;
			var t2 = t + delta;

			// Capping in case of danger

			if (t1 < 0) t1 = 0;
			if (t2 > 1) t2 = 1;

			var pt1 = this.GetPoint(t1);
			var pt2 = this.GetPoint(t2);

			var vec = (pt2.Clone() as Vector3).Sub(pt1);

			return vec.Normalize();

		}
		public Vector3 GetTangentAt(float u)
		{

			var t = this.GetUtoTmapping(u);
			return this.GetTangent(t);

		}
		public Hashtable ComputeFrenetFrames(int segments, bool closed)
		{

			// see http://www.cs.indiana.edu/pub/techreports/TR425.pdf

			var normal = new Vector3();

			var tangents = new List<Vector3>();
			var normals = new List<Vector3>();
			var binormals = new List<Vector3>();

			var vec = new Vector3();
			var mat = new Matrix4();

			

			// compute the tangent vectors for each segment on the curve

			for (int i = 0; i <= segments; i++)
			{

				float u = i / (float)segments;

				Vector3 v = this.GetTangentAt(u);
				v.Normalize();
				tangents.Add(v);			

			}

			// select an initial normal vector perpendicular to the first tangent vector,
			// and in the direction of the minimum tangent xyz component

			normals.Add(new Vector3());
			binormals.Add(new Vector3());
			float min = float.MaxValue;
			var tx = (float)System.Math.Abs(tangents[0].X);
			var ty = (float)System.Math.Abs(tangents[0].Y);
			var tz = (float)System.Math.Abs(tangents[0].Z);

			if (tx <= min)
			{

				min = tx;
				normal.Set(1, 0, 0);

			}

			if (ty <= min)
			{

				min = ty;
				normal.Set(0, 1, 0);

			}

			if (tz <= min)
			{

				normal.Set(0, 0, 1);

			}

			vec.CrossVectors(tangents[0], normal).Normalize();

			normals[0].CrossVectors(tangents[0], vec);
			binormals[0].CrossVectors(tangents[0], normals[0]);


			// compute the slowly-varying normal and binormal vectors for each segment on the curve

			for (int i = 1; i <= segments; i++)
			{

				normals.Add((Vector3)(normals[i - 1].Clone()));

				binormals.Add(binormals[i - 1].Clone() as Vector3);

				vec.CrossVectors(tangents[i - 1], tangents[i]);

				if (vec.Length() > float.Epsilon)
				{

					vec.Normalize();

					//float theta = System.Math.Acos(_Math.clamp(tangents[i - 1].dot(tangents[i]), -1, 1)); // clamp for floating pt errors

					float theta = (float)System.Math.Acos(tangents[i - 1].Dot(tangents[i]).Clamp(-1, 1));
					normals[i].ApplyMatrix4(mat.MakeRotationAxis(vec, theta));

				}

				binormals[i].CrossVectors(tangents[i], normals[i]);

			}

			// if the curve is closed, postprocess the vectors so the first and last normal vectors are the same

			if (closed == true)
			{

				float theta = (float)System.Math.Acos(normals[0].Dot(normals[segments]).Clamp(-1, 1));
					//Math.acos(_Math.clamp(normals[0].dot(normals[segments]), -1, 1));
				theta /= (float)segments;

				if (tangents[0].Dot(vec.CrossVectors(normals[0], normals[segments])) > 0)
				{

					theta = -theta;

				}

				for (int i = 1; i <= segments; i++)
				{

					// twist a little...
					normals[i].ApplyMatrix4(mat.MakeRotationAxis(tangents[i], theta * i));
					binormals[i].CrossVectors(tangents[i], normals[i]);

				}

			}


			return new Hashtable(){
				{ "tangents", tangents },
				{ "normals", normals },
				{ "binormals", binormals }
			};

		}
	
	}
}

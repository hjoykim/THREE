using System;
using System.Collections;
using System.Collections.Generic;

namespace THREE
{
    public interface CurveInterface
    {

		Vector3 GetPoint(float t, Vector3 optionalTarget = null);

		Vector3 GetPointAt(float u, Vector3 optionalTarget = null);

		List<Vector3> GetPoints(float? divisions = null);


		List<Vector3> GetSpacedPoints(float? divisions = null);

		float GetLength();

		List<float> GetLengths(float? divisions = null);

		void UpdateArcLengths();

		float GetUtoTmapping(float u, float? distance = null);

		Vector3 GetTangent(float t);

		Vector3 GetTangentAt(float u);

		Hashtable ComputeFrenetFrames(int segments, bool closed);

		object Clone();
	}
}

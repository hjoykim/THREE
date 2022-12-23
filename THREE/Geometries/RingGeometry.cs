using System;
using System.Collections;
using System.Collections.Generic;

namespace THREE
{
    public class  RingGeometry : Geometry
    {
        public Hashtable parameter;

        public RingGeometry(float? innerRadius=null, float? outerRadius=null, float? thetaSegments=null,float? phiSegments=null, float? thetaStart=null,float? thetaLength = null) : base()
        {
            this.type = "RingGeometry";

            parameter = new Hashtable()
            {
                {"innerRadius",innerRadius },
                {"outerRadius",outerRadius },
                {"thetaSegments",thetaSegments },
                {"phiSegments",phiSegments },
                {"thetaStart",thetaStart },
                {"thetaLength",thetaLength }
            };

            this.FromBufferGeometry(new RingBufferGeometry(innerRadius, outerRadius, thetaSegments, phiSegments, thetaStart, thetaLength));
            this.MergeVertices();
        }
    }

    public class RingBufferGeometry : BufferGeometry
    {
        public Hashtable parameter;
        public RingBufferGeometry(float? innerRadius = null, float? outerRadius = null, float? thetaSegments = null, float? phiSegments = null, float? thetaStart = null, float? thetaLength = null) : base()
        {
            this.type = "RingGeometry";

            parameter = new Hashtable()
            {
                {"innerRadius",innerRadius },
                {"outerRadius",outerRadius },
                {"thetaSegments",thetaSegments },
                {"phiSegments",phiSegments },
                {"thetaStart",thetaStart },
                {"thetaLength",thetaLength }
            };

			innerRadius = innerRadius != null ? innerRadius : 0.5f;
			outerRadius = outerRadius !=null ? outerRadius : 1;

			thetaStart = thetaStart != null ? thetaStart : 0;
			thetaLength = thetaLength != null ? thetaLength :(float)System.Math.PI * 2;

			thetaSegments = thetaSegments != null ? (float)System.Math.Max(3, thetaSegments.Value) : 8;
			phiSegments = phiSegments != null ? (float)System.Math.Max(1, phiSegments.Value) : 1;

			// buffers

			List<int> indices = new List<int>();
			var vertices = new List<Vector3>();
			var normals = new List<Vector3>();
			var uvs = new List<Vector2>();

			// some helper variables

			float segment;
			var radius = innerRadius;
			var radiusStep = ((outerRadius - innerRadius) / phiSegments);
			var vertex = new Vector3();
			var uv = new Vector2();

			// generate vertices, normals and uvs

			for (int j = 0; j <= phiSegments; j++)
			{

				for (int i = 0; i <= thetaSegments; i++)
				{

					// values are generate from the inside of the ring to the outside

					segment = thetaStart.Value + i / thetaSegments.Value * thetaLength.Value;

					// vertex

					vertex.X = (float)(radius * System.Math.Cos(segment));
					vertex.Y = (float)(radius * System.Math.Sin(segment));

					vertices.Add((Vector3)vertex.Clone());

					// normal

					normals.Add(new Vector3(0, 0, 1));

					// uv

					uv.X = (vertex.X / outerRadius.Value + 1) / 2;
					uv.Y = (vertex.Y / outerRadius.Value + 1) / 2;

					uvs.Add((Vector2)uv.Clone());

				}

				// increase the radius for next row of vertices

				radius += radiusStep;

			}

			// indices

			for (int j = 0; j < phiSegments; j++)
			{

				var thetaSegmentLevel = j * (thetaSegments.Value + 1);

				for (int i = 0; i < thetaSegments; i++)
				{

					segment = i + thetaSegmentLevel;

					int a = Convert.ToInt32(segment);
					int b = Convert.ToInt32(segment + thetaSegments + 1);
					int c = Convert.ToInt32(segment + thetaSegments + 2);
					int d = Convert.ToInt32(segment + 1);

					// faces

					indices.Add(a, b, d);
					indices.Add(b, c, d);

				}

			}

			// build geometry

			this.SetIndex(indices);

			BufferAttribute<float> positions = new BufferAttribute<float>();
			positions.ItemSize = 3;
			positions.Type = typeof(float);

			this.SetAttribute("position", positions.CopyVector3sArray(vertices.ToArray()));

			BufferAttribute<float> normalAttributes = new BufferAttribute<float>();
			normalAttributes.ItemSize = 3;
			normalAttributes.Type = typeof(float);
			this.SetAttribute("normal", normalAttributes.CopyVector3sArray(normals.ToArray()));

			BufferAttribute<float> uvAttributes = new BufferAttribute<float>();
			uvAttributes.ItemSize = 2;
			uvAttributes.Type = typeof(float);
			this.SetAttribute("uv", uvAttributes.CopyVector2sArray(uvs.ToArray()));

		}
    }
}


using System;
using System.Collections;
using System.Collections.Generic;

namespace THREE
{


    public class TorusGeometry : Geometry
    {
        public Hashtable parameters;
		public TorusGeometry(float? radius = null, float? tube = null, float? radialSegments = null, float? tubularSegments = null, float? arc = null) : base()
		{
			parameters = new Hashtable()
			{
				{"radius",radius },
				{"tube",radius },
				{"radialSegments",radius },
				{"tubularSegments",radius },
				{"arc",radius },
			};

			this.FromBufferGeometry(new TorusBufferGeometry(radius, tube, radialSegments, tubularSegments, arc));
			this.MergeVertices();
		}

	}

    public class TorusBufferGeometry : BufferGeometry
    {
        public Hashtable parameters;

        public TorusBufferGeometry(float? radius=null,float? tube = null, float? radialSegments = null, float? tubularSegments = null, float? arc = null) : base()
        {
            radius = radius != null ? radius : 1;
            tube = tube != null ? tube : 1;
            radialSegments = radialSegments != null ? (float)Math.Floor(radialSegments.Value) : 8;
            tubularSegments = tubularSegments != null ? (float)Math.Floor(tubularSegments.Value) : 6;
            arc = arc != null ? arc : (float)Math.PI * 2;
            parameters = new Hashtable()
            {
                {"radius",radius },
                {"tube",radius },
                {"radialSegments",radius },
                {"tubularSegments",radius },
                {"arc",radius },
            };

			List<int> indices = new List<int>();
			List<float> vertices = new List<float>();
			List<float> normals = new List<float>();
			List<float> uvs = new List<float>();

			// helper variables

			var center = new Vector3();
			var vertex = new Vector3();
			var normal = new Vector3();

			int j, i;

			// generate vertices, normals and uvs

			for (j = 0; j <= radialSegments; j++)
			{

				for (i = 0; i <= tubularSegments; i++)
				{

					float u = i / (float)tubularSegments * (float)arc;
					float v = j / (float)radialSegments * (float)Math.PI * 2;

					// vertex

					vertex.X = (float)((radius + tube * Math.Cos(v)) * Math.Cos(u));
					vertex.Y = (float)((radius + tube * Math.Cos(v)) * Math.Sin(u));
					vertex.Z = (float)(tube * Math.Sin(v));

					vertices.Add(vertex.X, vertex.Y, vertex.Z);

					// normal

					center.X = radius.Value * (float)Math.Cos(u);
					center.Y = radius.Value * (float)Math.Sin(u);
					normal.SubVectors(vertex, center).Normalize();

					normals.Add(normal.X, normal.Y, normal.Z);

					// uv

					uvs.Add(i / tubularSegments.Value);
					uvs.Add(j / radialSegments.Value);

				}

			}

			// generate indices

			for (j = 1; j <= radialSegments; j++)
			{

				for (i = 1; i <= tubularSegments; i++)
				{

					// indices

					int a = ((int)tubularSegments + 1) * j + i - 1;
					int b = ((int)tubularSegments + 1) * (j - 1) + i - 1;
					int c = ((int)tubularSegments + 1) * (j - 1) + i;
					int d = ((int)tubularSegments + 1) * j + i;

					// faces

					indices.Add(a, b, d);
					indices.Add(b, c, d);

				}

			}

			// build geometry

			this.SetIndex(indices);
			this.SetAttribute("position", new BufferAttribute<float>(vertices.ToArray(), 3));
			this.SetAttribute("normal", new BufferAttribute<float>(normals.ToArray(), 3));
			this.SetAttribute("uv", new BufferAttribute<float>(uvs.ToArray(), 2));

		}
    }


}

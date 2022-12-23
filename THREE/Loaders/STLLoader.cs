using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace THREE
{
    public class STLLoader
    {
        public STLLoader() { }

        public static BufferGeometry ParseBinary(string filePath)
        {
			BufferGeometry geometry = new BufferGeometry();
			uint faces = 0;

			float r =0, g =0, b = 0;
			bool hasColors = false;
			float defaultR=0, defaultG=0, defaultB=0, alpha=1;

			// process STL header
			// check for default color in header ("COLOR=rgba" sequence).
			using (BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
			{
				var bytes = reader.ReadBytes(80);
				faces = reader.ReadUInt32();
				reader.BaseStream.Seek(-84, SeekOrigin.Current);

				for (var index = 0; index < 80 - 10; index++)
				{

					if (reader.ReadChar().Equals('C') && reader.ReadChar().Equals('O') && reader.ReadChar().Equals('L') && reader.ReadChar().Equals('O'))
						if (reader.ReadChar().Equals('R') && reader.ReadChar().Equals('='))
						{
							hasColors = true;
							defaultR = reader.ReadByte() / 255.0f;
							defaultG = reader.ReadByte() / 255.0f;
							defaultB = reader.ReadByte() / 255.0f;
							alpha = reader.ReadByte() / 255.0f;

						}
				}
				reader.BaseStream.Seek(84, SeekOrigin.Begin);

				var dataOffset = 84;
				var faceLength = 12 * 4 + 2;


				List<float> vertices = new List<float>(new float[faces * 3 * 3]);
				List<float> normals = new List<float>(new float[faces * 3 * 3]);
				List<float> colors = new List<float>(new float[faces * 3 * 3]);

				for (var face = 0; face < faces; face++)
				{
					var buffer = new byte[4];

					var start = dataOffset + face * faceLength;
					reader.BaseStream.Position = start;
					var normalX = reader.ReadSingle();
					var normalY = reader.ReadSingle();
					var normalZ = reader.ReadSingle();

					if (hasColors)
					{

						reader.BaseStream.Position = start + 48;
						var packedColor = reader.ReadUInt16();

						if ((packedColor & 0x8000) == 0)
						{

							// facet has its own unique color

							r = (packedColor & 0x1F) / 31;
							g = ((packedColor >> 5) & 0x1F) / 31;
							b = ((packedColor >> 10) & 0x1F) / 31;

						}
						else
						{

							r = defaultR;
							g = defaultG;
							b = defaultB;

						}

					}

					for (var i = 1; i <= 3; i++)
					{

						var vertexstart = start + i * 12;
						var componentIdx = (face * 3 * 3) + ((i - 1) * 3);
						reader.BaseStream.Position = vertexstart;
						vertices[componentIdx] = reader.ReadSingle();
						vertices[componentIdx + 1] = reader.ReadSingle();
						vertices[componentIdx + 2] = reader.ReadSingle();

						normals[componentIdx] = normalX;
						normals[componentIdx + 1] = normalY;
						normals[componentIdx + 2] = normalZ;

						if (hasColors)
						{

							colors[componentIdx] = r;
							colors[componentIdx + 1] = g;
							colors[componentIdx + 2] = b;

						}

					}

				}


				geometry.SetAttribute("position", new BufferAttribute<float>(vertices.ToArray(), 3));
				geometry.SetAttribute("normal", new BufferAttribute<float>(normals.ToArray(), 3));

				if (hasColors)
				{

					geometry.SetAttribute("color", new BufferAttribute<float>(colors.ToArray(), 3));
					geometry.UserData["hasColors"] = true;
					geometry.UserData["alpha"] = alpha;

				}
			}
			return geometry;
		}

		public static BufferGeometry ParseASCII(string filePath)
        {
			var alltext = File.ReadAllText(filePath);

			BufferGeometry geometry = new BufferGeometry();
			//var patternSolid = / solid([\s\S] *?)endsolid / g;
			//var patternFace = / facet([\s\S] *?)endfacet / g;
			var faceCounter = 0;
			var groupCount = 0;
			var startVertex = 0;
			var endVertex = 0;
			var patternFloat = @"[\s]+([+-]?(?:\d*)(?:\.\d*)?(?:[eE][+-]?\d+)?)";

			Regex patternSolid = new Regex(@"^solid([\s\S]*?)endsolid$");
			Regex patternFace = new Regex(@"facet([\s\S]*?)endfacet");
			Regex patternVertex = new Regex("vertex" + patternFloat + patternFloat + patternFloat);
			Regex patternNormal = new Regex("normal" + patternFloat + patternFloat + patternFloat);

			List<float> vertices = new List<float>();
			List<float> normals = new List<float>();

			var normal = new Vector3();

			MatchCollection solidCollection = patternSolid.Matches(alltext);
			foreach(Match s in solidCollection)
            {
				startVertex = endVertex;
				var solid = s.Groups[1].Value;
				MatchCollection faceCollection = patternFace.Matches(solid);

				foreach(Match n in faceCollection)
                {
					var vertexCountPerFace = 0;
					var normalCounterFace = 0;

					var text = n.Groups[1].Value;
					MatchCollection normalCollection = patternNormal.Matches(text);
					normal.X = float.Parse(normalCollection[0].Groups[1].Value);
					normal.Y = float.Parse(normalCollection[0].Groups[2].Value);
					normal.Z = float.Parse(normalCollection[0].Groups[3].Value);
					normalCounterFace++;

					MatchCollection vertexCollection = patternVertex.Matches(text);
					foreach(Match v in vertexCollection)
                    {
						vertices.Add(float.Parse(v.Groups[1].Value), float.Parse(v.Groups[2].Value), float.Parse(v.Groups[3].Value));
						normals.Add(normal.X, normal.Y, normal.Z);
						vertexCountPerFace++;
						endVertex++;
                    }
					if(normalCounterFace!=1)
                    {
						Trace.TraceError("THREE.STLLoader:Something isn't right with the normal of face nubmer {0}", faceCounter);
                    }
                    if (vertexCountPerFace != 3)
                    {
						Trace.TraceError("THREE.STLLoader:Something isn't right with the verticies of face number {0}", faceCounter);
					}

					faceCounter++;

				}           			

				var start = startVertex;
				var count = endVertex - startVertex;

				geometry.AddGroup(start, count, groupCount);
				groupCount++;

			}

			geometry.SetAttribute("position", new BufferAttribute<float>(vertices.ToArray(), 3));
			geometry.SetAttribute("normal", new BufferAttribute<float>(normals.ToArray(), 3));

			return geometry;
		}

		public static bool isBinary(string path)
        {
			long length = new FileInfo(path).Length;
			if (length == 0) return false;

			using(StreamReader stream = new StreamReader(path))
            {
				int ch;
				while((ch=stream.Read())!=-1)
                {
					if(isControlChar(ch))
                    {
						return true;
                    }
                }
            }
			return false;
        }

		public static bool isControlChar(int ch)
        {
			return (ch > Chars.NUL && ch < Chars.BS) 
				|| (ch > Chars.CR && ch < Chars.SUB);
        }
		public static class Chars
        {
			public static char NUL = (char)0; // Null
			public static char BS = (char)8; // Back space
			public static char CR = (char)13; // Carriage return
			public static char SUB = (char)26;//substitute
        }

		public static BufferGeometry Load(string filePath)
        {
			if (isBinary(filePath))
				return ParseBinary(filePath);
			else
				return ParseASCII(filePath);
        }
    }
}

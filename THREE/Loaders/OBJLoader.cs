using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace THREE
{
    class Object
    {
        public class ObjectMaterial
        {
            public int Index;
            
            public string Name;

            public int GroupStart = 0;

            public int GroupEnd = -1;

            public int GroupCount = -1;

            public bool Inherited = false;

            public bool Smooth;

            public object MtlLib;

            public ObjectMaterial Clone(int index)
            {
                var cloned = new ObjectMaterial();

                cloned.Index = index;
                cloned.Name = Name;

                if (MtlLib is List<ObjectMaterial>)
                {
                    cloned.MtlLib = (MtlLib as List<ObjectMaterial>).ToList();
                }
                else
                {
                    cloned.MtlLib = MtlLib;
                }
                cloned.Smooth = Smooth;

                return cloned;
            }
        }

        public class ObjectGeometry
        {
            public List<float> Vertices = new List<float>();

            public List<float> Normals = new List<float>();

            public List<float> Colors = new List<float>();

            public List<float> Uvs = new List<float>();

            public string type;
        }

        public string Name ="";

        public bool FromDeclaration = true;

        public ObjectGeometry Geometry;

        public List<ObjectMaterial> Materials = new List<ObjectMaterial>();

        public bool Smooth = true;

        public ObjectMaterial CurrentMaterial()
        {
            if (this.Materials.Count > 0)
            {
                return this.Materials[this.Materials.Count - 1];
            }
            else
                return null;
        }

        public ObjectMaterial StartMaterial(string name, object libraries)
        {

            var previous = this.Finalize(false);

            if(previous!=null && (previous.Inherited || previous.GroupCount<=0))
            {
                this.Materials.RemoveAt(previous.Index);
            }

            var material = new ObjectMaterial();

            material.Index = this.Materials.Count;
            material.Name = name;
            material.MtlLib = (libraries is List<string> && (libraries as List<string>).Count > 0) ? (libraries as List<string>)[(libraries as List<string>).Count - 1] : null;
            material.Smooth = previous != null ? previous.Smooth : this.Smooth;
            material.GroupStart = previous != null ? previous.GroupEnd : 0;
            material.GroupEnd = -1;
            material.GroupCount = -1;
            material.Inherited = false;

            this.Materials.Add(material);

            return material;
        }

        public ObjectMaterial Finalize(bool end)
        {
            var lastMultiMaterial = this.CurrentMaterial();
            if (lastMultiMaterial != null && lastMultiMaterial.GroupEnd == -1)
            {
                lastMultiMaterial.GroupEnd = this.Geometry.Vertices.Count / 3;
                lastMultiMaterial.GroupCount = lastMultiMaterial.GroupEnd - lastMultiMaterial.GroupStart;
                lastMultiMaterial.Inherited = false;
            }
            // Ignore objects tail materials if no face declarations followed them before a new o/g started.
			if ( end && this.Materials.Count > 1 ) {

			    for ( var mi = this.Materials.Count - 1; mi >= 0; mi -- ) {

				    if(this.Materials[ mi ].GroupCount <= 0 ) {

							this.Materials.RemoveAt(mi);

					}
				}
			}

			// Guarantee at least one empty material, this makes the creation later more straight forward.
			if ( end && this.Materials.Count == 0 ) {

                ObjectMaterial material = new ObjectMaterial(){Name="",Smooth=this.Smooth};

                this.Materials.Add(material);
				
			}

			return lastMultiMaterial;
        }
    }
    class ObjectState
    {
        public List<Object> Objects = new List<Object>();

        public Object Object = new Object();

        public List<float> Vertices = new List<float>();

        public List<float> Normals = new List<float>();

        public List<float> Colors = new List<float>();

        public List<float> Uvs = new List<float>();

        public List<string> MaterialLibraries = new List<string>();

        public void StartObject(string name, bool fromDeclaration)
        {
            if(this.Object!=null && this.Object.FromDeclaration==false)
            {
                this.Object.Name = name;
                this.Object.FromDeclaration = fromDeclaration != false;
           
                return;
            }

            var previousMaterial = this.Object.CurrentMaterial();

            if (this.Object != null)
            {
                this.Object.Finalize(true);
            }
            
            this.Object = new Object() 
            { 
                Name = name,
                FromDeclaration = fromDeclaration!=false,
                Geometry = new Object.ObjectGeometry()                
            };

            if (previousMaterial != null && previousMaterial.Name!=null)
            {
                var declared = previousMaterial.Clone(0);
                declared.Inherited = true;
                this.Object.Materials.Add(declared);
            }

            this.Objects.Add(this.Object);  
          
        }

        public void finalize()
        {
            if (this.Object != null)
            {
                this.Object.Finalize(true);
            }
        }

        private int ParseVertexIndex(int value,int len)
        {
            var index = (int)value;

            return (index >= 0 ? index - 1 : index + len / 3) * 3;
        }
        private int ParseNormalIndex(int value, int len ) 
        {

			var index = (int)value;
			return ( index >= 0 ? index - 1 : index + len / 3 ) * 3;

		}

		private int ParseUVIndex(int value, int len ) 
        {

			var index = (int)value;
				
            return ( index >= 0 ? index - 1 : index + len / 2 ) * 2;
        }

        private void AddVertex(int a, int b, int c ) 
        {
			var src = this.Vertices;
			var dst = this.Object.Geometry.Vertices;

			dst.Add( src[ a + 0 ], src[ a + 1 ], src[ a + 2 ] );
			dst.Add( src[ b + 0 ], src[ b + 1 ], src[ b + 2 ] );
			dst.Add( src[ c + 0 ], src[ c + 1 ], src[ c + 2 ] );
        }
		private void AddColor(int a, int b, int c ) 
        {

			var src = this.Colors;
			var dst = this.Object.Geometry.Colors;

			dst.Add( src[ a + 0 ], src[ a + 1 ], src[ a + 2 ] );
			dst.Add( src[ b + 0 ], src[ b + 1 ], src[ b + 2 ] );
			dst.Add( src[ c + 0 ], src[ c + 1 ], src[ c + 2 ] );

		}     
        private void AddUV(int a, int b, int c ) 
        {
            var src = this.Uvs;
			var dst = this.Object.Geometry.Uvs;

			dst.Add( src[ a + 0 ], src[ a + 1 ] );
			dst.Add( src[ b + 0 ], src[ b + 1 ] );
			dst.Add( src[ c + 0 ], src[ c + 1 ] );
		}

        private void AddNormal(int a, int b, int c ) 
        {

			var src = this.Normals;
			var dst = this.Object.Geometry.Normals;

			dst.Add( src[ a + 0 ], src[ a + 1 ], src[ a + 2 ] );
			dst.Add( src[ b + 0 ], src[ b + 1 ], src[ b + 2 ] );
			dst.Add( src[ c + 0 ], src[ c + 1 ], src[ c + 2 ] );

		}

        public void AddVertexPoint(int a)
        {
            var src = this.Vertices;
            var dst = this.Object.Geometry.Vertices;

            dst.Add(src[a + 0], src[a + 1], src[a + 2]);
        }
        public void AddVertexLine(int a)
        {

            var src = this.Vertices;
            var dst = this.Object.Geometry.Vertices;

            dst.Add(src[a + 0], src[a + 1], src[a + 2]);

        }
        public void AddUVLine(int a)
        {

            var src = this.Uvs;
            var dst = this.Object.Geometry.Uvs;

            dst.Add(src[a + 0], src[a + 1]);

        }
        public void AddPointGeometry(string[] vertices)
        {
            this.Object.Geometry.type = "Points";

            var vLen = this.Vertices.Count;

            for (var vi = 0; vi < vertices.Length; vi++)
            {
                this.AddVertexPoint(this.ParseVertexIndex(int.Parse(vertices[vi]), vLen));
            }
        }
        public void AddPointGeometry(float[] vertices)
        {
            this.Object.Geometry.type = "Points";

            var vLen = this.Vertices.Count;

            for(var vi = 0;vi< vertices.Length; vi++)
            {
                this.AddVertexPoint(this.ParseVertexIndex((int)vertices[vi],vLen));
            }
        }
        public void AddLineGeometry(List<string> vertices,List<string> uvs)
        {
            this.Object.Geometry.type = "Line";

            var vLen = this.Vertices.Count;
            var uvLen = this.Uvs.Count;

            for (var vi = 0; vi < vertices.Count; vi++)
            {

                this.AddVertexLine(this.ParseVertexIndex(int.Parse(vertices[vi]), vLen));

            }

            if (uvs != null)
            {
                for (var uvi = 0; uvi < uvs.Count; uvi++)
                {

                    this.AddUVLine(this.ParseUVIndex(int.Parse(uvs[uvi]), uvLen));

                }
            }
        }
        public void AddLineGeometry(float[] vertices,float[] uvs=null)
        {
            this.Object.Geometry.type = "Line";

            var vLen = this.Vertices.Count;
            var uvLen = this.Uvs.Count;

            for (var vi = 0;vi< vertices.Length; vi++)
            {

                this.AddVertexLine(this.ParseVertexIndex((int)vertices[vi], vLen));

            }

            if (uvs != null)
            {
                for (var uvi = 0;uvi< uvs.Length;uvi++)
                {

                    this.AddUVLine(this.ParseUVIndex((int)uvs[uvi], uvLen));

                }
            }

        }

        public void AddFace(int? a, int? b, int? c, int? ua, int? ub, int? uc, int? na, int? nb, int? nc)
        {

            var vLen = this.Vertices.Count;

            var ia = this.ParseVertexIndex((int)a, vLen);
            var ib = this.ParseVertexIndex((int)b, vLen);
            var ic = this.ParseVertexIndex((int)c, vLen);

            this.AddVertex(ia, ib, ic);

            if (this.Colors.Count > 0)
            {
                this.AddColor(ia, ib, ic);
            }

            if (ua != null)
            {

                var uvLen = this.Uvs.Count;
                ia = this.ParseUVIndex((int)ua, uvLen);
                ib = this.ParseUVIndex((int)ub, uvLen);
                ic = this.ParseUVIndex((int)uc, uvLen);

                this.AddUV(ia, ib, ic);                               
            }

            if (na != null)
            {

                // Normals are many times the same. If so, skip function call and parseInt.
                var nLen = this.Normals.Count;
                ia = this.ParseNormalIndex((int)na, nLen);

                ib = na.Value == nb.Value ? ia : this.ParseNormalIndex((int)nb, nLen);
                ic = na.Value == nc.Value ? ia : this.ParseNormalIndex((int)nc, nLen);

                this.AddNormal(ia, ib, ic);

            }
        }

        public void AddFace(int? a, int? b, int? c, int?d ,int? ua, int? ub, int? uc, int? ud,int? na, int? nb, int? nc,int? nd ) 
        {

			var vLen = this.Vertices.Count;

			var ia = this.ParseVertexIndex((int) a, vLen );
			var ib = this.ParseVertexIndex((int) b, vLen );
			var ic = this.ParseVertexIndex((int) c, vLen );

            if (d == null)
            {
                this.AddVertex(ia, ib, ic);
            }
            else
            {
                var id = ParseVertexIndex(d.Value,vLen);

                AddVertex(ia, ib, id);
                AddVertex(ib, ic, id);
            }

			if ( this.Colors.Count > 0 ) 
            {

				this.AddColor( ia, ib, ic );

			}

			if ( ua != null) 
            {

				var uvLen = this.Uvs.Count;
				ia = this.ParseUVIndex( (int)ua, uvLen );
				ib = this.ParseUVIndex( (int)ub, uvLen );
				ic = this.ParseUVIndex( (int)uc, uvLen );

                if (d == null)
                {
                    this.AddUV(ia, ib, ic);
                }
                else
                {
                    var id = this.ParseUVIndex(ud.Value, uvLen);

                    AddUV(ia, ib, id);
                    AddUV(ib, ic, id);
                }


			}

			if ( na != null) 
            {

				// Normals are many times the same. If so, skip function call and parseInt.
				var nLen = this.Normals.Count;
				ia = this.ParseNormalIndex((int)na, nLen );

				ib = na.Value == nb.Value ? ia : this.ParseNormalIndex( (int)nb, nLen );
				ic = na.Value == nc.Value ? ia : this.ParseNormalIndex( (int)nc, nLen );

                if (d == null)
                {
                    this.AddNormal(ia, ib, ic);
                }
                else
                {
                    var id = ParseNormalIndex(nd.Value, nLen);

                    AddNormal(ia, ib, id);
                    AddNormal(ib, ic, id);
                }

			}

		}
       
    }

    public class OBJLoader
    {
        private MTLLoader.MaterialCreator Materials;

        public void SetMaterials(MTLLoader.MaterialCreator materials)
        {
            this.Materials = materials;
        }
        private ObjectState ParserState()
        {
            ObjectState state = new ObjectState();

            state.StartObject("", false);

            return state;
        }
        private float ParseFloat(string value)
        {
            return float.Parse(value);
        }

        private int? ParseInt(string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;
            return int.Parse(value);
        }

        public Group Parse(string path)
        {
            return Parse(File.ReadAllText(path), path);
        }

        public Group Parse(string objContents, string path)
        {
            Debug.WriteLine("OBJLoader");

            Stopwatch stopWatch = new Stopwatch();

            stopWatch.Start();

            var state = ParserState();

            if (objContents.IndexOf(@"\r\n") > -1)
            {
                objContents = objContents.Replace(@"\r\n", @"\n");
            }
            if (objContents.IndexOf(@"\\\n") > -1)
            {
                objContents = objContents.Replace(@"\\\n", "");
            }
            var lines = objContents.Split('\n');

            int lineLength = 0;

            char lineFirstChar;

            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i].Trim();

                lineLength = line.Length;

                if (lineLength == 0) continue;

                lineFirstChar = line[0];

                if (lineFirstChar == '#') continue;

                if (lineFirstChar == 'v')
                {
                    var data = Regex.Split(line, @"\s+");
                    switch (data[0])
                    {
                        case "v":
                            state.Vertices.Add(ParseFloat(data[1]), ParseFloat(data[2]), ParseFloat(data[3]));
                            if (data.Length >= 7)
                            {
                                state.Colors.Add(ParseFloat(data[1]), ParseFloat(data[2]), ParseFloat(data[3]));
                            }
                            break;

                        case "vn":
                            state.Normals.Add(ParseFloat(data[1]), ParseFloat(data[2]), ParseFloat(data[3]));
                            break;

                        case "vt":
                            state.Uvs.Add(ParseFloat(data[1]), ParseFloat(data[2]));
                            break;
                    }
                }
                else if (lineFirstChar == 'f')
                {
                    var lineData = line.Substring(1).Trim();
                    var vertexData = Regex.Split(lineData, @"\s+");
                    var faceVertices = new List<string[]>();

                    for (int j = 0; j < vertexData.Length; j++)
                    {
                        var vertex = vertexData[j];

                        if (vertex.Length > 0)
                        {
                            var vertexParts = vertex.Split('/');
                            faceVertices.Add(vertexParts);
                        }
                    }

                    var v1 = faceVertices[0];

                    for (int j = 1; j < faceVertices.Count - 1; j++)
                    {
                        var v2 = faceVertices[j];
                        var v3 = faceVertices[j + 1];

                        state.AddFace(
                            ParseInt(v1[0]), ParseInt(v2[0]), ParseInt(v3[0]),
                            v1.Length > 1 ? ParseInt(v1[1]) : null, v2.Length > 1 ? ParseInt(v2[1]) : null, v3.Length > 1 ? ParseInt(v3[1]) : null,
                            v1.Length > 2 ? ParseInt(v1[2]) : null, v2.Length > 2 ? ParseInt(v2[2]) : null, v3.Length > 2 ? ParseInt(v3[2]) : null
                         );
                    }
                }
                else if (lineFirstChar == 'l')
                {
                    var lineParts = line.Substring(1).Trim().Split(' ');

                    var lineVertices = new List<string>();
                    var lineUVs = new List<string>();
                    if (line.IndexOf("/") == -1)
                    {
                        lineVertices.Concat(lineParts.ToList());
                    }
                    else
                    {
                        for (int li = 0; li < lineParts.Length; li++)
                        {
                            var parts = lineParts[li].Split('/');

                            if (parts[0] != "") lineVertices.Add(parts[0]);
                            if (parts[1] != "") lineUVs.Add(parts[1]);
                        }
                    }
                    state.AddLineGeometry(lineVertices, lineUVs);
                }
                else if (lineFirstChar == 'p')
                {
                    var lineData = line.Substring(1).Trim();
                    var pointData = lineData.Split(' ');

                    state.AddPointGeometry(pointData);
                }
                else if (line.Contains("o ") || line.Contains("g "))
                {
                    var name = line.Substring(2, line.Length - 2).Trim();
                    state.StartObject(name, true);
                    continue;
                }
                else if (line.Contains("usemtl"))
                {
                    if (line.Trim() == "usemtl" || string.IsNullOrWhiteSpace(path)) continue;
                    string mtlName = line.Substring(7, line.Length - 7).Trim();
                    state.Object.StartMaterial(mtlName, state.MaterialLibraries);
                    continue;
                }
                else if (line.Contains("mtllib"))
                {
                    if (string.IsNullOrWhiteSpace(path)) continue;
                    string matFileName = line.Substring(7, line.Length - 7).Trim();
                    string directory = System.IO.Path.GetDirectoryName(path);
                    string matPath = System.IO.Path.Combine(directory, matFileName);
                    //state.Object.StartMaterial(matPath, state.MaterialLibraries);
                    state.MaterialLibraries.Add(matPath);
                    continue;
                }
                else if (lineFirstChar == 's')
                {
                    string[] result = line.Split(' ');
                    if (result.Length > 1)
                    {
                        var value = result[1].Trim().ToLower();
                        state.Object.Smooth = (value != "0" && value != "off");
                    }

                    else
                    {
                        state.Object.Smooth = true;
                    }
                    var material = state.Object.CurrentMaterial();
                    if (material != null) material.Smooth = state.Object.Smooth;
                    continue;
                }
                else
                {
                    if (line == "\0") continue;

                    throw new Exception("THREE.OBJLoader: Unexpected line:" + line);
                }
            }

            state.finalize();

            var container = new Group();

            container.MaterialLibraries = state.MaterialLibraries.ToList();

            if (container.MaterialLibraries != null && container.MaterialLibraries.Count > 0)
            {
                MTLLoader mtlLoader = new MTLLoader();
                for (int i = 0; i < container.MaterialLibraries.Count; i++)
                {
                    string mtlPath = container.MaterialLibraries[i];
                    if (File.Exists(mtlPath))
                        mtlLoader.Load(mtlPath);
                }
                this.SetMaterials(mtlLoader.MultiMaterialCreator);
            }


            for (var i = 0; i < state.Objects.Count; i++)
            {
                var Object = state.Objects[i];
                var geometry = Object.Geometry;
                var materials = Object.Materials;
                var isLine = geometry.type == "Line" ? true : false;
                var isPoints = geometry.type == "Points" ? true : false;
                var hasVertexColors = false;

                if (geometry.Vertices.Count == 0) continue;

                var bufferGeometry = new BufferGeometry();

                bufferGeometry.SetAttribute("position", new BufferAttribute<float>(geometry.Vertices.ToArray(), 3));

                if (geometry.Normals.Count > 0)
                {
                    bufferGeometry.SetAttribute("normal", new BufferAttribute<float>(geometry.Normals.ToArray(), 3));
                }
                else
                {
                    bufferGeometry.ComputeVertexNormals();
                }

                if (geometry.Colors.Count > 0)
                {
                    hasVertexColors = true;
                    bufferGeometry.SetAttribute("color", new BufferAttribute<float>(geometry.Colors.ToArray(), 3));
                }

                if (geometry.Uvs.Count > 0)
                {
                    bufferGeometry.SetAttribute("uv", new BufferAttribute<float>(geometry.Uvs.ToArray(), 2));
                }

                // Create materials

                var createdMaterials = new List<Material>();

                for (var mi = 0; mi < materials.Count; mi++)
                {

                    var sourceMaterial = materials[mi];
                    Material material = null;

                    if (this.Materials != null && this.Materials.Materials.Count > 0 && !string.IsNullOrEmpty(sourceMaterial.Name))
                    {

                        material = this.Materials.Create(sourceMaterial.Name);

                        // mtl etc. loaders probably can't create line materials correctly, copy properties to a line material.
                        if (isLine && material != null && !(material is LineBasicMaterial))
                        {

                            var materialLine = new LineBasicMaterial();
                            materialLine = (LineBasicMaterial)material.Clone();
                            materialLine.Color = material.Color;
                            material = materialLine;

                        }
                        else if (isPoints && material != null && !(material is PointsMaterial))
                        {

                            var materialPoints = new PointsMaterial() { Size = 10, SizeAttenuation = false };
                            materialPoints = (PointsMaterial)material.Clone();
                            materialPoints.Color = material.Color;
                            materialPoints.Map = material.Map;
                            material = materialPoints;
                        }

                    }
                    if (material == null)
                    {
                        if (isLine)
                        {
                            material = new LineBasicMaterial();
                        }
                        else if (isPoints)
                        {
                            material = new PointsMaterial() { Size = 1, SizeAttenuation = false };

                        }
                        else
                        {

                            material = new MeshPhongMaterial();

                        }

                        material.Name = sourceMaterial.Name;

                    }

                    material.FlatShading = sourceMaterial.Smooth ? false : true;
                    material.VertexColors = hasVertexColors;

                    createdMaterials.Add(material);

                }

                if (Object.Name.Equals("butterfly_body"))
                    Console.WriteLine("butterfly-body");

                // Create Mesh
                Mesh mesh;

                if (createdMaterials.Count > 1)
                {

                    for (var mi = 0; mi < materials.Count; mi++)
                    {

                        var sourceMaterial = materials[mi];
                        bufferGeometry.AddGroup(sourceMaterial.GroupStart, sourceMaterial.GroupCount, mi);

                    }

                    if (isLine)
                    {

                        var lineSegment = new LineSegments(bufferGeometry, createdMaterials);
                        lineSegment.Name = Object.Name;
                        container.Add(lineSegment);
                    }
                    else if (isPoints)
                    {

                        Points points = new Points(bufferGeometry, createdMaterials);
                        points.Name = Object.Name;
                        container.Add(points);

                    }
                    else
                    {

                        mesh = new Mesh(bufferGeometry, createdMaterials);
                        mesh.Name = Object.Name;
                        container.Add(mesh);

                    }

                }
                else
                {

                    if (isLine)
                    {

                        var lineSegment = new LineSegments(bufferGeometry, createdMaterials[0]);
                        lineSegment.Name = Object.Name;
                        container.Add(lineSegment);
                    }
                    else if (isPoints)
                    {

                        Points points = new Points(bufferGeometry, createdMaterials[0]);
                        points.Name = Object.Name;
                        container.Add(points);
                    }
                    else
                    {

                        mesh = new Mesh(bufferGeometry, createdMaterials[0]);
                        mesh.Name = Object.Name;
                        container.Add(mesh);
                    }

                }
            }

            stopWatch.Stop();
            Debug.WriteLine("Obj loading time : " + stopWatch.ElapsedMilliseconds + "ms");
            return container;
        }

        public Group Load(string path)
        {
            return Parse(path);
        }
    }
}

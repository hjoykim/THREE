using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;


namespace THREE
{
    public class Mesh : Object3D
    {
		private Sphere _sphere = new Sphere();		
		private Matrix4 _inverseMatrix = new Matrix4();
		private Ray _ray = new Ray();
		private Vector3 _intersectionPoint = new Vector3();
		private Vector3 _intersectionPointWorld = new Vector3();

        //public List<int> MorphTargetInfluences = new List<int>();

        //public Dictionary<string, int> MorphTargetDictionary = new Dictionary<string, int>();
        public Mesh() : base()
        {
            InitGeometry(null, null);
        }

        public Mesh(Geometry geometry = null, List<Material> materials = null)
            : base()
        {
            InitGeometries(geometry, materials);
        }

        public Mesh(Geometry geometry = null, Material material = null) : base()
        {
            InitGeometry(geometry, material);
        }

        public void InitGeometries(Geometry geometry,List<Material> materials)
        {
            this.type = "Mesh";

            if (geometry == null)
                this.Geometry = new BufferGeometry();
            else
                this.Geometry = geometry;

            if (materials == null)
            {
                this.Material = new MeshBasicMaterial() { Color = new Color().SetHex(0xffffff) };
                this.Materials.Add(Material);
            }
            else
            {
                this.Materials = materials;
                if (this.Materials.Count > 0)
                    this.Material = this.Materials[0];
            }

            this.UpdateMorphTargets();
        }

        public void InitGeometry(Geometry geometry, Material material)
        {
            this.type = "Mesh";

            if (geometry == null)
                this.Geometry = new BufferGeometry();
            else
                this.Geometry = geometry;

            if (material == null)
            {
                this.Material = new MeshBasicMaterial() { Color = new Color().SetHex(0xffffff) };
                
            }
            else
            {
                Materials.Clear();
                this.Material = material;
            }

            this.Materials.Add(Material);

            this.UpdateMorphTargets();
        }

        public void UpdateMorphTargets()
        {
            var geometry = this.Geometry as BufferGeometry;
			if (geometry != null && geometry is BufferGeometry)
			{
				var morphAttributes = geometry.MorphAttributes;
				var keys = morphAttributes.Keys;

				//if (keys.Count > 0)
				if(morphAttributes!=null && morphAttributes.Count>0)
				{

					foreach (DictionaryEntry entry in morphAttributes)
					{
						var morphAttribute = morphAttributes[entry.Key] as List<BufferAttribute<float>>;

						if (morphAttribute != null)
						{
							this.MorphTargetInfluences = new List<float>();
							this.MorphTargetDictionary = new Hashtable();

							for (int m = 0; m < morphAttribute.Count; m++)
							{
								string name = morphAttribute[m] != null ? (morphAttribute[m] as BufferAttribute<float>).Name : m.ToString();

								this.MorphTargetInfluences.Add(0);
								if (this.MorphTargetDictionary.ContainsKey(name))
									this.MorphTargetDictionary[name] = m;
								else
									this.MorphTargetDictionary.Add(name, m);
							}
						}
					}
				}
			}
			else
			{
				if (geometry!=null && geometry.MorphTargets != null && geometry.MorphTargets.Count > 0)
				{
					Trace.TraceError("THREE.Mesh.updateMorphTargets() no longer supports THREE.Geometry. Use THREE.BufferGeometry instead.");
				}
			}
        }

        public override void Raycast(Raycaster raycaster, List<Intersection> intersects)
        {
            if (Material == null) return;

			// Checking boundingSphere distance to ray

			if (Geometry.BoundingSphere == null) Geometry.ComputeBoundingSphere();

			_sphere.Copy(Geometry.BoundingSphere);
			_sphere.ApplyMatrix4(MatrixWorld);

			if (raycaster.ray.IntersectsSphere(_sphere) == false) return;

			//

			_inverseMatrix.GetInverse(MatrixWorld);
			_ray.copy(raycaster.ray).ApplyMatrix4(_inverseMatrix);

			// Check boundingBox before continuing

			if (Geometry.BoundingBox != null)
			{

				if (_ray.IntersectsBox(Geometry.BoundingBox) == false) return;

			}

			Intersection intersection;

			if (Geometry is BufferGeometry)
			{
				BufferGeometry bufferGeometry = Geometry as BufferGeometry;
				//const index = geometry.index;
				BufferAttribute<float> position = bufferGeometry.Attributes.ContainsKey("position") ? bufferGeometry.Attributes["position"] as BufferAttribute<float> : null;
				List<BufferAttribute<float>> morphPosition = bufferGeometry.MorphAttributes.ContainsKey("position") ? bufferGeometry.MorphAttributes["position"] as List<BufferAttribute<float>> : null;
				var morphTargetsRelative = bufferGeometry.MorphTargetsRelative;
				BufferAttribute<float> uv = bufferGeometry.Attributes.ContainsKey("uv") ? bufferGeometry.Attributes["uv"] as BufferAttribute<float> : null; ;
				BufferAttribute<float> uv2 = bufferGeometry.Attributes.ContainsKey("uv2") ? bufferGeometry.Attributes["uv2"] as BufferAttribute<float> : null; ;
				//const groups = geometry.groups;
				//const drawRange = geometry.drawRange;

				if (bufferGeometry.Index != null)
				{

					// indexed buffer geometry

					if (Materials.Count > 1 )
					{

						for (int i = 0;i< bufferGeometry.Groups.Count;i++)
						{

							var group = bufferGeometry.Groups[i];
							var groupMaterial = Materials[group.MaterialIndex];

							var start = (int)System.Math.Max(group.Start, bufferGeometry.DrawRange.Start);
							var end = (int)System.Math.Min((group.Start + group.Count), (bufferGeometry.DrawRange.Start + bufferGeometry.DrawRange.Count));

							for (int j = start;j< end; j += 3)
							{

								var a = bufferGeometry.Index.getX(j);
								var b = bufferGeometry.Index.getX(j + 1);
								var c = bufferGeometry.Index.getX(j + 2);

								intersection = checkBufferGeometryIntersection(this, groupMaterial, raycaster, _ray, position, morphPosition, morphTargetsRelative, uv, uv2, a, b, c);

								if (intersection!=null)
								{

									intersection.faceIndex =(int)System.Math.Floor((decimal)j / 3); // triangle number in indexed buffer semantics
									intersection.face.MaterialIndex = group.MaterialIndex;
									intersects.Add(intersection);

								}
							}
						}
					}
					else
					{
						var start = (float)System.Math.Max(0, bufferGeometry.DrawRange.Start);
						var end = (float)System.Math.Min(bufferGeometry.Index.count, (bufferGeometry.DrawRange.Start + bufferGeometry.DrawRange.Count));

						for (int i = (int)start;i<(int)end; i += 3)
						{

							int a = bufferGeometry.Index.getX(i);
							int b = bufferGeometry.Index.getX(i + 1);
							int c = bufferGeometry.Index.getX(i + 2);

							intersection = checkBufferGeometryIntersection(this, Material, raycaster, _ray, position, morphPosition, morphTargetsRelative, uv, uv2, a, b, c);
							
							if (intersection!=null)
							{

								intersection.faceIndex = (int)System.Math.Floor((decimal)i / 3); // triangle number in indexed buffer semantics
								intersects.Add(intersection);

							}

						}

					}

				}
				else if (bufferGeometry.Attributes["position"] != null)
				{

					// non-indexed buffer geometry

					if (Materials.Count > 1)
					{

						for (int i = 0;i < bufferGeometry.Groups.Count; i++)
						{

							var group = bufferGeometry.Groups[i];
							Material groupMaterial = Materials[group.MaterialIndex];

							var start = (int)System.Math.Max(group.Start, bufferGeometry.DrawRange.Start);
							var end = (int)System.Math.Min((group.Start + group.Count), (bufferGeometry.DrawRange.Start + bufferGeometry.DrawRange.Count));

							for (int j = start;j< end; j += 3)
							{

								int a = j;
								int b = j + 1;
								int c = j + 2;

								intersection = checkBufferGeometryIntersection(this, groupMaterial, raycaster, _ray, bufferGeometry.Attributes["position"] as BufferAttribute<float>, bufferGeometry.MorphAttributes["position"] as List<BufferAttribute<float>>, bufferGeometry.MorphTargetsRelative, bufferGeometry.Attributes["uv"] as BufferAttribute<float>, bufferGeometry.Attributes["uv2"] as BufferAttribute<float>, a, b, c);

								if (intersection!=null)
								{

									intersection.faceIndex = (int)System.Math.Floor((decimal)j / 3); // triangle number in non-indexed buffer semantics
									intersection.face.MaterialIndex = group.MaterialIndex;
									intersects.Add(intersection);
								}
							}
						}

					}
					else
					{

						int start = (int)System.Math.Max(0, bufferGeometry.DrawRange.Start);
						int end = (int)System.Math.Min((bufferGeometry.Attributes["position"] as BufferAttribute<float>).count, (bufferGeometry.DrawRange.Start + bufferGeometry.DrawRange.Count));

						for (int i = start;i< end; i += 3)
						{

							int a = i;
							int b = i + 1;
							int c = i + 2;

							intersection = checkBufferGeometryIntersection(this, Material, raycaster, _ray, bufferGeometry.Attributes["position"] as BufferAttribute<float>,
								bufferGeometry.MorphAttributes.ContainsKey("position")?bufferGeometry.MorphAttributes["position"] as List<BufferAttribute<float>>:null,bufferGeometry.MorphTargetsRelative,
								bufferGeometry.Attributes.ContainsKey("uv") ?bufferGeometry.Attributes["uv"] as BufferAttribute<float>:null,
								bufferGeometry.Attributes.ContainsKey("uv2")?bufferGeometry.Attributes["uv2"] as BufferAttribute<float>:null, a, b, c);

							if (intersection!=null)
							{

								intersection.faceIndex = (int)System.Math.Floor((decimal)i / 3); // triangle number in non-indexed buffer semantics
								intersects.Add(intersection);

							}

						}

					}

				}

			}
			else if (this.Geometry is Geometry)
			{

				bool isMultiMaterial = Materials.Count > 1;

				var vertices = this.Geometry.Vertices;
				var faces = this.Geometry.Faces;
				List<List<Vector2>> uvs = null;

				var faceVertexUvs = this.Geometry.FaceVertexUvs[0];
				if (faceVertexUvs.Count > 0) uvs = faceVertexUvs;

				for (int f = 0;f< faces.Count; f++)
				{

					var face = faces[f];
					var faceMaterial = isMultiMaterial ? Materials[face.MaterialIndex] : this.Material;

					if (faceMaterial == null) continue;

					Vector3 fvA = vertices[face.a];
					Vector3 fvB = vertices[face.b];
					Vector3 fvC = vertices[face.c];							

					intersection = checkIntersection(this, faceMaterial, raycaster, _ray, fvA, fvB, fvC, _intersectionPoint);
				
					if (intersection!=null)
					{

						if (uvs!=null && uvs.Count>0 && uvs[f].Count>0)
						{

							var uvs_f = uvs[f];
							_uvA.Copy(uvs_f[0]);
							_uvB.Copy(uvs_f[1]);
							_uvC.Copy(uvs_f[2]);

							intersection.uv = Triangle.GetUV(_intersectionPoint, fvA, fvB, fvC, _uvA, _uvB, _uvC, new Vector2());

						}

						intersection.face = face;
						intersection.faceIndex = f;
						intersects.Add(intersection);

					}

				}

			}

		}

		private Intersection checkIntersection(Object3D object3D,Material material,Raycaster raycaster,Ray ray,Vector3 pA,Vector3 pB,Vector3 pC,Vector3 point)
        {
			Vector3 intersect;

			if (material.Side == Constants.BackSide)
				intersect = ray.IntersectTriangle(pC, pB, pA, true, point);
			else
				intersect = ray.IntersectTriangle(pA, pB, pC, material.Side != Constants.DoubleSide, point);


			if (intersect == null) return null;

			_intersectionPointWorld.Copy(point);
			_intersectionPointWorld.ApplyMatrix4(object3D.MatrixWorld);

			float distance = raycaster.ray.origin.DistanceTo(_intersectionPointWorld);

			if (distance < raycaster.near || distance > raycaster.far) return null;

			Intersection result = new Intersection();
			result.distance= distance;
			result.point =_intersectionPointWorld.Clone();
			result.object3D = object3D;

			return result;
        }
		private Vector3 _vA = new Vector3();
		private Vector3 _vB = new Vector3();
		private Vector3 _vC = new Vector3();
		private Vector3 _morphA = new Vector3();
		private Vector3 _morphB = new Vector3();
		private Vector3 _morphC = new Vector3();
		private Vector3 _tempA = new Vector3();
		private Vector3 _tempB = new Vector3();
		private Vector3 _tempC = new Vector3();

		private Vector2 _uvA = new Vector2();
		private Vector2 _uvB = new Vector2();
		private Vector2 _uvC = new Vector2();
	
		private Intersection checkBufferGeometryIntersection(Object3D object3D, Material material, Raycaster raycaster, Ray ray, BufferAttribute<float> position, List<BufferAttribute<float>> morphPosition, bool morphTargetsRelative, BufferAttribute<float> uv, BufferAttribute<float> uv2, int a, int b, int c)
        {
			_vA.FromBufferAttribute(position, a);
			_vB.FromBufferAttribute(position, b);
			_vC.FromBufferAttribute(position, c);

			var morphInfluences = object3D.MorphTargetInfluences;

			if (material.MorphTargets==true && (morphPosition!=null && morphPosition.Count>0) && (morphInfluences!=null && morphInfluences.Count>0))
			{

				_morphA.Set(0, 0, 0);
				_morphB.Set(0, 0, 0);
				_morphC.Set(0, 0, 0);

				for (int i = 0;i< morphPosition.Count; i++)
				{

					var influence = morphInfluences[i];
					var morphAttribute = morphPosition[i];

					if (influence == 0) continue;

					_tempA.FromBufferAttribute(morphAttribute, a);
					_tempB.FromBufferAttribute(morphAttribute, b);
					_tempC.FromBufferAttribute(morphAttribute, c);

					if (morphTargetsRelative)
					{

						_morphA.AddScaledVector(_tempA, influence);
						_morphB.AddScaledVector(_tempB, influence);
						_morphC.AddScaledVector(_tempC, influence);

					}
					else
					{

						_morphA.AddScaledVector(_tempA.Sub(_vA), influence);
						_morphB.AddScaledVector(_tempB.Sub(_vB), influence);
						_morphC.AddScaledVector(_tempC.Sub(_vC), influence);

					}

				}

				_vA.Add(_morphA);
				_vB.Add(_morphB);
				_vC.Add(_morphC);

			}

			if (object3D is SkinnedMesh)
			{
				SkinnedMesh skin = object3D as SkinnedMesh;

				skin.BoneTransform(a, _vA.ToVector4());
				skin.BoneTransform(b, _vB.ToVector4());
				skin.BoneTransform(c, _vC.ToVector4());

			}

			var intersection = checkIntersection(object3D, material, raycaster, ray, _vA, _vB, _vC, _intersectionPoint);

			if (intersection!=null)
			{

				if (uv!=null)
				{

					_uvA.FromBufferAttribute(uv, a);
					_uvB.FromBufferAttribute(uv, b);
					_uvC.FromBufferAttribute(uv, c);

					intersection.uv = Triangle.GetUV(_intersectionPoint, _vA, _vB, _vC, _uvA, _uvB, _uvC, new Vector2());

				}

				if (uv2!=null)
				{

					_uvA.FromBufferAttribute(uv2, a);
					_uvB.FromBufferAttribute(uv2, b);
					_uvC.FromBufferAttribute(uv2, c);

					intersection.uv2 =Triangle.GetUV(_intersectionPoint, _vA, _vB, _vC, _uvA, _uvB, _uvC, new Vector2());

				}

				Face3 face = new Face3(a, b, c);
				Triangle.GetNormal(_vA, _vB, _vC, face.Normal);

				intersection.face = face;

			}

			return intersection;
		}
    }
}

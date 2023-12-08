using System;
using System.Collections.Generic;
using System.Diagnostics;
using THREE;

namespace THREE
{
    public class DirectGeometry : Geometry
    {
        public List<int> Indices = new List<int>();

        public DirectGeometry() : base() 
        { 
        }

        public void ComputeGroups(Geometry geometry)
        {
            var faces = geometry.Faces;
            DrawRange group = new DrawRange { Start = -1 };
            List<DrawRange> groups = new List<DrawRange>();

            int materialIndex = -1;
            int i;
            for (i = 0; i < faces.Count; i++)
            {
                var face = faces[i];

                if (face.MaterialIndex != materialIndex)
                {
                    materialIndex = face.MaterialIndex;

                    if (group.Start!=-1)
                    {

                        group.Count = (i * 3) - group.Start;
                        groups.Add(group);
                    }
                    group = new DrawRange { Start = i * 3, MaterialIndex = materialIndex };
                }
            }
            if (group.Start != -1)
            {
                group.Count = (i * 3) - group.Start;
                groups.Add(group);
            }

            this.Groups = groups;
        }

        public DirectGeometry FromGeometry(Geometry geometry)
        {
            var faces = geometry.Faces;
            var vertices = geometry.Vertices;
            var faceVertexUvs = geometry.FaceVertexUvs;

            bool hasFaceVertexUv = faceVertexUvs.Count>0 && faceVertexUvs[0] != null && faceVertexUvs[0].Count > 0;
            bool hasFaceVertexUv2 = faceVertexUvs.Count>1 && faceVertexUvs[1] != null && faceVertexUvs[1].Count > 0;


            var morphTargets = geometry.MorphTargets;

            var morphTargetsLength = geometry.MorphTargets.Count;

            List<MorphTarget> morphTargetsPosition = new List<MorphTarget>();
            List<string> morphTargetKeys = new List<string>();
            if(morphTargetsLength>0)
            {

                foreach(string key in morphTargets.Keys)
                {
                    morphTargetsPosition.Add(new MorphTarget{Name= key, Data=new List<Vector3>()});
                    morphTargetKeys.Add(key);
                }
                this.MorphTargets.Add("position",morphTargetsPosition);
            }


            var morphNormals = geometry.MorphNormals;
            var morphNormalsLength = morphNormals.Count;

            List<MorphTarget> morphTargetsNormal = new List<MorphTarget>();
            List<string> morphTargetsNormalKeys = new List<string>();
            if(morphNormalsLength>0)
            {
                foreach(string key in morphNormals.Keys)
                {
                    morphTargetsNormal.Add(new MorphTarget{Name=key,Data=new List<Vector3>()});
                    morphTargetsNormalKeys.Add(key);
                }
                this.MorphTargets.Add("normal", morphTargetsNormal);
            }

            var skinIndices = geometry.SkinIndices;
            var skinWeights = geometry.SkinWeights;

            var hasSkinIndices = skinIndices.Count == vertices.Count;
            var hasSkinWeights = skinWeights.Count == vertices.Count;

            if (vertices.Count > 0 && faces.Count == 0)
            {
                Trace.TraceError("THREE.Core.DirectGeometry:Faceless geometries are not supported.");
            }

            int vLen = vertices.Count - 1;
            for (int i = 0; i < faces.Count; i++)
            {
                var face = faces[i];
                if (face.a > vLen) continue;
                this.Vertices.Add(vertices[face.a]);
                this.Vertices.Add(vertices[face.b]);
                this.Vertices.Add(vertices[face.c]);

                var vertexNormals = face.VertexNormals;

                if (vertexNormals.Count == 3)
                {
                    this.Normals.Add(vertexNormals[0]);
                    this.Normals.Add(vertexNormals[1]);
                    this.Normals.Add(vertexNormals[2]);
                }
                else
                {
                    var normal = face.Normal;

                    this.Normals.Add(normal);
                    this.Normals.Add(normal);
                    this.Normals.Add(normal);
                }

                var vertexColors = face.VertexColors;

                if (vertexColors.Count == 3)
                {
                    this.Colors.Add(vertexColors[0]);
                    this.Colors.Add(vertexColors[1]);
                    this.Colors.Add(vertexColors[2]);
                }
                else
                {
                    var color = face.Color;

                    this.Colors.Add(color);
                    this.Colors.Add(color);
                    this.Colors.Add(color);
                }

                if (hasFaceVertexUv == true)
                {
                    var vertexUvs = faceVertexUvs[0][i];

                    if (vertexUvs.Count > 0)
                    {
                        this.Uvs.Add(vertexUvs[0]);
                        this.Uvs.Add(vertexUvs[1]);
                        this.Uvs.Add(vertexUvs[2]);
                    }
                    else
                    {
                        Trace.TraceError("THREE.Core.DirectGeometry.FromGeometry():undefined vertexUV");

                        this.Uvs.Add(Vector2.Zero());
                        this.Uvs.Add(Vector2.Zero());
                        this.Uvs.Add(Vector2.Zero());
                    }
                }

                if (hasFaceVertexUv2 == true)
                {
                    var vertexUvs = faceVertexUvs[1][i];

                    if (vertexUvs.Count > 0)
                    {
                        this.Uvs2.Add(vertexUvs[0]);
                        this.Uvs2.Add(vertexUvs[1]);
                        this.Uvs2.Add(vertexUvs[2]);
                    }
                    else
                    {
                        Trace.TraceError("THREE.Core.DirectGeometry.FromGeometry():undefined vertexUV2");

                        this.Uvs2.Add(Vector2.Zero());
                        this.Uvs2.Add(Vector2.Zero());
                        this.Uvs2.Add(Vector2.Zero());
                    }
                }

                // morphs
                for(int j=0;j<morphTargetKeys.Count;j++)
                {
                    string key = morphTargetKeys[j];
                    var morphTarget = geometry.MorphTargets[key] as List<Vector3>;

                    (morphTargetsPosition[j].Data as List<Vector3>).Add(morphTarget[face.a]);
                    (morphTargetsPosition[j].Data as List<Vector3>).Add(morphTarget[face.b]);
                    (morphTargetsPosition[j].Data as List<Vector3>).Add(morphTarget[face.c]);
                }

                for(int j=0;j<morphTargetsNormalKeys.Count;j++)
                {
                    string key = morphTargetsNormalKeys[j];

                    var morphNormal = ((MorphTarget)geometry.MorphNormals[key]).Data;

                    ((MorphTarget)morphNormals[key]).Data.Add(morphNormal[face.a]);
                    ((MorphTarget)morphNormals[key]).Data.Add(morphNormal[face.b]);
                    ((MorphTarget)morphNormals[key]).Data.Add(morphNormal[face.c]);
                }

                //skins
                if (hasSkinIndices)
                {
                    this.SkinIndices.Add(skinIndices[face.a]);
                    this.SkinIndices.Add(skinIndices[face.b]);
                    this.SkinIndices.Add(skinIndices[face.c]);
                }
                if (hasSkinWeights)
                {
                    this.SkinWeights.Add(skinWeights[face.a]);
                    this.SkinWeights.Add(skinWeights[face.b]);
                    this.SkinWeights.Add(skinWeights[face.c]);
                }            
            }

            this.ComputeGroups(geometry);

            this.VerticesNeedUpdate = geometry.VerticesNeedUpdate;
            this.NormalsNeedUpdate = geometry.NormalsNeedUpdate;
            this.ColorsNeedUpdate = geometry.ColorsNeedUpdate;
            this.UvsNeedUpdate = geometry.UvsNeedUpdate;

            if (geometry.BoundingSphere != null)
            {
                this.BoundingSphere = (Sphere)geometry.BoundingSphere.Clone();
            }

            if (geometry.BoundingBox != null)
            {
                this.BoundingBox = (Box3)geometry.BoundingBox.Clone();
            }

            return this;
        }


        #region IDisposable Members
        /// <summary>
        /// Implement the IDisposable interface
        /// </summary>
        public override void Dispose()
        {
            this.Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue 
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }        
        #endregion
    }
}

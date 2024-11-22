using Assimp;
using Assimp.Unmanaged;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace THREE
{
    public class AssimpLoader
    {
        HashSet<int> lineMaterialSet = new HashSet<int>();
        HashSet<int> pointMaterialSet = new HashSet<int>();
        Material basicMaterial = new MeshBasicMaterial();
        Dictionary<int, Material> threeMaterials = new Dictionary<int, Material>();
        string FilePath;

        public Group Load(string path)
        {
            AssimpContext importer = new AssimpContext();           
            
            var scene = importer.ImportFile(path,PostProcessSteps.GenerateSmoothNormals | PostProcessSteps.Triangulate);
            this.FilePath = path;

            lineMaterialSet.Clear();
            pointMaterialSet.Clear();

            basicMaterial.Color = Color.Hex(0xffff00);
          
            foreach(Assimp.Mesh m in scene.Meshes)
            {
                if (m.PrimitiveType == PrimitiveType.Point)
                    pointMaterialSet.Add(m.MaterialIndex);
                else if (m.PrimitiveType == PrimitiveType.Line)
                    lineMaterialSet.Add(m.MaterialIndex);
            }

            CreateMaterial(scene, lineMaterialSet, pointMaterialSet);

            return ProcessNode(scene.RootNode, scene);
        }
        private Group ProcessNode(Assimp.Node node,Assimp.Scene scene)
        {
            Group group = new Group();
            group.Name = node.Name;

            for(var i = 0; i < node.MeshCount; i++)
            {
                var mesh = scene.Meshes[node.MeshIndices[i]];
                group.Add(ProcessMesh(mesh, scene));
            }

            for(var i=0;i<node.ChildCount;i++)
            {
                var child = ProcessNode(node.Children[i], scene);   
                group.Add(child);
            }
            return group;

        }
        private Object3D ProcessMesh(Assimp.Mesh m,Assimp.Scene scene)
        {
            List<Vector3D> verts = m.Vertices;
            List<Vector3D> norms = m.Normals;
            List<Vector3D> uvs = m.HasTextureCoords(0) ? m.TextureCoordinateChannels[0] : null;
            List<float> mpositions = new List<float>();
            List<float> mnormals = new List<float>();
            List<float> muvs = new List<float>();
            List<int> mindex = new List<int>();

            for(int i=0;i<verts.Count; i++)
            {
                Vector3D pos = verts[i];
                Vector3D norm = m.HasNormals ? norms[i] : new Vector3D(0, 0, 0);
                Vector3D uv = m.HasTextureCoords(0) ? uvs[i] : new Vector3D(0, 0, 0);

                mpositions.Add(pos.X, pos.Y, pos.Z);
                mnormals.Add(norm.X, norm.Y, norm.Z);
                muvs.Add(uv.X, uv.Y);
            }
            List<Face> faces = m.Faces;
            for (int i = 0; i < faces.Count; i++)
            {
                Face f = faces[i];
                if(f.IndexCount!=3)
                {
                    mindex.Add(0, 0, 0);
                    continue;
                }
                mindex.Add(f.Indices[0], f.Indices[1], f.Indices[2]);
            }

            var geometry = new BufferGeometry();
            geometry.SetIndex(new BufferAttribute<int>(mindex.ToArray(), 1));
            
            geometry.SetAttribute("position", new BufferAttribute<float>(mpositions.ToArray(), 3));
            
            if (m.HasNormals)
                geometry.SetAttribute("normal", new BufferAttribute<float>(mnormals.ToArray(), 3));
            
            if(m.HasTextureCoords(0))
                geometry.SetAttribute("uv",new BufferAttribute<float>(muvs.ToArray(),2));

            var material = basicMaterial;
            if(scene.HasMaterials)
                material = threeMaterials[m.MaterialIndex];

            
            if (m.PrimitiveType == PrimitiveType.Line)
                return new LineSegments(geometry, material);
            else if(m.PrimitiveType == PrimitiveType.Point)
                return new Points(geometry, material);
            else
            {
                //geometry.ComputeVertexNormals();
                return new Mesh(geometry, material);
            }            
        }

        private void CreateMaterial(Assimp.Scene scene,HashSet<int> linesSet,HashSet<int> pointsSet)
        {
            threeMaterials.Clear();

            for (int i = 0; i < scene.Materials.Count; i++)
            {
                Assimp.Material m = scene.Materials[i];
                if(!linesSet.Contains(i) && !pointsSet.Contains(i))
                {
                    Hashtable parameter = new Hashtable()
                    {
                        {"name", m.Name },
                        {"side",m.HasTwoSided ? Constants.DoubleSide : Constants.FrontSide },
                        {"wireframe",m.IsWireFrameEnabled ? true:false },
                        {"color",m.HasColorDiffuse ? new Color(m.ColorDiffuse[0],m.ColorDiffuse[1],m.ColorDiffuse[2]) : null },
                        {"specular",m.HasColorSpecular ? new Color(m.ColorSpecular[0],m.ColorSpecular[1],m.ColorSpecular[2]) : null },
                        {"emissive",m.HasColorEmissive ? new Color(m.ColorEmissive[0],m.ColorEmissive[1],m.ColorEmissive[2]) :null },
                        {"transparent",m.HasTransparencyFactor? true :false }                        
                    };
                    if(m.HasShininess) parameter["shininess"] = m.Shininess;
                    if (m.HasOpacity) parameter["opacity"] = m.Opacity;
                    if(m.HasTextureDiffuse)
                    {
                        var textures = LoadMaterialTexture(m, Assimp.TextureType.Diffuse);
                        parameter["map"] = textures[0];
                    }
                    if (m.HasTextureSpecular)
                    {
                        var textures = LoadMaterialTexture(m, Assimp.TextureType.Specular);
                        parameter["map"] = textures[0];
                    }
                    if(m.HasTextureEmissive)
                    {
                        var textures = LoadMaterialTexture(m, Assimp.TextureType.Emissive);
                        parameter["map"] = textures[0];
                    }
                    if(m.HasTextureNormal)
                    {
                        var textures = LoadMaterialTexture(m, Assimp.TextureType.Normals);
                        parameter["map"] = textures[0];
                    }
                    var material = new MeshPhongMaterial(parameter);
                    threeMaterials[i] = material;
                }
                else if(linesSet.Contains(i))
                {
                    var material = new LineBasicMaterial();
                    if(m.HasColorDiffuse)
                        material.Color = new Color(m.ColorDiffuse[0], m.ColorDiffuse[1], m.ColorDiffuse[2]);
                    threeMaterials[i] = material;
                }
                else if(pointsSet.Contains(i))
                {
                    var material = new PointsMaterial() { Size = 10, SizeAttenuation = false };
                    if (m.HasColorDiffuse)
                        material.Color = new Color(m.ColorDiffuse[0], m.ColorDiffuse[1], m.ColorDiffuse[2]);
                    if (m.HasTextureDiffuse)
                    {
                        var textures = LoadMaterialTexture(m, Assimp.TextureType.Diffuse);
                        material.Map = textures[0];
                    }
                    threeMaterials[i] = material;
                }
            }
        }

        private List<Texture> LoadMaterialTexture(Assimp.Material m,Assimp.TextureType texType)
        {
            var textureCount = m.GetMaterialTextureCount(texType);
            List<Texture> textures = new List<Texture>();
            for(int texIndex = 0; texIndex < textureCount; texIndex++)
            {
                string texName = Assimp.Material.CreateFullyQualifiedName(AiMatKeys.TEXBLEND_BASE, texType, texIndex);
                MaterialProperty texNameProp = m.GetProperty(texName);
                string filePath = texNameProp.GetStringValue();
                string texFilePath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(FilePath), filePath);
                Texture texture = TextureLoader.Load(texFilePath);
                textures.Add(texture);
            }
            return textures;
        }
    }
}

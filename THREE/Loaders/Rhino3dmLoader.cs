using Rhino.DocObjects;
using Rhino.FileIO;
using Rhino.Geometry;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using THREE;
using SkiaSharp;
using System.Diagnostics;
using System.Linq;
using System.IO;
namespace THREE
{
    //Intermediate Geometry between Three and Rhino
    class RhinoGeometry : BufferGeometry
    {
        public object Geometry { get; set; }
        public RhinoGeometry() : base() { }
        public RhinoGeometry(object geometry) : base()
        {
            Geometry = geometry;
        }
    }
    //Intermediate Textrue between Three and Rhino
    class RhinoTexture
    {
        public TextureType TextureType { get; set; }
        public string FileName { get; set; }
        public RhinoTexture() { }
    }
    //Intermediate Material between Three and Rhino
    class RhinoMaterial
    {
        public Rhino.DocObjects.Material Material { get; set; }
        public Rhino.DocObjects.PhysicallyBasedMaterial PhysicallyBasedMaterial { get; set; }
        public List<RhinoTexture> Textures { get; set; }
        public RhinoMaterial() { }
        public RhinoMaterial(Rhino.DocObjects.Material m)
        {
            Material = m;
        }
    }
    public class Rhino3dmLoader
    {
        private Dictionary<string,Material> Materials = new Dictionary<string,Material>();
        public Rhino3dmLoader() { }

        public Object3D Load(string filePath)
        {
            File3dm file3dm = File3dm.Read(filePath);
            var data = DecodeObjects(file3dm);
            Object3D object3d = CreateGeometry(file3dm,data);
            return object3d;
        }
        private Hashtable DecodeObjects(File3dm doc)
        {
            List<Hashtable> objects = new List<Hashtable>();
            var materials = new List<RhinoMaterial>();
            var layers = new List<Layer>();
            var views = new List<ViewInfo>();
            var namedViews = new List<ViewInfo>();
            var groups = new List<Rhino.DocObjects.Group>();
            var strings = new List<string>();

            //Handle objects
            var objsEnumerator = doc.Objects.GetEnumerator();
            while (objsEnumerator.MoveNext())
            {
                var _object = objsEnumerator.Current;
                var obj = ExtractObjectData(_object, doc);

                if (obj != null)
                {
                    objects.Add(obj);
                }
            }

            // Handle instance definitions
            Debug.WriteLine($"Instance Definitions Count: {doc.InstanceDefinitions.Count}");

            for (var i = 0; i < doc.InstanceDefinitions.Count; i++)
            {
                var idef = doc.InstanceDefinitions[i];

                objects.Add(new Hashtable { { "geometry", null }, { "attributes", idef }, { "objectType", ObjectType.InstanceDefinition } });
            }

            List<TextureType> textureTypes = new List<TextureType>{
			    // rhino.TextureType.Bitmap,
			        TextureType.Diffuse,
                    TextureType.Bump,
                    TextureType.Transparency,
                    TextureType.Opacity,
                    TextureType.Emap
                };

            List<TextureType> pbrTextureTypes = new List<TextureType>{
                    TextureType.PBR_BaseColor,
                    TextureType.PBR_Subsurface,
                    TextureType.PBR_SubsurfaceScattering,
                    TextureType.PBR_SubsurfaceScatteringRadius,
                    TextureType.PBR_Metallic,
                    TextureType.PBR_Specular,
                    TextureType.PBR_SpecularTint,
                    TextureType.PBR_Roughness,
                    TextureType.PBR_Anisotropic,
                    TextureType.PBR_Anisotropic_Rotation,
                    TextureType.PBR_Sheen,
                    TextureType.PBR_SheenTint,
                    TextureType.PBR_Clearcoat,
                    TextureType.PBR_ClearcoatBump,
                    TextureType.PBR_ClearcoatRoughness,
                    TextureType.PBR_OpacityIor,
                    TextureType.PBR_OpacityRoughness,
                    TextureType.PBR_Emission,
                    TextureType.PBR_AmbientOcclusion,
                    TextureType.PBR_Displacement
                };
            for (var i = 0; i < doc.Materials.Count; i++)
            {
                var _material = doc.Materials[i];
                var _pbrMaterial = _material.PhysicallyBased;
                var material = new RhinoMaterial(_material);
                var textures = new List<RhinoTexture>();

                for (var j = 0; j < textureTypes.Count; j++)
                {
                    var _texture = _material.GetTexture(textureTypes[j]);
                    if (_texture != null)
                    {
                        var textureType = textureTypes[j];
                        var texture = new RhinoTexture();
                        texture.FileName = _texture.FileName;
                        texture.TextureType = textureType;
                        textures.Add(texture);
                    }
                }

                material.Textures = textures;
                if (_pbrMaterial != null)
                {

                    for (var j = 0; j < pbrTextureTypes.Count; j++)
                    {
                        var _texture = _material.GetTexture(pbrTextureTypes[j]);
                        if (_texture != null)
                        {

                            var texture = new RhinoTexture();
                            texture.FileName = _texture.FileName;
                            texture.TextureType = pbrTextureTypes[j];
                            textures.Add(texture);
                        }
                    }

                    //var pbMaterialProperties = ExtractProperties(_material.PhysicallyBased);

                    //material.PhysicallyBasedMaterial = _material.PhysicallyBased;

                }
                materials.Add(material);
            }
            // Handle layers
            var allLayers = doc.AllLayers.ToList();
            for (var i = 0; i < allLayers.Count; i++)
            {
                var _layer = allLayers[i];
                layers.Add(_layer);
            }

            // Handle views

            for (var i = 0; i < doc.Views.Count; i++)
            {
                var _view = doc.Views[i];
                views.Add(_view);
            }

            // Handle named views

            for (var i = 0; i < doc.NamedViews.Count; i++)
            {
                var _namedView = doc.NamedViews[i];
                namedViews.Add(_namedView);
            }
            // Handle groups
            var enumerator = doc.AllGroups.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var _group = enumerator.Current;
                groups.Add(_group);
            }
            return new Hashtable { { "objects", objects }, { "materials", materials }, { "layers", layers }, { "views", views }, { "namedViews", namedViews }, { "groups", groups }, { "strings", strings }, { "settings", doc.Settings } };
        }
        private System.Drawing.Color GetColor(File3dm doc, ObjectAttributes attributes)
        {
            switch (attributes.ColorSource)
            {
                case ObjectColorSource.ColorFromMaterial:
                    var material = doc.Materials[attributes.MaterialIndex];
                    return material.DiffuseColor;
                case ObjectColorSource.ColorFromObject:
                    return attributes.ObjectColor;
                case ObjectColorSource.ColorFromLayer:
                    return doc.AllLayers.ToList()[attributes.LayerIndex].Color;
                default:
                    return attributes.ObjectColor;
            }

        }
        private Hashtable ExtractObjectData(File3dmObject obj, File3dm doc)
        {

            var _geometry = obj.Geometry;
            var _attributes = obj.Attributes;
            var objectType = _geometry.ObjectType;
            BufferGeometry geometry = null;


            Rhino.Geometry.Mesh mesh;

            // skip instance definition objects
            //if( _attributes.isInstanceDefinitionObject ) { continue; }

            // TODO: handle other geometry types
            switch (objectType)
            {

                case ObjectType.Curve:

                    var pts = CurveToPoints((Rhino.Geometry.Curve)_geometry, 100);

                    var array = new List<float>();

                    for (var j = 0; j < pts.Count; j++)
                    {
                        array.Add((float)pts[j].X);
                        array.Add((float)pts[j].Y);
                        array.Add((float)pts[j].Z);
                    }

                    geometry = new BufferGeometry();
                    geometry.SetAttribute("position", new BufferAttribute<float>(array.ToArray(), 3));

                    break;

                case ObjectType.Point:

                    var pt = (_geometry as Rhino.Geometry.Point).Location;
                    var pointArray = new float[3] { (float)pt.X, (float)pt.Y, (float)pt.Z };
                    var _color = GetColor(doc,_attributes);//_attributes.DrawColor(doc);
                    float[] pointColorArray = new float[3] { _color.R / 255.0f, _color.G / 255.0f, _color.B / 255.0f };

                    geometry = new BufferGeometry();
                    geometry.SetAttribute("position", new BufferAttribute<float>(pointArray, 3));
                    geometry.SetAttribute("color", new BufferAttribute<float>(pointColorArray, 3));

                    break;

                case ObjectType.PointSet:
                    var pointSets = (_geometry as Rhino.Geometry.PointCloud).GetPoints();
                    var pointSetColors = (_geometry as Rhino.Geometry.PointCloud).GetColors();
                    var pointsArray = new List<float>();
                    for (int i = 0; i < pointSets.Length; i++)
                    {
                        pointsArray.Add((float)pointSets[i].X, (float)pointSets[i].Y, (float)pointSets[i].Z);
                    }
                    var pointColors = new List<float>();
                    for (int i = 0; i < pointSetColors.Length; i++)
                    {
                        pointColors.Add(pointSetColors[i].R / 255.0f, pointSetColors[i].G / 255.0f, pointSetColors[i].B / 255.0f);
                    }
                    geometry = new BufferGeometry();
                    geometry.SetAttribute("position", new BufferAttribute<float>(pointsArray.ToArray(), 3));
                    geometry.SetAttribute("color", new BufferAttribute<float>(pointColors.ToArray(), 3));
                    break;
                case ObjectType.Mesh:

                    geometry = ToThreeGeometry(_geometry as Rhino.Geometry.Mesh);

                    break;

                case ObjectType.Brep:

                    var faces = (_geometry as Rhino.Geometry.Brep).Faces;
                    mesh = new Rhino.Geometry.Mesh();

                    for (var faceIndex = 0; faceIndex < faces.Count; faceIndex++)
                    {
                        var face = faces[faceIndex];
                        var _mesh = face.GetMesh(MeshType.Any);

                        if (_mesh != null)
                        {
                            mesh.Append(_mesh);
                        }
                    }

                    if (mesh.Faces.Count > 0)
                    {
                        mesh.Compact();
                        geometry = ToThreeGeometry(mesh);
                    }
                    break;

                case ObjectType.Extrusion:
                    mesh = (_geometry as Rhino.Geometry.Extrusion).GetMesh(MeshType.Any);

                    if (mesh != null)
                    {
                        geometry = ToThreeGeometry(mesh);
                    }
                    break;

                case ObjectType.TextDot:
                    var textGeometry = _geometry as Rhino.Geometry.TextDot;
                    geometry = new RhinoGeometry(textGeometry);
                    break;

                case ObjectType.Light:

                    var lightGeometry = _geometry as Rhino.Geometry.Light;
                    geometry = new RhinoGeometry(lightGeometry);
                    break;

                case ObjectType.InstanceReference:
                    var instReferenceGeometry = _geometry as Rhino.Geometry.InstanceReferenceGeometry;
                    geometry = new RhinoGeometry(instReferenceGeometry);
                    var xform = instReferenceGeometry.Xform;
                    geometry.UserData["xform"] = xform;
                    break;

                case ObjectType.SubD:

                    // TODO: precalculate resulting vertices and faces and warn on excessive results
                    var subDGeometry = _geometry as Rhino.Geometry.SubD;
                    subDGeometry.Subdivide(3);
                    mesh = Rhino.Geometry.Mesh.CreateFromSubDControlNet(subDGeometry);
                    if (mesh != null)
                    {
                        geometry = ToThreeGeometry(mesh);
                    }
                    break;

                /*
                case rhino.ObjectType.Annotation:
                case rhino.ObjectType.Hatch:
                case rhino.ObjectType.ClipPlane:
                */

                default:
                    break;
            }

            if (geometry != null)
            {
                return new Hashtable() { { "geometry", geometry }, { "attributes", _attributes }, { "objectType", objectType } };
            }
            return null;
        }
        private BufferGeometry ToThreeGeometry(Rhino.Geometry.Mesh mesh)
        {

            var data = new Hashtable();
            var indexList = new List<int>();
            for (int i = 0; i < mesh.Faces.Count; i++)
            {
                var face = mesh.Faces[i];
                indexList.Add(face[0], face[1], face[2]);
                if (face[2] != face[3])
                    indexList.Add(face[2], face[3], face[0]);
            }
            var positionList = new float[mesh.Vertices.Count * 3];
            for (int i = 0; i < mesh.Vertices.Count; i++)
            {
                positionList[i * 3] = mesh.Vertices[i].X;
                positionList[i * 3 + 1] = mesh.Vertices[i].Y;
                positionList[i * 3 + 2] = mesh.Vertices[i].Z;
            }
            var normalList = new float[mesh.Normals.Count * 3];
            for (int i = 0; i < mesh.Normals.Count; i++)
            {
                normalList[i * 3] = mesh.Normals[i].X;
                normalList[i * 3 + 1] = mesh.Normals[i].Y;
                normalList[i * 3 + 2] = mesh.Normals[i].Z;
            }

            List<float> uvList = null;
            if (mesh.HasCachedTextureCoordinates)
            {
                var tcList = new float[mesh.TextureCoordinates.Count * 2];
                for (int i = 0; i < mesh.TextureCoordinates.Count; i++)
                {
                    tcList[i * 2] = mesh.TextureCoordinates[i].X;
                    tcList[i * 2 + 1] = mesh.TextureCoordinates[i].Y;
                }
                uvList = new List<float>(tcList);
            }

            List<float> colorList = null;
            if (mesh.VertexColors != null && mesh.VertexColors.Count > 0)
            {
                var vcsList = new float[mesh.VertexColors.Count * 3];
                for (int i = 0; i < mesh.VertexColors.Count; i++)
                {
                    vcsList[i * 3] = mesh.VertexColors[i].R / 255.0f;
                    vcsList[i * 3 + 1] = mesh.VertexColors[i].G / 255.0f;
                    vcsList[i * 3 + 2] = mesh.VertexColors[i].B / 255.0f;
                }
                colorList = new List<float>(vcsList);
            }

            BufferGeometry geometry = new BufferGeometry();
            geometry.SetAttribute("position", new BufferAttribute<float>(positionList, 3));
            geometry.SetAttribute("normal", new BufferAttribute<float>(normalList, 3));
            if (colorList != null) geometry.SetAttribute("color", new BufferAttribute<float>(colorList.ToArray(), 3));
            if (uvList != null) geometry.SetAttribute("uv", new BufferAttribute<float>(uvList.ToArray(), 2));
            geometry.SetIndex(new BufferAttribute<int>(indexList.ToArray(), 1));

            return geometry;
        }
        private List<Rhino.Geometry.Point3d> CurveToPoints(Rhino.Geometry.Curve curve, int pointLimit)
        {

            int pointCount = pointLimit;
            List<Point3d> rc = new List<Point3d>();
            List<float> ts = new List<float>();

            if (curve is Rhino.Geometry.LineCurve)
            {
                var lineCurve = (Rhino.Geometry.LineCurve)curve;
                return new List<Point3d>() { lineCurve.PointAtStart, lineCurve.PointAtEnd };
            }

            if (curve is Rhino.Geometry.PolylineCurve)
            {
                var polylineCurve = (PolylineCurve)curve;
                pointCount = polylineCurve.PointCount;
                for (int i = 0; i < pointCount; i++)
                {

                    rc.Add(polylineCurve.Point(i));

                }
                return rc;
            }

            if (curve is Rhino.Geometry.PolyCurve)
            {
                var polyCurve = (PolyCurve)curve;
                int segmentCount = polyCurve.SegmentCount;

                for (var i = 0; i < segmentCount; i++)
                {

                    var segment = polyCurve.SegmentCurve(i);
                    var segmentArray = CurveToPoints(segment, pointCount);
                    rc = rc.Concat(segmentArray);
                }

                return rc;
            }

            if (curve is Rhino.Geometry.ArcCurve)
            {
                var arcCurve = (Rhino.Geometry.ArcCurve)curve;
                pointCount = (int)Math.Floor(arcCurve.AngleDegrees / 5);
                pointCount = pointCount < 2 ? 2 : pointCount;
                // alternative to this hardcoded version: https://stackoverflow.com/a/18499923/2179399

            }
            if (curve is Rhino.Geometry.NurbsCurve && (curve as Rhino.Geometry.NurbsCurve).Degree == 1)
            {

                var nurbsCurve = (Rhino.Geometry.NurbsCurve)curve;
                Polyline pLine;
                nurbsCurve.TryGetPolyline(out pLine);

                for (var i = 0; i < pLine.Count; i++)
                {

                    rc.Add(pLine.PointAt(i));

                }
                return rc;
            }

            var domain = curve.Domain;
            var divisions = pointCount - 1.0f;

            for (var j = 0; j < pointCount; j++)
            {

                var t = (float)(domain[0] + (j / divisions) * (domain[1] - domain[0]));

                if (t == domain[0] || t == domain[1])
                {
                    ts.Add(t);
                    continue;
                }
                if (ts.Count == 0) continue;
                var tan = curve.TangentAt(t);
                var clonedTs = new List<float>(ts);
                var prevTan = curve.TangentAt(ts[clonedTs.Count - 1]);
                // Duplicated from THREE.Vector3
                // How to pass imports to worker?
                double x = tan[0] * tan[0];
                double y = tan[1] * tan[1];
                double z = tan[2] * tan[2];
                double tS = (tan[0] * tan[0] + tan[1] * tan[1] + tan[2] * tan[2]);
                double ptS = (prevTan[0] * prevTan[0] + prevTan[1] * prevTan[1] + prevTan[2] * prevTan[2]);
                var denominator = Math.Sqrt(tS * ptS);

                double angle;

                if (denominator == 0)
                {

                    angle = (float)Math.PI / 2;

                }
                else
                {

                    double theta = (tan.X * prevTan.X + tan.Y * prevTan.Y + tan.Z * prevTan.Z) / denominator;
                    angle = Math.Acos(Math.Max(-1, Math.Min(1, theta)));

                }

                //if (angle < 0.05) continue;

                ts.Add(t);

            }

            //rc = ts.map(t => curve.PointAt(t));

            if (rc.Count > 0) rc.Clear();

            for (int i = 0; i < ts.Count; i++)
            {
                rc.Add(curve.PointAt(ts[i]));
            }
            return rc;

        }
        private Object3D CreateGeometry(File3dm doc,Hashtable data)
        {
            var object3d = new Object3D();
            var instanceDefinitionObjects = new List<Object3D>();
            var instanceDefinitions = new List<Hashtable>();
            var instanceReferences = new List<Hashtable>();

            object3d.UserData["layers"] = data["layers"];
            object3d.UserData["groups"] = data["groups"];
            object3d.UserData["settings"] = data["settings"];
            object3d.UserData["objectType"] = "File3dm";
            object3d.UserData["materials"] = null;

            var objects = (List<Hashtable>)data["objects"];
            var materials = (List<RhinoMaterial>)data["materials"];
            var layers = (List<Layer>)data["layers"];
            for (var i = 0; i < objects.Count; i++)
            {

                var obj = objects[i];
                var attributes = (ObjectAttributes)obj["attributes"];
                ObjectType objectType = (ObjectType)obj["objectType"];
                switch (objectType)
                {

                    case ObjectType.InstanceDefinition:
                        instanceDefinitions.Add(obj);
                        break;
                    case ObjectType.InstanceReference:
                        instanceReferences.Add(obj);
                        break;
                    default:
                        Object3D _object = null;
                        int materialIndex = attributes.MaterialIndex;
                        if (materialIndex >= 0)
                        {
                            var rMaterial = materials[materialIndex];
                            var material = CreateMaterial(rMaterial);
                            material = CompareMaterials(material);
                            _object = CreateObject(doc,obj, material);
                        }
                        else
                        {
                            var material = CreateMaterial();
                            _object = CreateObject(doc,obj, material);
                        }
                        if (_object == null)
                        {
                            continue;
                        }

                        var layer = layers[attributes.LayerIndex];
                        _object.Visible = layer.IsVisible;
                        if (attributes.IsInstanceDefinitionObject)
                        {
                            instanceDefinitionObjects.Add(_object);
                        }
                        else
                        {
                            object3d.Add(_object);
                        }
                        break;
                }
            }

            for (var i = 0; i < instanceDefinitions.Count; i++)
            {
                var iDef = instanceDefinitions[i];
                var instanceDefinitionGeometry = (InstanceDefinitionGeometry)iDef["attributes"];
                var object3dList = new List<Object3D>();
                Guid[] objectIds = instanceDefinitionGeometry.GetObjectIds();
                for (var j = 0; j < objectIds.Length; j++)
                {
                    var objId = objectIds[j];
                    for (var p = 0; p < instanceDefinitionObjects.Count; p++)
                    {
                        var idoId = (instanceDefinitionObjects[p].UserData["attributes"] as ObjectAttributes).ObjectId;
                        if (objId == idoId)
                        {
                            object3dList.Add(instanceDefinitionObjects[p]);
                        }
                    }
                }

                // Currently clones geometry and does not take advantage of instancing

                for (var j = 0; j < instanceReferences.Count; j++)
                {
                    var iRefObj = (Hashtable)instanceReferences[j];
                    var iRef = iRefObj["geometry"] as RhinoGeometry;
                    var iRefGeometry = iRef.Geometry as InstanceReferenceGeometry;
                    if (iRefGeometry.ParentIdefId == instanceDefinitionGeometry.Id)
                    {
                        var iRefObject = new Object3D();
                        var xf = iRefGeometry.Xform.ToFloatArray(true);

                        var matrix = new Matrix4();
                        matrix.Set(xf[0], xf[1], xf[2], xf[3], xf[4], xf[5], xf[6], xf[7], xf[8], xf[9], xf[10], xf[11], xf[12], xf[13], xf[14], xf[15]);

                        iRefObject.ApplyMatrix4(matrix);

                        for (var p = 0; p < object3dList.Count; p++)
                        {

                            iRefObject.Add((Object3D)object3dList[p].Clone());

                        }

                        object3d.Add(iRefObject);

                    }

                }

            }

            object3d.UserData["materials"] = materials;

            return object3d;

        }
        private Object3D CreateObject(File3dm doc,Hashtable obj, THREE.Material mat)
        {

            var attributes = (ObjectAttributes)obj["attributes"];
            BufferGeometry geometry;
            THREE.Color color;
            THREE.Material material;
            System.Drawing.Color _color;

            ObjectType objectType = (ObjectType)obj["objectType"];
            switch (objectType)
            {

                case ObjectType.Point:
                case ObjectType.PointSet:

                    geometry = (BufferGeometry)obj["geometry"];

                    if (geometry.Attributes.ContainsKey("color"))
                    {

                        material = new PointsMaterial { VertexColors = true, SizeAttenuation = false, Size = 2 };

                    }
                    else
                    {
                        _color = GetColor(doc, attributes);
                        color = new THREE.Color(_color.R / 255.0f, _color.G / 255.0f, _color.B / 255.0f);
                        material = new PointsMaterial { Color = color, SizeAttenuation = false, Size = 2 };
                    }

                    material = CompareMaterials(material);

                    var points = new Points(geometry, material);
                    points.UserData["attributes"] = attributes;
                    points.UserData["objectType"] = obj["objectType"];

                    if (attributes.Name != null)
                    {
                        points.Name = attributes.Name;
                    }

                    return points;

                case ObjectType.Mesh:
                case ObjectType.Extrusion:
                case ObjectType.SubD:
                case ObjectType.Brep:

                    if (!obj.ContainsKey("geometry") || obj["geometry"] == null) return null;

                    geometry = (BufferGeometry)obj["geometry"];

                    if (geometry.Attributes.ContainsKey("color"))
                    {
                        mat.VertexColors = true;
                    }

                    if (mat == null)
                    {
                        mat = CreateMaterial();
                        mat = CompareMaterials(mat);
                    }

                    var mesh = new THREE.Mesh(geometry, mat);
                    mesh.CastShadow = attributes.CastsShadows;
                    mesh.ReceiveShadow = attributes.ReceivesShadows;
                    mesh.UserData["attributes"] = attributes;
                    mesh.UserData["objectType"] = obj["objectType"];
                    if (attributes.Name != null)
                    {
                        mesh.Name = attributes.Name;
                    }
                    return mesh;

                case ObjectType.Curve:

                    geometry = (BufferGeometry)obj["geometry"];
                    _color = GetColor(doc,attributes);
                    color = new THREE.Color(_color.R / 255.0f, _color.G / 255.0f, _color.B / 255.0f);

                    material = new LineBasicMaterial { Color = color };
                    material = CompareMaterials(material);

                    var lines = new THREE.Line(geometry, material);
                    lines.UserData["attributes"] = attributes;
                    lines.UserData["objectType"] = obj["objectType"];
                    if (attributes.Name != null)
                    {
                        lines.Name = attributes.Name;
                    }

                    return lines;

                //case 'TextDot':

                //    geometry = obj.geometry;

                //    const ctx = document.createElement('canvas').getContext('2d');
                //    const font = `${ geometry.fontHeight}
                //    px ${ geometry.fontFace}`;
                //    ctx.font = font;
                //    const width = ctx.measureText(geometry.text).width + 10;
                //    const height = geometry.fontHeight + 10;

                //    const r = window.devicePixelRatio;

                //    ctx.canvas.width = width * r;
                //    ctx.canvas.height = height * r;
                //    ctx.canvas.style.width = width + 'px';
                //    ctx.canvas.style.height = height + 'px';
                //    ctx.setTransform(r, 0, 0, r, 0, 0);

                //    ctx.font = font;
                //    ctx.textBaseline = 'middle';
                //    ctx.textAlign = 'center';
                //    color = attributes.drawColor;
                //    ctx.fillStyle = `rgba(${ color.r},${ color.g},${ color.b},${ color.a})`;
                //    ctx.fillRect(0, 0, width, height);
                //    ctx.fillStyle = 'white';
                //    ctx.fillText(geometry.text, width / 2, height / 2);

                //    const texture = new CanvasTexture(ctx.canvas);
                //    texture.minFilter = LinearFilter;
                //    texture.wrapS = ClampToEdgeWrapping;
                //    texture.wrapT = ClampToEdgeWrapping;

                //    material = new SpriteMaterial( { map: texture, depthTest: false } );
                //    const sprite = new Sprite(material);
                //    sprite.position.set(geometry.point[0], geometry.point[1], geometry.point[2]);
                //    sprite.scale.set(width / 10, height / 10, 1.0);

                //    sprite.userData['attributes'] = attributes;
                //    sprite.userData['objectType'] = obj.objectType;

                //    if (attributes.name)
                //    {

                //        sprite.name = attributes.name;

                //    }

                //    return sprite;

                case ObjectType.Light:
                    var rhinoGeometry = (RhinoGeometry)obj["geometry"];
                    var lightGeometry = rhinoGeometry.Geometry as Rhino.Geometry.Light;

                    THREE.Light light = null;

                    switch (lightGeometry.LightStyle)
                    {

                        case LightStyle.WorldPoint:

                            light = new PointLight();
                            light.CastShadow = attributes.CastsShadows;
                            light.Position.Set((float)lightGeometry.Location[0], (float)lightGeometry.Location[1], (float)lightGeometry.Location[2]);
                            light.Shadow.NormalBias = 0.1f;
                            break;

                        case LightStyle.WorldSpot:

                            light = new SpotLight();
                            light.CastShadow = attributes.CastsShadows;
                            light.Position.Set((float)lightGeometry.Location[0], (float)lightGeometry.Location[1], (float)lightGeometry.Location[2]);
                            light.Target.Position.Set((float)lightGeometry.Direction[0], (float)lightGeometry.Direction[1], (float)lightGeometry.Direction[2]);
                            light.Angle = (float)lightGeometry.SpotAngleRadians;
                            light.Shadow.NormalBias = 0.1f;

                            break;

                        case LightStyle.WorldRectangular:

                            light = new RectAreaLight();
                            var width = Math.Abs(lightGeometry.Width[2]);
                            var height = Math.Abs(lightGeometry.Length[0]);
                            light.Position.Set((float)(lightGeometry.Location[0] - (height / 2)), (float)lightGeometry.Location[1], (float)(lightGeometry.Location[2] - (width / 2)));
                            light.Height = (int)height;
                            light.Width = (int)width;
                            light.LookAt(new Vector3((float)lightGeometry.Direction[0], (float)lightGeometry.Direction[1], (float)lightGeometry.Direction[2]));

                            break;

                        case LightStyle.WorldDirectional:

                            light = new DirectionalLight();
                            light.CastShadow = attributes.CastsShadows;
                            light.Position.Set((float)lightGeometry.Location[0], (float)lightGeometry.Location[1], (float)lightGeometry.Location[2]);
                            light.Target.Position.Set((float)lightGeometry.Direction[0], (float)lightGeometry.Direction[1], (float)lightGeometry.Direction[2]);
                            light.Shadow.NormalBias = 0.1f;

                            break;

                        case LightStyle.WorldLinear:
                            // not conversion exists, warning has already been printed to the console
                            break;

                        default:
                            break;
                    }

                    if (light != null)
                    {

                        light.Intensity = (float)lightGeometry.Intensity;
                        _color = lightGeometry.Diffuse;
                        color = new THREE.Color(_color.R / 255.0f, _color.G / 255.0f, _color.B / 255.0f);
                        light.Color = color;
                        light.UserData["Attributes"] = attributes;
                        light.UserData["ObjectType"] = obj["objectType"];
                    }
                    return light;
            }
            return null;
        }
        private string GetMaterialKey(THREE.Material material)
        {
            THREE.Color color = material.Color == null ? material.Color.Value : THREE.Color.ColorName(ColorKeywords.black);
            string key = $"Name:{material.Name};Color:{color.R},{color.G},{color.B};Type:{material.type}; ";
            return key;
        }
        private THREE.Material CompareMaterials(THREE.Material material)
        {
            var materialKey = GetMaterialKey(material);
            if (Materials.ContainsKey(materialKey))
            {
                return Materials[materialKey];
            }
            Materials[materialKey] = material;
            return material;
        }
        private THREE.Material CreateMaterial(RhinoMaterial material = null)
        {
            if (material == null)
            {

                return new MeshStandardMaterial
                {
                    Color = new THREE.Color(1, 1, 1),
                    Metalness = 0.8f,
                    Name = "default",
                    Side = Constants.DoubleSide
                };
            }

            var _diffuseColor = (System.Drawing.Color)material.Material.DiffuseColor;

            var diffusecolor = new THREE.Color(_diffuseColor.R / 255.0f, _diffuseColor.G / 255.0f, _diffuseColor.B / 255.0f);

            if (_diffuseColor.R == 0 && _diffuseColor.G == 0 && _diffuseColor.B == 0)
            {
                diffusecolor.R = 1;
                diffusecolor.G = 1;
                diffusecolor.B = 1;
            }

            var mat = new MeshStandardMaterial
            {
                Color = diffusecolor,
                Name = material.Material.Name,
                Side = 2,
                Transparent = (float)material.Material.Transparency > 0 ? true : false,
                Opacity = 1.0f - (float)material.Material.Transparency
            };

            var textures = (List<RhinoTexture>)material.Textures;
            for (var i = 0; i < textures.Count; i++)
            {

                var texture = textures[i];

                if (texture.FileName != null)
                {

                    var map = TextureLoader.Load(texture.FileName);

                    switch (texture.TextureType)
                    {

                        case TextureType.Diffuse:
                            mat.Map = map;
                            break;

                        case TextureType.Bump:
                            mat.BumpMap = map;
                            break;

                        case TextureType.Transparency:
                            mat.AlphaMap = map;
                            mat.Transparent = true;
                            break;

                        case TextureType.Emap:
                            mat.EnvMap = map;
                            break;
                    }
                }
            }

            return mat;
        }
    }
}

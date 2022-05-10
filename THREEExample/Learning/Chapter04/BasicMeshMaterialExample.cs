using ImGuiNET;
using OpenTK;
using THREE.Core;
using THREE.Geometries;
using THREE.Lights;
using THREE.Loaders;
using THREE.Materials;
using THREE.Math;
using THREE.Objects;

namespace THREEExample.Learning.Chapter04
{
    [Example("01.BasicMeshMaterial", ExampleCategory.LearnThreeJS, "Chapter04")]
    public class BasicMeshMaterialExample : MaterialExampleTemplate
    {
        Mesh plane, cube, sphere;
        Object3D selectedMesh;
        Group gopher;

        AmbientLight ambientLight;
        SpotLight spotLight;

        int selectedIndex = 0;

        float step = 0;

        public Material meshMaterial;

        public BasicMeshMaterialExample() : base()
        {

        }
       
        public virtual void BuildMeshMaterial()
        {
            meshMaterial = new MeshBasicMaterial()
            {
                Color = Color.Hex(0x7777ff),
                Name = "Basic Material",
                FlatShading = true,
                Opacity = 0.01f,
                ColorWrite = true,
                Fog = true
            };
        }
        public virtual void BuildGeometry()
        {
            var groundGeometry = new PlaneGeometry(100, 100, 4, 4);
            var groundMesh = new Mesh(groundGeometry, new MeshBasicMaterial() { Color = Color.Hex(0x777777) });
            groundMesh.Rotation.X = (float)(-System.Math.PI / 2);
            groundMesh.Position.Y = -20;
            scene.Add(groundMesh);

            var sphereGeometry = new SphereGeometry(14, 20, 20);
            var cubeGeometry = new BoxGeometry(15, 15, 15);
            var planeGeometry = new PlaneGeometry(14, 14, 4, 4);

            BuildMeshMaterial();

            sphere = new Mesh(sphereGeometry, meshMaterial);
            cube = new Mesh(cubeGeometry, meshMaterial);
            plane = new Mesh(planeGeometry, meshMaterial);

            sphere.Position.Set(0, 3, 2);
            cube.Position.Copy(sphere.Position);
            plane.Position.Copy(sphere.Position);

            materialsLib.Add(meshMaterial.Name, meshMaterial);

            OBJLoader loader = new OBJLoader();
            gopher = loader.Load(@"../../../assets/models/gopher/gopher.obj");

            ComputeNormalsGroup(gopher);

            SetMaterialGroup(meshMaterial, gopher);

            gopher.Scale.Set(4, 4, 4);
            scene.Add(cube);

            selectedMesh = cube;

            ambientLight = new AmbientLight(Color.Hex(0x0c0c0c));
            scene.Add(ambientLight);

            spotLight = new SpotLight(Color.Hex(0xffffff));
            spotLight.Position.Set(-40, 60, -10);
            spotLight.CastShadow = true;
            scene.Add(spotLight);

        }
        public override void Load(GLControl control)
        {
            base.Load(control);

            BuildGeometry();

        }

        public override void Render()
        {
            base.Render();
            step += 0.001f;
            selectedMesh.Rotation.Y = step;
        }
        public void ComputeNormalsGroup(Group group)
        {
            group.Traverse(o =>
            {
                if (o is Mesh)
                {
                    var tempGeom = new Geometry();
                    tempGeom.FromBufferGeometry((BufferGeometry)o.Geometry);
                    tempGeom.ComputeFaceNormals();
                    tempGeom.MergeVertices();
                    tempGeom.ComputeFlatVertexNormals();

                    tempGeom.NormalsNeedUpdate = true;

                    o.Geometry = tempGeom;
                }
            });
        }
        public void SetMaterialGroup(Material material, Group group)
        {
            group.Traverse(o =>
            {
                o.Material = material;
                if (o is Mesh && o.Materials.Count > 1)
                {
                    for (var i = 0; i < o.Materials.Count; i++)
                        o.Materials[i] = material;
                }
            });
        }

        public override void AddSpecificMaterialSettings(Material material, string name)
        {
            base.AddSpecificMaterialSettings(material, name);

            if (ImGui.Combo("SelectedObject", ref selectedIndex, "Cube\0Sphere\0Plane\0Gopher\0"))
            {
                scene.Remove(plane);
                scene.Remove(cube);
                scene.Remove(sphere);
                scene.Remove(gopher);

                switch (selectedIndex)
                {
                    case 0:
                        scene.Add(cube);
                        selectedMesh = cube;
                        break;
                    case 1:
                        scene.Add(sphere);
                        selectedMesh = sphere;
                        break;
                    case 2:
                        scene.Add(plane);
                        selectedMesh = plane;
                        break;
                    case 3:
                        scene.Add(gopher);
                        selectedMesh = gopher;
                        break;
                }
            }
        }
    }
}

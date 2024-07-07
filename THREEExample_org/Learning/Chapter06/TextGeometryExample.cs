using ImGuiNET;
using OpenTK;
using System.Collections;
using THREE;
using THREEExample.Learning.Chapter04;
using THREEExample.Learning.Utils;
using Matrix4 = THREE.Matrix4;

namespace THREEExample.Learning.Chapter06
{
    [Example("07-Text-Geometry", ExampleCategory.LearnThreeJS, "Chapter06")]
    public class TextGeometryExample : Chapter04.MaterialExampleTemplate
    {
        public Mesh appliedMesh;

        public Mesh groundPlane;

        Hashtable options;
        int size =90;
        int height = 90;
        Font font;
        int bevelThickness = 2;
        float bevelSize=0.5f;
        int bevelSegments = 3;
        int curveSegments = 12;
        bool bevelEnabled = true;
        int steps = 1;

        int fontIndex = 0;

        float step = 0;
        public TextGeometryExample() : base()
        {

        }

       
        public override void Load(GLControl control)
        {
            base.Load(control);

            buildMesh();
        }
        private TextGeometry createTextGeometry(Hashtable parameter)
        {
            return new TextGeometry("Learning Three.js", parameter);
        }
        private void buildMesh()
        {
            groundPlane = DemoUtils.AddLargeGroundPlane(this.scene);

            groundPlane.Position.Y = -30;

            DemoUtils.InitDefaultLighting(this.scene);



            font = FontLoader.Load(@"../../../../assets/fonts/bitstream_vera_sans_mono_roman.typeface.json");

            options = new Hashtable()
            {
                {"size",size },
                {"height",height },
                {"font",font },
                {"bevelThickness", bevelThickness },
                {"bevelSize", bevelSize },
                {"bevelSegments", bevelSegments },
                {"bevelEnabled", bevelEnabled },
                {"curveSegments", curveSegments },
                {"steps", steps }
            };


            var geom = createTextGeometry(options);
            geom.ApplyMatrix4(new Matrix4().MakeScale(0.05f, 0.05f, 0.05f));
            geom.Center();

            //var geometry = GeneratePoints(NewPoints(5),64,1,8,false);
            Material material = null;
            appliedMesh = DemoUtils.AppliyMeshNormalMaterial(geom,ref material);
            material.Name = "MeshNormalMaterial";
            materialsLib.Add(material.Name, material);

            appliedMesh.CastShadow = true;

            scene.Add(appliedMesh);
        }
        public override void Render()
        {
            appliedMesh.Rotation.Y = step += 0.005f;
            appliedMesh.Rotation.X = step;
            appliedMesh.Rotation.Z = step;
            base.Render();
        }
        public override void ShowGUIControls()
        {
            ImGui.NewFrame();
            ImGui.Begin("Controls");

            bool redraw = false;

            if (ImGui.SliderInt("size", ref size,0,200))
                redraw = true;
            if (ImGui.SliderInt("height", ref height, 0, 200))
                redraw = true;
            if (ImGui.Combo("fontName", ref fontIndex, "bitstream vera sans mono\0helvetiker\0helvetiker bold\0"))
            {
                redraw = true;
                switch(fontIndex)
                {
                    case 0: font = FontLoader.Load(@"../../../../assets/fonts/bitstream_vera_sans_mono_roman.typeface.json");break;
                    case 1: font = FontLoader.Load(@"../../../../assets/fonts/helvetiker_regular.typeface.json"); break;
                    case 2: font = FontLoader.Load(@"../../../../assets/fonts/helvetiker_bold.typeface.json"); break;

                }
            }
            if (ImGui.SliderInt("bevelThickness", ref bevelThickness, 0, 10))
                redraw = true;

            if (ImGui.SliderFloat("bevelSize", ref bevelSize, 0, 100))
                redraw = true;
            if (ImGui.SliderInt("bevelSegments", ref bevelSegments, 0, 30))
                redraw = true;
            if (ImGui.Checkbox("bevelEnabled", ref bevelEnabled))
                redraw = true;
            if (ImGui.SliderInt("curveSegments", ref curveSegments, 1, 30))
                redraw = true;
            if (ImGui.SliderInt("steps", ref steps, 1, 5))
                redraw = true;

            if(redraw)
            {
                options["size"] = size;
                options["font"] = font;
                options["height"] = height;
                options["bevelThickness"] = bevelThickness;
                options["bevelSize"] = bevelSize;
                options["bevelSegments"] = bevelSegments;
                options["bevelEnabled"] = bevelEnabled;
                options["steps"] = steps;

                appliedMesh.Geometry = createTextGeometry(options);
                appliedMesh.Geometry.ApplyMatrix4(new Matrix4().MakeScale(0.05f, 0.05f, 0.05f));
                appliedMesh.Geometry.Center();
            }

            foreach (var item in materialsLib)
            {
                AddBasicMaterialSettings(item.Value, item.Key + "-THREE.Material");
                AddSpecificMaterialSettings(item.Value, item.Key + "-THREE.MeshStandardMaterial");
            }

            ImGui.End();
            ImGui.Render();
            imGuiManager.ImGui_ImplOpenGL3_RenderDrawData(ImGui.GetDrawData());

        }
        public override void AddWireframeProperty(Material material)
        {
            ImGui.Checkbox("wireframe", ref material.Wireframe);
        }
        public override void AddSpecificMaterialSettings(Material material, string name)
        {
            if (ImGui.TreeNode(name))
            {
                AddWireframeProperty(material);
                ImGui.TreePop();
            }
           
        }
    }
}

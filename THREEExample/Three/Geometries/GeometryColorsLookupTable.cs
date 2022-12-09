using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using THREE;
using THREE.Cameras;
using THREE.Core;
using THREE.Lights;
using THREE.Loaders;
using THREE.Materials;
using THREE.Math;
using THREE.Objects;
using THREE.Scenes;
namespace THREEExample.Three.Geometries
{
    [Example("geometry colors lookuptable", ExampleCategory.ThreeJs, "geometry")]
    public class GeometryColorsLookupTable : THREEExampleTemplate
    {
        Camera orthoCamera;
        Sprite sprite;


        Mesh mesh;
        Lut lut;

        Scene uiScene;
        int colorIndex = 0;
        
        public GeometryColorsLookupTable() : base()
        {
            uiScene = new Scene();
            lut = new Lut();
        }
        public override void InitCamera()
        {
            base.InitCamera();
            camera.Fov = 60.0f;
            camera.Near = 0.1f;
            camera.Far = 1000;
            camera.Position.Set(8, -2, 0);
            camera.LookAt(Vector3.Zero());
            scene.Add(camera);


            orthoCamera = new OrthographicCamera(-1, 1, 1, -1, 1, 2);;
            orthoCamera.Position.Set(0.5f, 0, 1);
            (orthoCamera as OrthographicCamera).View.Enabled = false;
            orthoCamera.UpdateProjectionMatrix();
        }
        public override void InitLighting()
        {
            var pointLight = new PointLight(0xffffff, 1);
            camera.Add(pointLight);
        }
        public override void Init()
        {
            base.Init();

            scene.Background = Color.Hex(0xffffff);
                        
            renderer.AutoClear = false;

            CreateObject();

            AddGuiControlsAction = ShowControls;
        }
        
        public void CreateObject()
        {
            sprite = new Sprite(new SpriteMaterial() { Opacity=1.0f,Transparent=true,Color=Color.Hex(0xffffff),Map = lut.CreateTexture() } );
            sprite.Scale.X = 0.125f;
            uiScene.Add(sprite);

            mesh = new Mesh(null, new MeshLambertMaterial() {
                Side = Constants.DoubleSide,
                Color = Color.Hex(0xF5F5F5),
                VertexColors = true
            });
            scene.Add(mesh);
				
            LoadModel();

            
        }
        private void LoadModel()
        {
            var loader = new BufferGeometryLoader();

            var geometry = loader.Load(@"../../../assets/models/json/pressure.json");
            geometry.Center();
            geometry.ComputeFlatVertexNormals();

            List<float> colors = new List<float>();
            BufferAttribute<float> positions = (BufferAttribute<float>)geometry.Attributes["position"];

            for(var i = 0; i < positions.count; i++)
            {
                colors.Add(1);colors.Add(1);colors.Add(1);
            }

            geometry.SetAttribute("color", new BufferAttribute<float>(colors.ToArray(), 3));

            mesh.Geometry = geometry;

            UpdateColors();
        }

        private void UpdateColors()
        {
            string colorMap = "rainbow";
            switch(colorIndex)
            {
                case 0: colorMap = "rainbow";break;
                case 1: colorMap = "cooltowarm"; break;
                case 2: colorMap = "blackbody"; break;
                case 3: colorMap = "grayscale"; break;
            }
            lut.SetColorMap(colorMap,null);

            lut.SetMax(2000);
            lut.SetMin(0);

            var geometry = mesh.Geometry as BufferGeometry;
            var pressures = geometry.Attributes["pressure"] as BufferAttribute<float>;
            var colors = geometry.Attributes["color"] as BufferAttribute<float>;
            for (var i = 0; i < pressures.Array.Length; i++)
            {

                var colorValue = pressures.Array[i];

                var color = lut.GetColor(colorValue);

                colors.setXYZ(i, color.R, color.G, color.B);

            }

            colors.NeedsUpdate = true;

            //var map = sprite.material.map;
            lut.UpdateTexture(sprite.Material.Map);
            sprite.Material.Map.NeedsUpdate = true;
        }

        public override void Render()
        {
            if (!imGuiManager.ImWantMouse)
                controls.Enabled = true;
            else
                controls.Enabled = false;

            controls.Update();
            renderer.Clear();
            renderer.Render(scene, camera);
            renderer.Render(uiScene, orthoCamera);
            ShowGUIControls();
        }

        private void ShowControls()
        {
            if(ImGui.Combo("colorMap",ref colorIndex, "rainbow\0cooltowarm\0blackbody\0grayscale\0"))
            {
                UpdateColors();
            }
            bool cameraUpdate = false;
            bool persCameraUpdate = false;
            if (ImGui.TreeNode("OrthoCamera"))
            {
                if (ImGui.SliderFloat("left", ref orthoCamera.Left, -20.0f, -1.0f)) cameraUpdate = true;
                if (ImGui.SliderFloat("right", ref orthoCamera.CameraRight, 0.0f, 1.0f)) cameraUpdate = true;
                if (ImGui.SliderFloat("top", ref orthoCamera.Top, 0.0f, 1.0f)) cameraUpdate = true;
                if (ImGui.SliderFloat("bottom", ref orthoCamera.Bottom, -1.0f, 0.0f)) cameraUpdate = true;
                if (ImGui.SliderFloat("near", ref orthoCamera.Near, -10.0f, 10.0f)) cameraUpdate = true;
                if (ImGui.SliderFloat("far", ref orthoCamera.Far, 10.0f, 20.0f)) cameraUpdate = true;

                if (cameraUpdate)
                    (orthoCamera as OrthographicCamera).UpdateProjectionMatrix();
                ImGui.TreePop();
            }
            if (ImGui.TreeNode("PersCamera"))
            {
                if (ImGui.SliderFloat("fov", ref (camera as PerspectiveCamera).Fov, 1.0f, 100.0f)) cameraUpdate = true;
                if (ImGui.SliderFloat("near", ref (camera as PerspectiveCamera).Near, -10.0f, 10.0f)) cameraUpdate = true;
                if (ImGui.SliderFloat("far", ref (camera as PerspectiveCamera).Far, 0.0f, 1000.0f)) cameraUpdate = true;
                if(ImGui.TreeNode("position"))
                {
                    
                    if (ImGui.SliderFloat("X", ref camera.Position.X, -1000.0f, 1000.0f)) cameraUpdate = true;
                    if (ImGui.SliderFloat("Y", ref camera.Position.Y, -1000.0f, 1000.0f)) cameraUpdate = true;
                    if (ImGui.SliderFloat("Z", ref camera.Position.Z, -1000.0f, 1000.0f)) cameraUpdate = true;
                    ImGui.TreePop();
                }               


                if (cameraUpdate)
                    (orthoCamera as OrthographicCamera).UpdateProjectionMatrix();

                if (persCameraUpdate)
                    (camera as PerspectiveCamera).UpdateProjectionMatrix();
                ImGui.TreePop();
            }
            if (ImGui.TreeNode("scale"))
            {
                ImGui.SliderFloat("scaleX", ref sprite.Scale.X, 0.01f, 10);
                ImGui.SliderFloat("scaleY", ref sprite.Scale.X, 1.0f, 10);
                ImGui.SliderFloat("scaleZ", ref sprite.Scale.X, 1.0f, 10);

                ImGui.TreePop();
            }
            if (ImGui.TreeNode("spriteMaterial"))
            {
                ImGui.SliderFloat("opacity", ref sprite.Material.Opacity, 0.0f, 1.0f);
                ImGui.Checkbox("transparent", ref sprite.Material.Transparent);
                ImGui.Checkbox("fog", ref sprite.Material.Fog);
                ImGui.TreePop();
            }
        }
        public override void Resize(System.Drawing.Size clientSize)
        {
            renderer.Resize(clientSize.Width, clientSize.Height);
            camera.Aspect = this.glControl.AspectRatio;
            (camera as PerspectiveCamera).UpdateProjectionMatrix();
        }
    }
}

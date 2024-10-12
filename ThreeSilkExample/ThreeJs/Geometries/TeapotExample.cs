using ImGuiNET;
using System.Collections.Generic;
using THREE;
using Color = THREE.Color;
namespace THREE.Silk.Example
{
    [Example("teapot",ExampleCategory.ThreeJs,"Geometry")]
    public class TeapotExample : Example
    {
        Mesh teapot;

        Color materialColor;
        AmbientLight ambientLight;
        THREE.Light light;
        CubeTexture textureCube;
        MeshBasicMaterial wireMaterial;
        MeshPhongMaterial flatMaterial;
        MeshLambertMaterial gouraudMaterial;
        MeshPhongMaterial phongMaterial;
        MeshPhongMaterial texturedMaterial;
        MeshPhongMaterial reflectiveMaterial;

        int teapotSize = 400;
        int newTess = 7;
        bool bottom = true;
        bool lid = true;
        bool body = true;
        bool fitLid = false;
        bool nonblinn = false;
        int newShading = 3;
        float shininess = 40.0f;

        float ka = 0.17f;
        float kd = 0.51f;
        float ks = 0.2f;
        float hue = 0.121f;

        float lhue = 0.04f;
        float lsaturation = 0.01f;
        float llightness = 1.0f;

        float lx = 0.32f;
        float ly = 0.39f;
        float lz = 0.7f;

        float saturation = 0.73f;
        float lightness = 0.66f;
        bool metallic = true;
        Color diffuseColor;
        Color specularColor;

        int[] tessArray = new int[] { 2, 3, 4, 5, 6, 8, 10, 15, 20, 30, 40, 50 };
        Material[] materialArray;

        public TeapotExample() : base()
        {
            scene.Background = Color.Hex(0xAAAAAA);
        }
        public override void InitCamera()
        {
            camera = new PerspectiveCamera(45, this.AspectRatio, 1, 80000);
            camera.Position.Set(-600, 550, 1300);
        }
        public override void InitLighting()
        {
            // LIGHTS
            ambientLight = new AmbientLight(0x333333);    // 0.2

            light = new DirectionalLight(0xFFFFFF, 1.0f);

            scene.Add(ambientLight);
            scene.Add(light);
        }
        public override void InitRenderer()
        {
            base.InitRenderer();
            renderer.SetClearColor(Color.Hex(0x000000));
        }
        public override void Init()
        {
            base.Init();

            InitGeometry();

            AddGuiControlsAction = SetupGui;
        }
        private void InitGeometry()
        {
            InitMaterial();
          

            CreateNewTeapot();

            diffuseColor.SetHSL(hue, saturation, lightness);
        }
        private void InitMaterial()
        {

            var textureMap = TextureLoader.Load("../../../../assets/textures/uv_grid_opengl.jpg");
            textureMap.WrapS = textureMap.WrapT = Constants.RepeatWrapping;
            textureMap.Anisotropy = 16;
            textureMap.Encoding = Constants.sRGBEncoding;

            string path = "../../../../assets/textures/cube/pisa/";
            List<string> urls = new List<string>()
            {
                    path + "px.png", path + "nx.png",
                    path + "py.png", path + "ny.png",
                    path + "pz.png", path + "nz.png"
            };

            textureCube = CubeTextureLoader.Load(urls);
            textureCube.Encoding = Constants.sRGBEncoding;


            materialColor.SetRGB(1.0f, 1.0f, 1.0f);

            wireMaterial = new MeshBasicMaterial() { Color = Color.Hex(0xFFFFFF), Wireframe = true };

            flatMaterial = new MeshPhongMaterial() { Color= materialColor, Specular= Color.Hex(0x000000), FlatShading= true, Side= Constants.DoubleSide } ;

            gouraudMaterial = new MeshLambertMaterial() { Color= materialColor, Side= Constants.DoubleSide } ;

            phongMaterial = new MeshPhongMaterial() { Color= materialColor, Side= Constants.DoubleSide } ;

            texturedMaterial = new MeshPhongMaterial() { Color= materialColor, Map= textureMap, Side= Constants.DoubleSide } ;

            reflectiveMaterial = new MeshPhongMaterial() { Color= materialColor, EnvMap= textureCube, Side= Constants.DoubleSide } ;

            materialArray = new Material[6] { wireMaterial,flatMaterial,gouraudMaterial,phongMaterial,texturedMaterial,reflectiveMaterial};

        }
        private void CreateNewTeapot()
        {
            if (teapot != null)
            {

                teapot.Geometry.Dispose();
                scene.Remove(teapot);

            }

            var teapotGeometry = new TeapotBufferGeometry(teapotSize,tessArray[newTess],
                bottom,
                lid,
                body,
                fitLid,
                !nonblinn);

            teapot = new Mesh(teapotGeometry, materialArray[newShading]);

            if (newShading == 5)
            {

                scene.Background = textureCube;

            }
            else
            {

                scene.Background = null;

            }
            scene.Add(teapot);
        }
      
     
        private void SetupGui()
        {
            if(ImGui.TreeNode("Material control"))
            {
                ImGui.SliderFloat("shininess", ref shininess, 1.0f, 400.0f);
               
                ImGui.SliderFloat("diffuse strength", ref kd, 0.0f, 1.0f);
                
                ImGui.SliderFloat("specular strength", ref ks, 0.0f, 1.0f);                
                
                ImGui.Checkbox("metallic", ref metallic);
                
                ImGui.TreePop();
            }
            if(ImGui.TreeNode("Material color"))
            {
                ImGui.SliderFloat("hue", ref hue, 0.0f, 1.0f);

                ImGui.SliderFloat("saturation", ref saturation, 0.0f, 1.0f);

                ImGui.SliderFloat("lightness", ref lightness, 0.0f, 1.0f);
                
                ImGui.TreePop();
            }
            if(ImGui.TreeNode("Lighting"))
            {
                ImGui.SliderFloat("lhue", ref lhue, 0.0f, 1.0f);

                ImGui.SliderFloat("lsaturation", ref lsaturation, 0.0f, 1.0f);

                ImGui.SliderFloat("llightness", ref llightness, 0.0f, 1.0f);

                ImGui.SliderFloat("ambient", ref ka, 0.0f, 1.0f);

                ImGui.TreePop();
            }

            if(ImGui.TreeNode("Light direction"))
            {
                ImGui.SliderFloat("lx", ref lx, -1.0f, 1.0f);

                ImGui.SliderFloat("ly", ref ly, -1.0f, 1.0f);

                ImGui.SliderFloat("lz", ref lz, -1.0f, 1.0f);

                ImGui.TreePop();
            }

            bool recreateTeapot = false;
            if (ImGui.TreeNode("Tessellation control"))
            {
               
                if(ImGui.Combo("newTess", ref newTess, "2\03\04\05\06\08\010\015\020\030\040\050\0")) recreateTeapot=true;
                if(ImGui.Checkbox("display lid", ref lid))  recreateTeapot = true; 
                if(ImGui.Checkbox("display body", ref body)) recreateTeapot = true;
                if(ImGui.Checkbox("display bottom", ref bottom))  recreateTeapot = true; 
                if(ImGui.Checkbox("snug lid", ref fitLid)) recreateTeapot = true; 
                if(ImGui.Checkbox("original scale", ref nonblinn)) recreateTeapot = true; 
                if(ImGui.Combo("newShading", ref newShading, "wireframe\0flat\0smooth\0glossy\0textured\0reflective\0")) recreateTeapot = true; 
                ImGui.TreePop();
            }
            if(recreateTeapot)
            {
                CreateNewTeapot();
            }
        }
        public override void Render()
        {
           

            // skybox is rendered separately, so that it is always behind the teapot.

            if (!this.ImWantMouse)
                controls.Enabled = true;
            else
                controls.Enabled = false;

            controls.Update();
            
            phongMaterial.Shininess = shininess;
            texturedMaterial.Shininess = shininess;

            diffuseColor.SetHSL(hue, saturation, lightness);
            if (metallic)
            {

                // make colors match to give a more metallic look
                specularColor.Copy(diffuseColor);

            }
            else
            {

                // more of a plastic look
                specularColor.SetRGB(1, 1, 1);

            }

            diffuseColor.MultiplyScalar(kd);
            flatMaterial.Color = new Color().Copy(diffuseColor);
            gouraudMaterial.Color = new Color().Copy(diffuseColor);
            phongMaterial.Color = new Color().Copy(diffuseColor);
            texturedMaterial.Color = new Color().Copy(diffuseColor);

            specularColor.MultiplyScalar(ks);
            phongMaterial.Specular.Copy(specularColor);
            texturedMaterial.Specular.Copy(specularColor);

            // Ambient's actually controlled by the light for this demo
            ambientLight.Color.SetHSL(hue, saturation, lightness * ka);

            light.Position.Set(lx, ly, lz);
            light.Color.SetHSL(lhue, lsaturation, llightness);

            base.Render();
        }
    }
}

using ImGuiNET;
using OpenTK;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using THREE;
using THREE.Cameras;
using THREE.Cameras.Controlls;
using THREE.Core;
using THREE.Lights;
using THREE.Materials;
using THREE.Math;
using THREE.Objects;
using THREE.Postprocessing;
using THREE.Renderers.gl;
using THREE.Scenes;
using THREE.Textures;
using THREEExample.ThreeImGui;
namespace THREEExample.Learning.Chapter11
{
    public class EffectComposerTemplate : Example
    {
        public Scene scene;

        public Camera camera;

        public TrackballControls controls;

        public ImGuiManager imGuiManager;

        public Mesh earth;

        public Scene pivot;

        public RenderPass renderPass;
        public BokehPass bokehPass;
        public FilmPass effectFilm;


        public GlitchPass glitchPass;
        public HalftonePass halftonePass;
        public OutlinePass outlinePass;
        public UnrealBloomPass unrealBloomPass;

        public EffectComposer composer;


        public BloomPass bloomPass;
        public DotScreenPass dotScreenPass;

        public TexturePass renderedScene;

        public EffectComposer effectFilmComposer;
        public EffectComposer bloomComposer;
        public EffectComposer dotScreenComposer;

        public int halfWidth, halfHeight;

        public EffectComposer composer1;
        public EffectComposer composer2;
        public EffectComposer composer3;
        public EffectComposer composer4;

        //FilmPass parameter
        bool grayScale = false;
        float noiseIntensity = 0.8f;
        float scanlinesIntensity = 0.325f;
        float scanlinesCount = 256.0f;

        //dotScreenPass Parameter
        float centerX = 0.5f;
        float centerY = 0.5f;
        float angle = 1.0f;
        float scale = 1.0f;

        //BloomPass parameter
        float strength = 3.0f;
        int kernelSize = 25;
        float sigma = 5.0f;
        int resolution = 256;

        //GiltchPass parameter
        int dtSize = 64;

        //HalftonePass paramter;
        int shape = 0;
        float radius = 4.0f;
        float rotateR = (float)System.Math.PI / 12 * 1;
        float rotateG = (float)System.Math.PI / 12 * 2;
        float rotateB = (float)System.Math.PI / 12 * 2;
        float scatter = 0.0f;
        float width = 1.0f;
        float height = 1.0f;
        float blending = 1.0f;
        int blendingMode = 1;
        bool greyScale = false;

        //OutlinePass parameter
        float edgeStrength = 3.0f;
        float edgeGlow = 0.0f;
        float edgeThickness = 1.0f;
        float pulsePeriod = 0.0f;
        bool usePatternTexture = false;
        Color visibleEdgeColor = new Color(0xffffff);
        Color hiddenEdgeColor = new Color(0x190A05);

        public Dictionary<string, Material> materialsLib = new Dictionary<string, Material>();

        public EffectComposerTemplate() : base()
        {
            camera = new THREE.Cameras.PerspectiveCamera();
            scene = new Scene();
            stopWatch.Start();
            
        }
        public void InitRenderer()
        {
            this.renderer.SetClearColor(new Color().SetHex(0x000000));
            this.renderer.ShadowMap.Enabled = true;
            this.renderer.ShadowMap.type = Constants.PCFSoftShadowMap;
        }
        public void InitCamera()
        {
            camera.Fov = 45.0f;
            camera.Aspect = this.glControl.AspectRatio;
            camera.Near = 0.1f;
            camera.Far = 1000.0f;
            camera.Position.Set(0, 20, 40);
            camera.LookAt(THREE.Math.Vector3.Zero());
        }
        public void InitCameraController()
        {
            controls = new TrackballControls(this.glControl, camera);
            controls.StaticMoving = false;
            controls.RotateSpeed = 3.0f;
            controls.ZoomSpeed = 2;
            controls.PanSpeed = 2;
            controls.NoZoom = false;
            controls.NoPan = false;
            controls.NoRotate = false;
            controls.StaticMoving = true;
            controls.DynamicDampingFactor = 0.2f;
        }
        public override void Load(GLControl control)
        {
            base.Load(control);

            InitRenderer();

            this.renderer.SetSize(control.Width, control.Height);

            InitCamera();

            InitCameraController();

            imGuiManager = new ImGuiManager(this.glControl);

            (earth, pivot) = Util11.AddEarth(scene);

            halfWidth = glControl.Width / 2;
            halfHeight = glControl.Height / 2;
        }

        public override void Render()
        {
            controls.Update();
            earth.Rotation.Y += 0.001f;
            pivot.Rotation.Y += -0.0003f;
        }
        public virtual void ShowControls()
        {
            ImGui.NewFrame();
            ImGui.Begin("Controls");
            AddFilmPassControl("FilmPass",effectFilm);
            ImGui.End();
            ImGui.Render();
            imGuiManager.ImGui_ImplOpenGL3_RenderDrawData(ImGui.GetDrawData());
        }
        public virtual void AddFilmPassControl(string rootName,FilmPass film)
        {
            if (ImGui.TreeNode(rootName))
            {
                if (ImGui.Checkbox("grayScale", ref grayScale))
                {
                    (film.uniforms["grayscale"] as GLUniform)["value"] = grayScale;
                }
                if (ImGui.SliderFloat("noiseIntensity", ref noiseIntensity, 0.0f, 1.0f))
                {
                    (film.uniforms["nIntensity"] as GLUniform)["value"] = noiseIntensity;
                }
                if (ImGui.SliderFloat("scanlinesIntensity", ref scanlinesIntensity, 0.0f, 1.0f))
                {
                    (film.uniforms["sIntensity"] as GLUniform)["value"] = scanlinesIntensity;
                }
                if (ImGui.SliderFloat("scanlinesCount", ref scanlinesCount, 0.0f, 500.0f))
                {
                    (film.uniforms["sCount"] as GLUniform)["value"] = scanlinesCount;
                }
                ImGui.TreePop();
            }
        }
        public virtual void AddDotScreenPassControl(string rootName,DotScreenPass dot)
        {
            if (ImGui.TreeNode(rootName))
            {
                if (ImGui.SliderFloat("centerX", ref centerX, 0.0f, 5.0f))
                {
                    (dot.uniforms["center"] as GLUniform)["value"] = new THREE.Math.Vector2(centerX,centerY);
                }
                if (ImGui.SliderFloat("centerY", ref centerY, 0.0f, 5.0f))
                {
                    (dot.uniforms["center"] as GLUniform)["value"] = new THREE.Math.Vector2(centerX, centerY);
                }
                if (ImGui.SliderFloat("angle", ref angle, 0.0f, 3.14f))
                {
                    (dot.uniforms["angle"] as GLUniform)["value"] = angle;
                }
                if (ImGui.SliderFloat("scale", ref scale, 0.0f, 10.0f))
                {
                    (dot.uniforms["scale"] as GLUniform)["value"] = scale;
                }
                ImGui.TreePop();
            }
        }

        public virtual void AddBloomPassControl(string rootName,BloomPass bloom)
        {
            bool changed = false;
            if (ImGui.TreeNode(rootName))
            {
                if (ImGui.SliderFloat("strength", ref strength, 0.0f, 5.0f))
                {
                    changed = true;
                }
                if (ImGui.SliderInt("kernelSize", ref kernelSize, 10, 100))
                {
                    changed = true;
                }
                if (ImGui.SliderFloat("sigma", ref sigma, 1.0f, 8.0f))
                {
                    changed = true;
                }
                if (ImGui.SliderInt("resolution", ref resolution, 100, 156))
                {
                    changed = true;
                }
                if (changed)
                {
                    BloomPass pass = new BloomPass(strength, kernelSize, sigma, resolution);
                    bloomComposer.Passes[1] = pass;
                }
                ImGui.TreePop();
            }
        }
        public virtual void AddGiltchPassControl(string rootName,GlitchPass pass)
        {
            if (ImGui.TreeNode(rootName))
            {
                if (ImGui.SliderInt("dtsize", ref dtSize, 0, 1024))
                {
                    composer1.Passes[1] = new GlitchPass(dtSize);
                }
               
                ImGui.TreePop();
            }
        }
        public override void Resize(System.Drawing.Size clientSize)
        {
            base.Resize(clientSize);
        }

        public virtual void AddHalftonePassControl(string rootName,HalftonePass pass)
        {
            if (ImGui.TreeNode(rootName))
            {
                
                if (ImGui.Combo("shape", ref shape, "dot\0ellipse\0line\0square\0"))
                {
                    Hashtable param = new Hashtable { { "shape", shape } };
                 

                    HalftonePass halfPass = new HalftonePass(width,height,param);
                   
                    composer2.Passes[1] = halfPass;
                }
                if (ImGui.SliderFloat("radius", ref radius, 0, 40.0f))
                {
                    Hashtable param = new Hashtable { { "radius", radius } };
                    HalftonePass halfPass = new HalftonePass(width, height, param);                   
                    composer2.Passes[1] = halfPass;
                }
                if (ImGui.SliderFloat("rotateR", ref rotateR, 0, (float)System.Math.PI*2))
                {
                    Hashtable param = new Hashtable { { "rotateR", rotateR } };

                    HalftonePass halfPass = new HalftonePass(width, height, param);
                    composer2.Passes[1] = halfPass;
                }
                if (ImGui.SliderFloat("rotateG", ref rotateG, 0, (float)System.Math.PI * 2))
                {
                    Hashtable param = new Hashtable { { "rotateG", rotateG } };

                    HalftonePass halfPass = new HalftonePass(width, height, param);
                    composer2.Passes[1] = halfPass;
                }
                if (ImGui.SliderFloat("rotateB", ref rotateB, 0, (float)System.Math.PI * 2))
                {
                    Hashtable param = new Hashtable { { "rotateB", rotateB } };

                    HalftonePass halfPass = new HalftonePass(width, height, param);
                    composer2.Passes[1] = halfPass;
                }
                if (ImGui.SliderFloat("scatter", ref scatter, 0, 2.0f))
                {
                    Hashtable param = new Hashtable { { "scatter", scatter } };

                    HalftonePass halfPass = new HalftonePass(width, height, param);
                    composer2.Passes[1] = halfPass;
                }
                if (ImGui.SliderFloat("width", ref width, 0, 15.0f))
                {
                    Hashtable param = new Hashtable { { "width", width } };

                    HalftonePass halfPass = new HalftonePass(width, height, param);
                    composer2.Passes[1] = halfPass;
                }
                if (ImGui.SliderFloat("height", ref height, 0, 15.0f))
                {
                    Hashtable param = new Hashtable { { "height", height } };

                    HalftonePass halfPass = new HalftonePass(width, height, param);
                    composer2.Passes[1] = halfPass;
                }
                if (ImGui.SliderFloat("blending", ref blending, 0, 2.0f))
                {
                    Hashtable param = new Hashtable { { "blending", blending } };

                    HalftonePass halfPass = new HalftonePass(width, height, param);
                    composer2.Passes[1] = halfPass;
                }
                if (ImGui.Combo("blendingMode", ref blendingMode, "linear\0multiply\0add\0lighter\0darker\0"))
                {
                    Hashtable param = new Hashtable { { "blendingMode", shape } };


                    HalftonePass halfPass = new HalftonePass(width, height, param);

                    composer2.Passes[1] = halfPass;
                }
                if (ImGui.Checkbox("greyscale", ref greyScale))
                {
                    Hashtable param = new Hashtable { { "greyscale", greyScale } };
                    HalftonePass halfPass = new HalftonePass(width,height,param);
                    composer2.Passes[1] = halfPass;
                }
               
                ImGui.TreePop();
            }        
        
        }
        public virtual void AddOutlinePassControls(string rootName, OutlinePass pass)        
        {
            System.Numerics.Vector3 color1 = new System.Numerics.Vector3(visibleEdgeColor.R, visibleEdgeColor.G, visibleEdgeColor.B);
            System.Numerics.Vector3 color2 = new System.Numerics.Vector3(hiddenEdgeColor.R, hiddenEdgeColor.G, hiddenEdgeColor.B);
            
            if (ImGui.TreeNode(rootName))
            {
               
                if (ImGui.SliderFloat("edgeStrength", ref edgeStrength, 0.01f, 10.0f))
                {
                    outlinePass.edgeStrength = edgeStrength;
                }
                if (ImGui.SliderFloat("edgeGlow", ref edgeGlow, 0.0f, 1.0f))
                {
                    outlinePass.edgeGlow = edgeGlow;
                }
                if (ImGui.SliderFloat("edgeThickness", ref edgeThickness, 1.0f, 4.0f))
                {
                    outlinePass.edgeThickness = edgeThickness;
                }
                if (ImGui.SliderFloat("pulsePeriod", ref pulsePeriod, 0.0f, 5.0f))
                {
                    outlinePass.pulsePeriod = pulsePeriod;
                }
                if(ImGui.ColorPicker3("visibleEdgeColor",ref color1))
                {
                    visibleEdgeColor = new Color(color1.X, color1.Y, color1.Z);
                    outlinePass.visibleEdgeColor = visibleEdgeColor;
                }
                if (ImGui.ColorPicker3("hiddenEdgeColor", ref color2))
                {
                    hiddenEdgeColor = new Color(color2.X, color2.Y, color2.Z);
                    outlinePass.hiddenEdgeColor = hiddenEdgeColor;
                }
                ImGui.TreePop();
            }
        }
        public virtual Mesh AddGeometryWithMaterial(Scene scene, BufferGeometry geometry, string name, Material material)
        {
            var mesh = new Mesh(geometry, material);
            mesh.CastShadow = true;

            scene.Add(mesh);

            materialsLib.Add(name, material);
            return mesh;
        }
        public virtual Mesh AddGeometry(Scene scene, Geometry geometry, string name, Texture texture)
        {
            var mat = new MeshStandardMaterial()
            {
                Map = texture,
                Metalness = 0.2f,
                Roughness = 0.07f
            };

            var mesh = new Mesh(geometry, mat);

            mesh.CastShadow = true;

            scene.Add(mesh);

            materialsLib.Add(name, mat);

            return mesh;
        }

        public virtual void AddBasicMaterialSettings(Material material, string name)
        {
            int currentSide = material.Side;
            int shadowSide = material.ShadowSide == null ? 0 : material.ShadowSide.Value;
            if (ImGui.TreeNode(name))
            {
                ImGui.Text($"id={material.Id}");
                ImGui.Text($"uuid={material.Uuid}");
                ImGui.Text($"name={material.Name}");
                ImGui.SliderFloat("opacity", ref material.Opacity, 0.0f, 1.0f);
                ImGui.Checkbox("transparent", ref material.Transparent);
                ImGui.Checkbox("visible", ref material.Visible);
                if (ImGui.Combo("side", ref currentSide, "FrontSide\0BackSide\0BothSide\0"))
                {
                    material.Side = currentSide;
                }
                ImGui.Checkbox("colorWrite", ref material.ColorWrite);
                if (ImGui.Checkbox("flatShading", ref material.FlatShading))
                {
                    material.NeedsUpdate = true;
                }
                ImGui.Checkbox("premultipliedAlpha", ref material.PremultipliedAlpha);
                ImGui.Checkbox("dithering", ref material.Dithering);
                if (ImGui.Combo("shadowSide", ref shadowSide, "FrontSide\0BackSide\0BothSide\0"))
                {
                    material.ShadowSide = shadowSide;
                }
                ImGui.Checkbox("fog", ref material.Fog);
                ImGui.TreePop();
            }
        }
        public virtual void AddSpecificMaterialSettings(Material material, string name)
        {
            Color materialColor = material.Color.Value;
            Color emissiveColor = material.Emissive.Value;
            System.Numerics.Vector3 color = new System.Numerics.Vector3(materialColor.R, materialColor.G, materialColor.B);
            System.Numerics.Vector3 emissive = new System.Numerics.Vector3(emissiveColor.R, emissiveColor.G, emissiveColor.B);
            if (ImGui.TreeNode(name))
            {
                switch (material.type)
                {
                    case "MeshNormalMaterial":
                        ImGui.Checkbox("wireframe", ref material.Wireframe);
                        break;
                    case "MeshPhongMaterial":
                        ImGui.SliderFloat("shininess", ref material.Shininess, 0, 100);
                        break;
                    case "MeshStandardMaterial":
                        if (ImGui.ColorPicker3("color", ref color))
                        {
                            Color mColor = new Color(color.X, color.Y, color.Z);
                            material.Color = mColor;
                        }
                        if (ImGui.ColorPicker3("emissive", ref emissive))
                        {
                            Color eColor = new Color(emissive.X, emissive.Y, emissive.Z);
                            material.Emissive = eColor;
                        }
                        ImGui.SliderFloat("metalness", ref material.Metalness, 0, 1);
                        ImGui.SliderFloat("roughness", ref material.Roughness, 0, 1);
                        ImGui.Checkbox("wireframe", ref material.Wireframe);
                        break;
                }
                ImGui.TreePop();
            }
        }
        
        
    }
}

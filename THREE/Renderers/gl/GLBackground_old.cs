using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using THREE.Cameras;
using THREE.Scenes;
using THREE.Math;
using THREE.Objects;
using THREE.Textures;
using THREE.Geometries;
using THREE.Materials;
using System.Collections;
using THREE.Renderers.Shaders;
using THREE.Core;
namespace THREE.Renderers.gl
{
    public class GLBackground
    {
        public Color ClearColor = Color.ColorName(ColorKeywords.black);

        public float ClearAlpha = 0;

        public Mesh PlaneMesh;

        public Mesh BoxMesh;

        public object currentBackground;

        public int currentBackgroundVersion = 0;

        private GLState state;

        private bool premultipliedAlpha;

        private GLRenderer renderer;

        private GLObjects objects;


        public GLBackground(GLRenderer renderer, GLState state, GLObjects objects, bool premultipliedAlpha)
        {
            this.state = state;

            this.premultipliedAlpha = premultipliedAlpha;

            this.renderer = renderer;

            this.objects = objects;
        }

        public Color GetClearColor()
        {
            return ClearColor;
        }

        public void SetClearColor(Color color, float alpha=1)
        {
            ClearColor = color;
            ClearAlpha = alpha;

            SetClear(ClearColor, ClearAlpha);

        }

        public float GetClearAlpha()
        {
            return ClearAlpha;
        }

        public void SetClearAlpah(float alpha)
        {
            this.ClearAlpha = alpha;
            SetClear(this.ClearColor, this.ClearAlpha);
        }

        public void SetClear(Color color,float alpha)
        {
            state.buffers.color.SetClear(color.R, color.G, color.B, alpha, this.premultipliedAlpha);

        }       

        public void Render(GLRenderList renderList, Scene scene, Camera camera, bool forceClear)
        {
            var background = scene.Background;

            //var vr = Renderer.vr;
            //var session = vr.getSession && vr.getSession();

            //if ( session && session.environmentBlendMode == 'additive' ) {
            //    background = null;
            //}

            if (background == null)
            {
                SetClear(this.ClearColor, this.ClearAlpha);
                currentBackground = null;
                currentBackgroundVersion = 0;
            }
            else if (background is Color)
            {
                SetClear((Color)background, 1);
                forceClear = true;
                currentBackground = null;
                currentBackgroundVersion = 0;
            }

            if (renderer.AutoClear || forceClear)
            {
                renderer.Clear(renderer.AutoClearColor, renderer.AutoClearDepth, renderer.AutoClearStencil);
            }

            if (background != null && (background is CubeTexture || background is GLCubeRenderTarget))
            {
                if (BoxMesh == null)
                {
                    Hashtable parameters = new Hashtable();

                    GLShader shader = (renderer.ShaderLib["cube"] as GLShader);

                    parameters.Add("type","BackgroundCubeMaterial");
                    parameters.Add("uniforms",UniformsUtils.CloneUniforms(shader.Uniforms));
					parameters.Add("vertexShader",shader.VertexShader);
					parameters.Add("fragmentShader",shader.FragmentShader);
					parameters.Add("side",Constants.BackSide);
					parameters.Add("depthTest",false);
					parameters.Add("depthWrite", false);
					parameters.Add("fog",false);

                    BoxMesh = new Mesh(new BoxBufferGeometry(1, 1, 1), new ShaderMaterial(parameters));

                    (BoxMesh.Geometry as BufferGeometry).deleteAttribute("normal");
                    (BoxMesh.Geometry as BufferGeometry).deleteAttribute("uv");
                    // boxmesh.onbeforeRender
                    BoxMesh.Material.Map = (Texture)(shader.Uniforms["tCube"] as GLUniform)["value"];

                    this.objects.Update(BoxMesh);
                }

                var texture = background is GLCubeRenderTarget ? (background as GLCubeRenderTarget).Texture : background;

                (BoxMesh.Material as ShaderMaterial).Uniforms["tCube"] = new GLUniform { { "value", texture } };
                (BoxMesh.Material as ShaderMaterial).Uniforms["tFlip"] = new GLUniform { { "value", background is GLCubeRenderTarget ? 1 : -1 } };

                if (currentBackground != background ||(!(texture is Color) && currentBackgroundVersion != (texture as CubeTexture).version))
                {
                    BoxMesh.Material.NeedsUpdate = true;

                    currentBackground = background;
                    currentBackgroundVersion = (texture as CubeTexture).version;
                }

                renderList.Unshift(BoxMesh, BoxMesh.Geometry as BufferGeometry, BoxMesh.Material, 0, 0, null);

            }
            else if (background is Texture)
            {
                if (PlaneMesh == null)
                {
                    Hashtable parameters = new Hashtable();

                    GLShader shader = (renderer.ShaderLib["background"] as GLShader);

                    parameters.Add("type","BackgroundMaterial");
                    parameters.Add("uniforms",UniformsUtils.CloneUniforms(shader.Uniforms));
					parameters.Add("vertexShader",shader.VertexShader);
					parameters.Add("fragmentShader",shader.FragmentShader);
					parameters.Add("side",Constants.FrontSide);
					parameters.Add("depthTest",false);
					parameters.Add("depthWrite", false);
					parameters.Add("fog",false);

                    PlaneMesh = new Mesh(new PlaneBufferGeometry(2, 2), new ShaderMaterial(parameters));                   

                    (PlaneMesh.Geometry as BufferGeometry).deleteAttribute("normal");

                    PlaneMesh.Material.Map = (Texture)(shader.Uniforms["t2D"] as GLUniform)["value"];

                    objects.Update(PlaneMesh);
                }

                (PlaneMesh.Material as ShaderMaterial).Uniforms["t2D"] = new GLUniform { { "value", background } };

                if ((background as Texture).MatrixAutoUpdate == true)
                {
                    (background as Texture).UpdateMatrix();
                }

                (PlaneMesh.Material as ShaderMaterial).Uniforms["uvTransform"] = new GLUniform { { "value", (background as Texture).Matrix } };

                if (currentBackground != background || currentBackgroundVersion != (background as Texture).version)
                {
                    PlaneMesh.Material.NeedsUpdate = true;

                    currentBackground = background;
                    currentBackgroundVersion = (background as Texture).version;
                }

                renderList.Unshift(PlaneMesh, PlaneMesh.Geometry as BufferGeometry, PlaneMesh.Material, 0, 0, null);
            }
        }

    }
}

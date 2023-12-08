using OpenTK.Graphics.ES30;
using System.Collections;
namespace THREE
{
    public class GLBackground
    {
        public Color ClearColor = Color.ColorName(ColorKeywords.black);

        public float ClearAlpha = 0;

        public Mesh PlaneMesh;

        public Mesh BoxMesh;

        public object currentBackground;

        public int currentBackgroundVersion = 0;

        int? currentTonemapping = null;

        private GLState state;

        private bool premultipliedAlpha;

        private GLRenderer renderer;

        private GLObjects objects;

        private GLCubeMap cubeMaps;
        public GLBackground(GLRenderer renderer,GLCubeMap cubeMaps, GLState state, GLObjects objects, bool premultipliedAlpha)
        {
            this.state = state;

            this.premultipliedAlpha = premultipliedAlpha;

            this.renderer = renderer;

            this.objects = objects;

            this.cubeMaps = cubeMaps;
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

        public void SetClearAlpha(float alpha)
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
            var background =  scene is Scene ? scene.Background : null;

            if(background!=null && background is Texture)
            {
                background = cubeMaps.Get(background as Texture);
            }

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
               ErrorCode value = GL.GetError();
            }

            if (background != null && (background is CubeTexture || background is GLCubeRenderTarget || (background is Texture && (background as Texture).Mapping == Constants.CubeUVReflectionMapping)))
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
                    BoxMesh.OnBeforeRender = delegate (GLRenderer r, Scene s, Camera c, Geometry g, Material m, DrawRange? d, GLRenderTarget gt)
                    {
                        BoxMesh.MatrixWorld.CopyPosition(c.MatrixWorld);
                    };

                    BoxMesh.Material.EnvMap = (Texture)(shader.Uniforms["envMap"] as GLUniform)["value"];

                    this.objects.Update(BoxMesh);
                }
                
                if(background is GLCubeRenderTarget)
                {
                    background = (background as GLCubeRenderTarget).Texture;
                }

                (BoxMesh.Material as ShaderMaterial).Uniforms["envMap"] = new GLUniform { { "value", background } };
                ((BoxMesh.Material as ShaderMaterial).Uniforms["flipEnvMap"] as GLUniform)["value"] = background is CubeTexture ? -1.0f : 1.0f;
                BoxMesh.Material.EnvMap = (Texture)background;

               

                var texture = background is GLCubeRenderTarget ? (background as GLCubeRenderTarget).Texture : background;                             

                if (currentBackground != background ||(!(texture is Color) && currentBackgroundVersion != (texture as CubeTexture).version)
                    || (currentTonemapping!=null && currentTonemapping.Value !=renderer.ToneMapping))
                {
                    BoxMesh.Material.NeedsUpdate = true;

                    currentBackground = background;
                    currentBackgroundVersion = (texture as CubeTexture).version;
                    currentTonemapping = renderer.ToneMapping;
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

                if (currentBackground != background || currentBackgroundVersion != (background as Texture).version
                    || (currentTonemapping!=null && currentTonemapping.Value!=renderer.ToneMapping))
                {
                    PlaneMesh.Material.NeedsUpdate = true;

                    currentBackground = background;
                    currentBackgroundVersion = (background as Texture).version;
                    currentTonemapping = renderer.ToneMapping;
                }

                renderList.Unshift(PlaneMesh, PlaneMesh.Geometry as BufferGeometry, PlaneMesh.Material, 0, 0, null);
            }
        }

    }
}

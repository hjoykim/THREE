using OpenTK;
using OpenTK.Graphics.ES30;
using System.Collections;

namespace THREE
{
    public class CinematicCamera : PerspectiveCamera
    {
        public Hashtable postprocessing = new Hashtable();
        Hashtable ShaderSettings = new Hashtable() { { "rings", 3 }, { "samples", 4 } };
        BokehDepthShader depthShader = new BokehDepthShader();
        BokehShader2 bokehShader = new BokehShader2();
        ShaderMaterial materialDepth;

        float FNumber;

        float Coc;

        float Aperture;

        float HyperFocal;

        float NearPoint;
        float FarPoint;
        float DepthOfField;
        float Sdistance;
        float Ldistance;

        GLControl glControl;
        public CinematicCamera(GLControl glControl,float fov,float aspect,float near,float far) : base(fov,aspect,near,far)
        {
            type = "CinematicCamera";
            this.glControl = glControl;
            postprocessing.Add("enabled", true);

            materialDepth = new ShaderMaterial()
            {
                Uniforms = depthShader.Uniforms,
                VertexShader = depthShader.VertexShader,
                FragmentShader = depthShader.FragmentShader,
            };

            materialDepth.Uniforms["mNear"]= new GLUniform { { "value", near } };
            materialDepth.Uniforms["mFar"] = new GLUniform { { "value", far } };

            SetLens();

            InitProcessing();
        }

        public void SetLens(float? focalLength=null,int? filmGauge=null,float? fNumber=null,float? coc=null)
        {
            // In case of cinematicCamera, having a default lens set is important
            if (focalLength == null) focalLength = 35;
            if (filmGauge != null) this.FilmGauge = filmGauge.Value;

            this.SetFocalLength(focalLength.Value);

            // if fnumber and coc are not provided, cinematicCamera tries to act as a basic PerspectiveCamera
            if (fNumber == null) fNumber = 8;
            if (coc == null) coc = 0.019f;

            this.FNumber = fNumber.Value;
            this.Coc = coc.Value;

            // fNumber is focalLength by aperture
            this.Aperture = focalLength.Value / this.FNumber;

            // hyperFocal is required to calculate depthOfField when a lens tries to focus at a distance with given fNumber and focalLength
            this.HyperFocal = (focalLength.Value * focalLength.Value) / (this.Aperture * this.Coc);

        }

        public float Linearize(float depth)
        {
            var zfar = this.Far;
            var znear = this.Near;

            return -zfar * znear / (depth * (zfar - znear) - zfar);
        }

        public float SmoothStep(float near,float far,float depth)
        {
            var x = this.Saturate((depth - near) / (far - near));

            return x * x * (3 - 2 * x);
        }

        public float Saturate(float x)
        {
            return (float)System.Math.Max(0, System.Math.Min(1, x));
        }

        public void FocusAt(float? focusDistance)
        {
            if (focusDistance == null) focusDistance = 20;

            var focalLength = this.GetFocalLength();

            // distance from the camera (normal to frustrum) to focus on
            this.focus = focusDistance.Value;

            // the nearest point from the camera which is in focus (unused)
            this.NearPoint = (this.HyperFocal * this.focus) / (this.HyperFocal + (this.focus - focalLength));

            // the farthest point from the camera which is in focus (unused)
            this.FarPoint = (this.HyperFocal * this.focus) / (this.HyperFocal - (this.focus - focalLength));

            // the gap or width of the space in which is everything is in focus (unused)
            this.DepthOfField = this.FarPoint - this.NearPoint;

            // Considering minimum distance of focus for a standard lens (unused)
            if (this.DepthOfField < 0) this.DepthOfField = 0;

            this.Sdistance = this.SmoothStep(this.Near, this.Far, this.focus);

            this.Ldistance = this.Linearize(1 - this.Sdistance);

            (this.postprocessing["bokeh_uniforms"] as GLUniforms)["focalDepth"] = new GLUniform{ { "value",this.Ldistance} };
        }

        public void InitProcessing()
        {
            if ((bool)this.postprocessing["enabled"])
            {

                this.postprocessing["scene"] = new Scene();

                this.postprocessing["camera"] = new OrthographicCamera(glControl.Width / -2, glControl.Width / 2, glControl.Height / 2, glControl.Height / -2, -10000, 10000);

                (this.postprocessing["scene"] as Scene).Add(this.postprocessing["camera"] as OrthographicCamera);

                var pars = new Hashtable { { "minFilter", Constants.LinearFilter }, { "magFilter", Constants.LinearFilter }, { "format", Constants.RGBFormat } };
                this.postprocessing["rtTextureDepth"] = new GLRenderTarget(glControl.Width, glControl.Height, pars);
                this.postprocessing["rtTextureColor"] = new GLRenderTarget(glControl.Width, glControl.Height, pars);

                var bokeh_shader = bokehShader;

                this.postprocessing["bokeh_uniforms"] = UniformsUtils.CloneUniforms(bokeh_shader.Uniforms);

                (this.postprocessing["bokeh_uniforms"] as GLUniforms)["tColor"] = new GLUniform { { "value", (this.postprocessing["rtTextureColor"] as GLRenderTarget).Texture } };
                (this.postprocessing["bokeh_uniforms"] as GLUniforms)["tDepth"] = new GLUniform { { "value", (this.postprocessing["rtTextureDepth"] as GLRenderTarget).Texture } };

                (this.postprocessing["bokeh_uniforms"] as GLUniforms)["manualdof"] = new GLUniform { { "value", 0 } };
                (this.postprocessing["bokeh_uniforms"] as GLUniforms)["shaderFocus"] = new GLUniform { { "value", 0 } };

                (this.postprocessing["bokeh_uniforms"] as GLUniforms)["fstop"] = new GLUniform { { "value", 2.8f } };

                (this.postprocessing["bokeh_uniforms"] as GLUniforms)["showFocus"] = new GLUniform { { "value", 1 } };

                (this.postprocessing["bokeh_uniforms"] as GLUniforms)["focalDepth"] = new GLUniform { { "value", 0.1f } };

                //console.log( this.postprocessing["bokeh_uniforms"] as GLUniforms)[ "focalDepth" ] = new GLUniform{{"value", );

                (this.postprocessing["bokeh_uniforms"] as GLUniforms)["znear"] = new GLUniform { { "value", this.Near } };
                (this.postprocessing["bokeh_uniforms"] as GLUniforms)["zfar"] = new GLUniform { { "value", this.Near } };


                (this.postprocessing["bokeh_uniforms"] as GLUniforms)["textureWidth"] = new GLUniform { { "value", glControl.Width } };

                (this.postprocessing["bokeh_uniforms"] as GLUniforms)["textureHeight"] = new GLUniform { { "value", glControl.Height } };

                this.postprocessing["materialBokeh"] = new ShaderMaterial()
                {
                    Uniforms = this.postprocessing["bokeh_uniforms"] as GLUniforms,
                    VertexShader = bokeh_shader.VertexShader,
                    FragmentShader = bokeh_shader.FragmentShader,
                    Defines = new Hashtable{
                        {"RINGS", this.ShaderSettings["rings"] },

                        {"SAMPLES", this.ShaderSettings["samples"] },

                        {"DEPTH_PACKING", 1 }

                    }
                };

                this.postprocessing["quad"] = new Mesh(new PlaneBufferGeometry(glControl.Width, glControl.Height), this.postprocessing["materialBokeh"] as ShaderMaterial);
                (this.postprocessing["quad"] as Mesh).Position.Z = -500;
                (this.postprocessing["scene"] as Scene).Add(this.postprocessing["quad"] as Mesh);

            }
        }
        public void RenderCinematic(Scene scene,GLRenderer renderer)
        {
            if ((bool)this.postprocessing["enabled"])
            {

                var currentRenderTarget = renderer.GetRenderTarget();

                renderer.Clear();

                // Render scene into texture

                scene.OverrideMaterial = null;
                renderer.SetRenderTarget(this.postprocessing["rtTextureColor"] as GLRenderTarget);
                renderer.Clear();
                renderer.Render(scene, this);

                // Render depth into texture

                scene.OverrideMaterial = this.materialDepth;
                renderer.SetRenderTarget(this.postprocessing["rtTextureDepth"] as GLRenderTarget);
                renderer.Clear();
                renderer.Render(scene, this);

                // Render bokeh composite

                renderer.SetRenderTarget(null);
                renderer.Render(this.postprocessing["scene"] as Scene, this.postprocessing["camera"] as Camera);

                renderer.SetRenderTarget(currentRenderTarget);

                GL.ActiveTexture(TextureUnit.Texture0);
               
            }
        }
    }
}

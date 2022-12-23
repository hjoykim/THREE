using System;
using System.Collections;
using System.Collections.Generic;
using OpenTK.Graphics.ES30;
using System.IO;
using System.Diagnostics;


namespace THREE
{
    public struct ShaderInfo
    {
        public string FilePath;
        public ShaderType ShaderType;
    }
    public sealed class ShaderLib : Hashtable
    {
        private const int FOURCC_DXT1 = 0x31545844; // Equivalent to "DXT1" in ASCII
        private const int FOURCC_DXT3 = 0x33545844; // Equivalent to "DXT3" in ASCII
        private const int FOURCC_DXT5 = 0x35545844; // Equivalent to "DXT5" in ASCII       

        public UniformsLib UniformsLib = new UniformsLib();


        public Dictionary<string, string> ShaderChunkDictionary = new Dictionary<string, string>();
       
        //public readonly ShaderChunk ShaderChunk = new ShaderChunk();

        public ShaderLib()
        {
            ShaderChunkDictionary = new Dictionary<string, string>()
            {

{"alphamap_fragment", ShaderChunk.alphamap_fragment},
{"alphamap_pars_fragment", ShaderChunk.alphamap_pars_fragment},
{"alphatest_fragment", ShaderChunk.alphatest_fragment},
{"aomap_fragment", ShaderChunk.aomap_fragment},
{"aomap_pars_fragment", ShaderChunk.aomap_pars_fragment},
{"begin_vertex", ShaderChunk.begin_vertex},
{"beginnormal_vertex", ShaderChunk.beginnormal_vertex},
{"bsdfs", ShaderChunk.bsdfs},
{"bumpmap_pars_fragment", ShaderChunk.bumpmap_pars_fragment},
{"clipping_planes_fragment", ShaderChunk.clipping_planes_fragment},
{"clipping_planes_pars_fragment", ShaderChunk.clipping_planes_pars_fragment},
{"clipping_planes_pars_vertex", ShaderChunk.clipping_planes_pars_vertex},
{"clipping_planes_vertex", ShaderChunk.clipping_planes_vertex},
{"color_fragment", ShaderChunk.color_fragment},
{"color_pars_fragment", ShaderChunk.color_pars_fragment},
{"color_pars_vertex", ShaderChunk.color_pars_vertex},
{"color_vertex", ShaderChunk.color_vertex},
{"common", ShaderChunk.common},
{"cube_uv_reflection_fragment", ShaderChunk.cube_uv_reflection_fragment},
{"defaultnormal_vertex", ShaderChunk.defaultnormal_vertex},
{"displacementmap_pars_vertex", ShaderChunk.displacementmap_pars_vertex},
{"displacementmap_vertex", ShaderChunk.displacementmap_vertex},
{"emissivemap_fragment", ShaderChunk.emissivemap_fragment},
{"emissivemap_pars_fragment", ShaderChunk.emissivemap_pars_fragment},
{"encodings_fragment", ShaderChunk.encodings_fragment},
{"encodings_pars_fragment", ShaderChunk.encodings_pars_fragment},
{"envmap_fragment", ShaderChunk.envmap_fragment},
{"envmap_common_pars_fragment", ShaderChunk.envmap_common_pars_fragment},
{"envmap_pars_fragment", ShaderChunk.envmap_pars_fragment},
{"envmap_pars_vertex", ShaderChunk.envmap_pars_vertex},
{"envmap_physical_pars_fragment", ShaderChunk.envmap_physical_pars_fragment},
{"envmap_vertex", ShaderChunk.envmap_vertex},
{"fog_vertex", ShaderChunk.fog_vertex},
{"fog_pars_vertex", ShaderChunk.fog_pars_vertex},
{"fog_fragment", ShaderChunk.fog_fragment},
{"fog_pars_fragment", ShaderChunk.fog_pars_fragment},
{"gradientmap_pars_fragment", ShaderChunk.gradientmap_pars_fragment},
{"lightmap_fragment", ShaderChunk.lightmap_fragment},
{"lightmap_pars_fragment", ShaderChunk.lightmap_pars_fragment},
{"lights_lambert_vertex", ShaderChunk.lights_lambert_vertex},
{"lights_pars_begin", ShaderChunk.lights_pars_begin},
{"lights_toon_fragment", ShaderChunk.lights_toon_fragment},
{"lights_toon_pars_fragment", ShaderChunk.lights_toon_pars_fragment},
{"lights_phong_fragment", ShaderChunk.lights_phong_fragment},
{"lights_phong_pars_fragment", ShaderChunk.lights_phong_pars_fragment},
{"lights_physical_fragment", ShaderChunk.lights_physical_fragment},
{"lights_physical_pars_fragment", ShaderChunk.lights_physical_pars_fragment},
{"lights_fragment_begin", ShaderChunk.lights_fragment_begin},
{"lights_fragment_maps", ShaderChunk.lights_fragment_maps},
{"lights_fragment_end", ShaderChunk.lights_fragment_end},
{"logdepthbuf_fragment", ShaderChunk.logdepthbuf_fragment},
{"logdepthbuf_pars_fragment", ShaderChunk.logdepthbuf_pars_fragment},
{"logdepthbuf_pars_vertex", ShaderChunk.logdepthbuf_pars_vertex},
{"logdepthbuf_vertex", ShaderChunk.logdepthbuf_vertex},
{"map_fragment", ShaderChunk.map_fragment},
{"map_pars_fragment", ShaderChunk.map_pars_fragment},
{"map_particle_fragment", ShaderChunk.map_particle_fragment},
{"map_particle_pars_fragment", ShaderChunk.map_particle_pars_fragment},
{"metalnessmap_fragment", ShaderChunk.metalnessmap_fragment},
{"metalnessmap_pars_fragment", ShaderChunk.metalnessmap_pars_fragment},
{"morphnormal_vertex", ShaderChunk.morphnormal_vertex},
{"morphtarget_pars_vertex", ShaderChunk.morphtarget_pars_vertex},
{"morphtarget_vertex", ShaderChunk.morphtarget_vertex},
{"normal_fragment_begin", ShaderChunk.normal_fragment_begin},
{"normal_fragment_maps", ShaderChunk.normal_fragment_maps},
{"normalmap_pars_fragment", ShaderChunk.normalmap_pars_fragment},
{"clearcoat_normal_fragment_begin", ShaderChunk.clearcoat_normal_fragment_begin},
{"clearcoat_normal_fragment_maps", ShaderChunk.clearcoat_normal_fragment_maps},
{"clearcoat_pars_fragment", ShaderChunk.clearcoat_pars_fragment},
{"packing", ShaderChunk.packing},
{"premultiplied_alpha_fragment", ShaderChunk.premultiplied_alpha_fragment},
{"project_vertex", ShaderChunk.project_vertex},
{"dithering_fragment", ShaderChunk.dithering_fragment},
{"dithering_pars_fragment", ShaderChunk.dithering_pars_fragment},
{"roughnessmap_fragment", ShaderChunk.roughnessmap_fragment},
{"roughnessmap_pars_fragment", ShaderChunk.roughnessmap_pars_fragment},
{"shadowmap_pars_fragment", ShaderChunk.shadowmap_pars_fragment},
{"shadowmap_pars_vertex", ShaderChunk.shadowmap_pars_vertex},
{"shadowmap_vertex", ShaderChunk.shadowmap_vertex},
{"shadowmask_pars_fragment", ShaderChunk.shadowmask_pars_fragment},
{"skinbase_vertex", ShaderChunk.skinbase_vertex},
{"skinning_pars_vertex", ShaderChunk.skinning_pars_vertex},
{"skinning_vertex", ShaderChunk.skinning_vertex},
{"skinnormal_vertex", ShaderChunk.skinnormal_vertex},
{"specularmap_fragment", ShaderChunk.specularmap_fragment},
{"specularmap_pars_fragment", ShaderChunk.specularmap_pars_fragment},
{"tonemapping_fragment", ShaderChunk.tonemapping_fragment},
{"tonemapping_pars_fragment", ShaderChunk.tonemapping_pars_fragment},
{"transmissionmap_fragment", ShaderChunk.transmissionmap_fragment},
{"transmissionmap_pars_fragment", ShaderChunk.transmissionmap_pars_fragment},
{"uv_pars_fragment", ShaderChunk.uv_pars_fragment},
{"uv_pars_vertex", ShaderChunk.uv_pars_vertex},
{"uv_vertex", ShaderChunk.uv_vertex},
{"uv2_pars_fragment", ShaderChunk.uv2_pars_fragment},
{"uv2_pars_vertex", ShaderChunk.uv2_pars_vertex},
{"uv2_vertex", ShaderChunk.uv2_vertex},
{"worldpos_vertex", ShaderChunk.worldpos_vertex},

{"background_frag", ShaderLibVariable.background_frag},
{"background_vert", ShaderLibVariable.background_vert},
{"cube_frag", ShaderLibVariable.cube_frag},
{"cube_vert", ShaderLibVariable.cube_vert},
{"depth_frag", ShaderLibVariable.depth_frag},
{"depth_vert", ShaderLibVariable.depth_vert},
{"distanceRGBA_frag", ShaderLibVariable.distanceRGBA_frag},
{"distanceRGBA_vert", ShaderLibVariable.distanceRGBA_vert},
{"equirect_frag", ShaderLibVariable.equirect_frag},
{"equirect_vert", ShaderLibVariable.equirect_vert},
{"linedashed_frag", ShaderLibVariable.linedashed_frag},
{"linedashed_vert", ShaderLibVariable.linedashed_vert},
{"meshbasic_frag", ShaderLibVariable.meshbasic_frag},
{"meshbasic_vert", ShaderLibVariable.meshbasic_vert},
{"meshlambert_frag", ShaderLibVariable.meshlambert_frag},
{"meshlambert_vert", ShaderLibVariable.meshlambert_vert},
{"meshmatcap_frag", ShaderLibVariable.meshmatcap_frag},
{"meshmatcap_vert", ShaderLibVariable.meshmatcap_vert},
{"meshtoon_frag", ShaderLibVariable.meshtoon_frag},
{"meshtoon_vert", ShaderLibVariable.meshtoon_vert},
{"meshphong_frag", ShaderLibVariable.meshphong_frag},
{"meshphong_vert", ShaderLibVariable.meshphong_vert},
{"meshphysical_frag", ShaderLibVariable.meshphysical_frag},
{"meshphysical_vert", ShaderLibVariable.meshphysical_vert},
{"normal_frag", ShaderLibVariable.normal_frag},
{"normal_vert", ShaderLibVariable.normal_vert},
{"points_frag", ShaderLibVariable.points_frag},
{"points_vert", ShaderLibVariable.points_vert},
{"shadow_frag", ShaderLibVariable.shadow_frag},
{"shadow_vert", ShaderLibVariable.shadow_vert},
{"sprite_frag", ShaderLibVariable.sprite_frag},
{"sprite_vert", ShaderLibVariable.sprite_vert }

            };
            //// ShaderChunk folder
            //var glslFiles = Directory.EnumerateFiles(@".\Renderers\Shaders\ShaderChunk", "*.glsl");
            //if (glslFiles.Count() <= 0)
            //{
            //    throw new FileNotFoundException(".glsl files not found - check the path in ShaderLib.cs, line 25");
            //}

            //foreach (var path in glslFiles)
            //{
            //    this.ShaderChunkDictionary.Add(Path.GetFileNameWithoutExtension(path), path);
            //}


            //// ShaderLib folder
            //glslFiles = Directory.EnumerateFiles(@".\Renderers\Shaders\ShaderLib", "*.glsl");
            //if (glslFiles.Count() <= 0)
            //{
            //    throw new FileNotFoundException(".glsl files not found - check the path in ShaderLib.cs, line 25");
            //}

            //foreach (var path in glslFiles)
            //{
            //    this.ShaderChunkDictionary.Add(Path.GetFileNameWithoutExtension(path), path);
            //}

            this.Add("basic", this.BasicShader());
            this.Add("lambert", this.LambertShader());
            this.Add("phong", this.PhongShader());
            this.Add("standard", this.StandardShader());
            this.Add("toon", this.ToonShader());
            this.Add("matcap", this.MatcapShader());
            this.Add("points", this.PointsShader());
            this.Add("dashed", this.DashedShader());
            this.Add("depth", this.DepthShader());
            this.Add("normal", this.NormalShader());
            this.Add("sprite", this.SpriteShader());
            this.Add("background", this.BackgroundShader());
            this.Add("cube", this.CubeShader());
            this.Add("equirect", this.EquirectShader());
            this.Add("distanceRGBA", this.DistanceRGBAShader());
            this.Add("shadow", this.ShadowShader());
            this.Add("physical", this.PhysicalShader());

        }
        public string getChunk(string chunkName)
        {
            return ShaderChunkDictionary[chunkName];
            //string path;
            //return this.ShaderChunkDictionary.TryGetValue(chunkName, out path) ? File.ReadAllText(path) : string.Empty;
            //return this.ShaderChunk.getChunk(chunkName);
        }
        #region Basic Shader
        private GLShader BasicShader()
        {
            var shader = new GLShader();

            shader.Uniforms = UniformsUtils.Merge(new List<GLUniforms>
                                    {
                                        UniformsLib["common"],
                                        UniformsLib["specularmap"],
                                        UniformsLib["envmap"],
                                        UniformsLib["aomap"],
                                        UniformsLib["lightmap"],
                                        UniformsLib["fog"]
                                    });

            shader.VertexShader =  getChunk("meshbasic_vert");
            shader.FragmentShader = getChunk("meshbasic_frag");

            return shader;
        }
        #endregion

        #region Lambert Shader
        private GLShader LambertShader()
        {
            var shader = new GLShader();

            shader.Uniforms = UniformsUtils.Merge(new List<GLUniforms>
                                    {
                                        UniformsLib["common"],
                                        UniformsLib["specularmap"],
                                        UniformsLib["envmap"],
                                        UniformsLib["aomap"],
                                        UniformsLib["lightmap"],
                                        UniformsLib["emissivemap"],
                                        UniformsLib["fog"],
                                        UniformsLib["lights"],
                                        new GLUniforms {{"emissive",new GLUniform{{"value",Vector3.Zero()}}}}
                                    });

            shader.VertexShader = getChunk("meshlambert_vert");
            shader.FragmentShader = getChunk("meshlambert_frag");

            return shader;

        }
        #endregion Lambert Shader

        #region Phong Shader
        private GLShader PhongShader()
        {
            var shader = new GLShader();

            shader.Uniforms = UniformsUtils.Merge(new List<GLUniforms>
                                    {
                                        UniformsLib["common"],
                                        UniformsLib["specularmap"],
                                        UniformsLib["envmap"],
                                        UniformsLib["aomap"],
                                        UniformsLib["lightmap"],
                                        UniformsLib["emissivemap"],
                                        UniformsLib["bumpmap"],
                                        UniformsLib["normalmap"],
                                        UniformsLib["displacementmap"],
                                        UniformsLib["gradientmap"],
                                        UniformsLib["fog"],
                                        UniformsLib["lights"],
                                        new GLUniforms 
                                        {
                                            {"emissive",new GLUniform{{"value",new Color().SetHex(0x000000)}}},
                                            {"specular",new GLUniform{{"value",new Color().SetHex(0x111111)}}},
                                            {"shininess",new GLUniform{{"value",30}}}
                                        }
                                    });

            shader.VertexShader = getChunk("meshphong_vert");
            shader.FragmentShader = getChunk("meshphong_frag");

            return shader;
        }
        #endregion

        #region Standard Shader
        private GLShader StandardShader()
        {
            var shader = new GLShader();

            shader.Uniforms = UniformsUtils.Merge(new List<GLUniforms>
                                    {
                                        UniformsLib["common"],
                                        UniformsLib["specularmap"],
                                        UniformsLib["envmap"],
                                        UniformsLib["aomap"],
                                        UniformsLib["lightmap"],
                                        UniformsLib["emissivemap"],
                                        UniformsLib["bumpmap"],
                                        UniformsLib["normalmap"],
                                        UniformsLib["displacementmap"],
                                        UniformsLib["roughnessmap"],
                                        UniformsLib["metalnessmap"],
                                        UniformsLib["gradientmap"],
                                        UniformsLib["fog"],
                                        UniformsLib["lights"],
                                        new GLUniforms 
                                        {
                                            {"emissive",new GLUniform{{"value",new Color().SetHex(0x000000)}}},
                                            {"roughness",new GLUniform{{"value",0.5f}}},
                                            {"metalness",new GLUniform{{"value",1}}},
                                            {"envMapIntensity",new GLUniform{{"value",1}}}
                                        }
                                    });

            shader.VertexShader = getChunk("meshphysical_vert");
            shader.FragmentShader = getChunk("meshphysical_frag");

            return shader;
        }
        #endregion

        #region Toon Shader
        private GLShader ToonShader()
        {
            var shader = new GLShader();

            shader.Uniforms = UniformsUtils.Merge(new List<GLUniforms>
                                    {
                                        UniformsLib["common"],
                                        UniformsLib["specularmap"],                                       
                                        UniformsLib["aomap"],
                                        UniformsLib["lightmap"],
                                        UniformsLib["emissivemap"],
                                        UniformsLib["bumpmap"],
                                        UniformsLib["normalmap"],
                                        UniformsLib["displacementmap"],                                       
                                        UniformsLib["gradientmap"],
                                        UniformsLib["fog"],
                                        UniformsLib["lights"],
                                        new GLUniforms
                                        {
                                            {"emissive",new GLUniform{{"value",new Color().SetHex(0x000000)}}},
                                            {"specular",new GLUniform{{"value",new Color().SetHex(0x111111)}}},
                                            {"shininess",new GLUniform{{"value",30}}}
                                        }
                                    });

            shader.VertexShader = getChunk("meshtoon_vert");
            shader.FragmentShader = getChunk("meshtoon_frag");

            return shader;
        }
        #endregion

        #region Matcap Shader
        private GLShader MatcapShader()
        {
            var shader = new GLShader();

            shader.Uniforms = UniformsUtils.Merge(new List<GLUniforms>
                                    {
                                        UniformsLib["common"],
                                        UniformsLib["bumpmap"],
                                        UniformsLib["normalmap"],
                                        UniformsLib["displacementmap"],
                                        UniformsLib["fog"],
                                        new GLUniforms 
                                        {
                                            {"matcap",new GLUniform{{"value",null}}},
                                        }
                                    });

            shader.VertexShader = getChunk("meshmatcap_vert");
            shader.FragmentShader = getChunk("meshmatcap_frag");

            return shader;
        }
        #endregion

        #region Points Shader
        private GLShader PointsShader()
        {
            var shader = new GLShader();

            shader.Uniforms = UniformsUtils.Merge(new List<GLUniforms>
                                    {
                                        UniformsLib["points"],
                                        UniformsLib["fog"],                                       
                                    });

            shader.VertexShader = getChunk("points_vert");
            shader.FragmentShader = getChunk("points_frag");

            return shader;
        }
        #endregion

        #region Dashed Shader
        private GLShader DashedShader()
        {
            var shader = new GLShader();

            shader.Uniforms = UniformsUtils.Merge(new List<GLUniforms>
                                    {
                                        UniformsLib["common"],                                        
                                        UniformsLib["fog"],
                                        new GLUniforms 
                                        {
                                            {"scale",new GLUniform{{"value",1}}},
                                            {"dashSize",new GLUniform{{"value",1}}},
                                            {"totalSize",new GLUniform{{"value",2}}},
                                        }
                                    });

            shader.VertexShader = getChunk("linedashed_vert");
            shader.FragmentShader = getChunk("linedashed_frag");

            return shader;
        }
        #endregion

        #region Depth Shader
        private GLShader DepthShader()
        {
            var shader = new GLShader();

            shader.Uniforms = UniformsUtils.Merge(new List<GLUniforms>
                                    {
                                        UniformsLib["common"],                                       
                                        UniformsLib["displacementmap"],                                        
                                    });

            shader.VertexShader = getChunk("depth_vert");
            shader.FragmentShader = getChunk("depth_frag");

            return shader;
        }
        #endregion

        #region Normal Shader
        private GLShader NormalShader()
        {
            var shader = new GLShader();

            shader.Uniforms = UniformsUtils.Merge(new List<GLUniforms>
                                    {
                                        UniformsLib["common"],                                       
                                        UniformsLib["bumpmap"],
                                        UniformsLib["normalmap"],
                                        UniformsLib["displacementmap"],                                        
                                        new GLUniforms 
                                        {
                                            {"opacity",new GLUniform{{"value",1.0f}}},
                                        }
                                    });

            shader.VertexShader = getChunk("normal_vert");
            shader.FragmentShader = getChunk("normal_frag");

            return shader;
        }
        #endregion

        #region Sprite Shader
        private GLShader SpriteShader()
        {
            var shader = new GLShader();

            shader.Uniforms = UniformsUtils.Merge(new List<GLUniforms>
                                    {
                                        UniformsLib["sprite"],
                                        UniformsLib["fog"],
                                    });

            shader.VertexShader = getChunk("sprite_vert");
            shader.FragmentShader = getChunk("sprite_frag");

            return shader;
        }
        #endregion

        #region Background Shader
        private GLShader BackgroundShader()
        {
            var shader = new GLShader();

            shader.Uniforms = new GLUniforms 
                                {
                                    {"uvTransform",new GLUniform{{"value",new Matrix3()}}},
                                    {"t2D",new GLUniform{{"value",null}}},
                                };

            shader.VertexShader = getChunk("background_vert");
            shader.FragmentShader = getChunk("background_frag");

            return shader;
        }
        #endregion

        #region Cube Shader
        private GLShader CubeShader()
        {
            var shader = new GLShader();

            shader.Uniforms = UniformsUtils.Merge(new List<GLUniforms>
                                    {
                                        UniformsLib["envmap"],
                                        new GLUniforms
                                        {
                                            {"opacity",new GLUniform{{"value",1.0f}}}
                                        }
                                    });

            shader.VertexShader = getChunk("cube_vert");
            shader.FragmentShader = getChunk("cube_frag");

            return shader;
        }
        #endregion  
      
        #region Equirect Shader
        private GLShader EquirectShader()
        {
            var shader = new GLShader();

            shader.Uniforms = new GLUniforms 
                                {
                                    {"tEquirect",new GLUniform{{"value",null}}},
                                };

            shader.VertexShader = getChunk("equirect_vert");
            shader.FragmentShader = getChunk("equirect_frag");

            return shader;
        }
        #endregion  

        #region DistanceRGBA Shader
        private GLShader DistanceRGBAShader()
        {
            var shader = new GLShader();

            shader.Uniforms = UniformsUtils.Merge(new List<GLUniforms>
                                    {
                                        UniformsLib["common"],
                                        UniformsLib["displacementmap"],
                                        new GLUniforms 
                                        {
                                            {"referencePosition",new GLUniform{{"value",Vector3.Zero()}}},
                                            {"nearDistance",new GLUniform{{"value",1}}},
                                            {"farDistance",new GLUniform{{"value",1000}}}
                                        }
                                    });

            shader.VertexShader = getChunk("distanceRGBA_vert");
            shader.FragmentShader = getChunk("distanceRGBA_frag");

            return shader;
        }
        #endregion

        #region Shadow Shader
        private GLShader ShadowShader()
        {
            var shader = new GLShader();

            shader.Uniforms = UniformsUtils.Merge(new List<GLUniforms>
                                    {
                                        UniformsLib["lights"],
                                        UniformsLib["fog"],
                                        new GLUniforms 
                                        {
                                            {"color",new GLUniform{{"value",new Color().SetHex(0x000000)}}},
                                            {"opacity",new GLUniform{{"value",1.0f}}},
                                        }
                                    });

            shader.VertexShader = getChunk("shadow_vert");
            shader.FragmentShader = getChunk("shadow_frag");

            return shader;
        }
        #endregion

        #region PhysicalShader
        private GLShader PhysicalShader()
        {
            var shader = new GLShader();

            shader.Uniforms = UniformsUtils.Merge(new List<GLUniforms>
                                    {
                                        this.StandardShader().Uniforms,
                                        new GLUniforms 
                                        {
                                            {"transparency",new GLUniform{{"value",0}}},
                                            {"clearcoat",new GLUniform{{"value",0}}},
                                            {"clearcoatRoughness",new GLUniform{{"value",0}}},
                                            {"sheen",new GLUniform{{"value",new Color().SetHex(0x000000)}}},
                                            {"clearcoatNormalScale",new GLUniform{{"value",new Vector2(1,1)}}},
                                            {"clearcoatNormalMap",new GLUniform{{"value",null}}},
                                        }
                                    });

            shader.VertexShader = getChunk("meshphysical_vert");
            shader.FragmentShader = getChunk("meshphysical_frag");

            return shader;
        }
        #endregion


        public static int LoadShaders(List<ShaderInfo> list)
        {
            int program = GL.CreateProgram();
            List<int> shaderList = new List<int>();
            foreach(var s in list)
            {
                int shader = GL.CreateShader(s.ShaderType);
                string source = File.ReadAllText(s.FilePath);
                GL.ShaderSource(shader, source);
                GL.CompileShader(shader);

                int compileResult;
                GL.GetShader(shader, ShaderParameter.CompileStatus, out compileResult);
                if (compileResult != 1)
                {
                    Trace.TraceWarning("THREE.WebGLShader: Shader couldn\'t compile.");
                }

                if (GL.GetShaderInfoLog(shader) != string.Empty)
                {
                    Trace.TraceWarning("THREE.WebGLShader: gl.getShaderInfoLog()\n {0}", GL.GetShaderInfoLog(shader));
                    Trace.TraceWarning(addLineNumbers(source));

                    throw new ApplicationException("compilation warning or error, see console");
                }
                GL.AttachShader(program, shader);
                shaderList.Add(shader);
            }
            GL.LinkProgram(program);
            var linkStatus = -1;
            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out linkStatus);
            if (linkStatus == 0)
            {
                int validateStatus;
                GL.GetProgram(program, GetProgramParameterName.ValidateStatus, out validateStatus);

                foreach (var s in shaderList)
                    GL.DeleteShader(s);

                Trace.TraceError("THREE.WebGLProgram: Could not initialise shader.");
                Trace.TraceError("gl.VALIDATE_STATUS {0}", validateStatus);
                Trace.TraceError("gl.getError() {0}", GL.GetError());
            }

            if (GL.GetProgramInfoLog(program) != string.Empty)
            {
                Trace.TraceError("THREE.WebGLProgram: gl.getProgramInfoLog() {0}", GL.GetProgramInfoLog(program));

                throw new ApplicationException("Program failed to link - see Output console. When do the .glsl files loaded???");
            }

            return program;
        }
        public static string addLineNumbers(string code)
        {
            var lines = code.Split('\n');

            for (var i = 0; i < lines.Length; i++)
            {
                lines[i] = (i + 1) + ": " + lines[i];
            }
            return string.Join("\n", lines);
        }
        public static int vglAttachShaderSource(int prog,ShaderType type,string source)
        {
            int sh = GL.CreateShader(type);
            GL.ShaderSource(sh, source);
            GL.CompileShader(sh);
            int compileResult;
            GL.GetShader(sh, ShaderParameter.CompileStatus, out compileResult);
            if (compileResult != 1)
            {
                Trace.TraceWarning("ShaderLib : Shader couldn\'t compile.");
            }

            if (GL.GetShaderInfoLog(sh) != string.Empty)
            {
                Trace.TraceWarning("ShaderLib: gl.getShaderInfoLog()\n {0}", GL.GetShaderInfoLog(sh));
                Trace.TraceWarning(addLineNumbers(source));

                throw new ApplicationException("compilation warning or error, see console");
            }
            GL.AttachShader(prog, sh);
            return sh;

        }
        //public static int LoadDDS(string filename)
        //{
        //    long filesize = new FileInfo(filename).Length;
        //    byte[] data = File.ReadAllBytes(filename);
        //    int position = 0;
        //    string fileCode = new string(System.Text.Encoding.UTF8.GetChars(data, 0, 4));
        //    position += 4;
        //    if(!fileCode.Trim().Equals("DDS"))
        //    {
        //        return 0;
        //    }
        //    byte[] header = new byte[124];
        //    System.Buffer.BlockCopy(data, position, header, 0, 124);
        //    position += 124;

        //    int height = BitConverter.ToInt32(header, 8);
        //    int width = BitConverter.ToInt32(header, 12);
        //    int linearSize = BitConverter.ToInt32(header, 16);
        //    int mipMapCount = BitConverter.ToInt32(header, 24);
        //    int fourCC = BitConverter.ToInt32(header, 80);

        //    int bufsize = mipMapCount > 1 ? linearSize * 2 : linearSize;
        //    int limitSize = (int)(filesize - position);
        //    byte[] buffer = new byte[bufsize > limitSize ? limitSize : bufsize];
        //    System.Buffer.BlockCopy(data, position, buffer, 0,buffer.Length);

        //    int components = (fourCC == FOURCC_DXT1) ? 3 : 4;
        //    InternalFormat format;
        //    switch (fourCC)
        //    {
        //        case FOURCC_DXT1:
        //            format = InternalFormat.CompressedRgbaS3tcDxt1Ext;
        //            break;
        //        case FOURCC_DXT3:
        //            format = InternalFormat.CompressedRgbaS3tcDxt3Ext; ;
        //            break;
        //        case FOURCC_DXT5:
        //            format = InternalFormat.CompressedRgbaS3tcDxt5Ext; ;
        //            break;
        //        default:
        //            return 0;
        //    }

        //    // Create one OpenGL texture
        //    int textureID;
        //    GL.GenTextures(1,out textureID);
        
        //    // "Bind" the newly created texture : all future texture functions will modify this texture
        //    GL.BindTexture(TextureTarget.Texture2D, textureID);
        //    GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);

        //    int blockSize = (format ==InternalFormat.CompressedRgbaS3tcDxt1Ext) ? 8 : 16;
        //    int offset = 0;

        //    /* load the mipmaps */
        //    for (int level = 0; level < mipMapCount; ++level)
        //    {
        //        int size = ((width + 3) / 4) * ((height + 3) / 4) * blockSize;
        //        unsafe
        //        {
        //            fixed (byte* p = buffer)
        //            {
        //                IntPtr ptr = (IntPtr)p;
        //                GL.CompressedTexImage2D(TextureTarget.Texture2D, level, format, width, height,
        //                    0, size, IntPtr.Add(ptr,offset));
        //            }
        //        }
        //        offset += size;
        //        width /= 2;
        //        height /= 2;

        //        // Deal with Non-Power-Of-Two textures. This code is not included in the webpage to reduce clutter.
        //        if (width < 1) width = 1;
        //        if (height < 1) height = 1;

        //    }
        //    return textureID;
        //}

    }
}

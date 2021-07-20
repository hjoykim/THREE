using System;
using System.Collections;
using System.Collections.Generic;
using OpenTK.Graphics.ES30;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using THREE.Math;
using THREE.Renderers.gl;

namespace THREE.Renderers.Shaders
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

        public readonly UniformsLib UniformsLib = new UniformsLib();


        public readonly Dictionary<string, string> ShaderChunkDictionary = new Dictionary<string, string>();
       
        //public readonly ShaderChunk ShaderChunk = new ShaderChunk();

        public ShaderLib()
        {
            // ShaderChunk folder
            var glslFiles = Directory.EnumerateFiles(@".\Renderers\Shaders\ShaderChunk", "*.glsl");
            if (glslFiles.Count() <= 0)
            {
                throw new FileNotFoundException(".glsl files not found - check the path in ShaderLib.cs, line 25");
            }

            foreach (var path in glslFiles)
            {
                this.ShaderChunkDictionary.Add(Path.GetFileNameWithoutExtension(path), path);
            }


            // ShaderLib folder
            glslFiles = Directory.EnumerateFiles(@".\Renderers\Shaders\ShaderLib", "*.glsl");
            if (glslFiles.Count() <= 0)
            {
                throw new FileNotFoundException(".glsl files not found - check the path in ShaderLib.cs, line 25");
            }

            foreach (var path in glslFiles)
            {
                this.ShaderChunkDictionary.Add(Path.GetFileNameWithoutExtension(path), path);
            }

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
            string path;
            return this.ShaderChunkDictionary.TryGetValue(chunkName, out path) ? File.ReadAllText(path) : string.Empty;
            //return this.ShaderChunk.getChunk(chunkName);
        }
        #region Basic Shader
        private gl.GLShader BasicShader()
        {
            var shader = new gl.GLShader();

            shader.Uniforms = UniformsUtils.Merge(new List<GLUniforms>
                                    {
                                        this.UniformsLib["common"],
                                        this.UniformsLib["specularmap"],
                                        this.UniformsLib["envmap"],
                                        this.UniformsLib["aomap"],
                                        this.UniformsLib["lightmap"],
                                        this.UniformsLib["fog"]
                                    });

            shader.VertexShader =  getChunk("meshbasic_vert");
            shader.FragmentShader = getChunk("meshbasic_frag");

            return shader;
        }
        #endregion

        #region Lambert Shader
        private gl.GLShader LambertShader()
        {
            var shader = new gl.GLShader();

            shader.Uniforms = UniformsUtils.Merge(new List<GLUniforms>
                                    {
                                        this.UniformsLib["common"],
                                        this.UniformsLib["specularmap"],
                                        this.UniformsLib["envmap"],
                                        this.UniformsLib["aomap"],
                                        this.UniformsLib["lightmap"],
                                        this.UniformsLib["emissivemap"],
                                        this.UniformsLib["fog"],
                                        this.UniformsLib["lights"],
                                        new GLUniforms {{"emissive",new GLUniform{{"value",Vector3.Zero()}}}}
                                    });

            shader.VertexShader = getChunk("meshlambert_vert");
            shader.FragmentShader = getChunk("meshlambert_frag");

            return shader;

        }
        #endregion Lambert Shader

        #region Phong Shader
        private gl.GLShader PhongShader()
        {
            var shader = new gl.GLShader();

            shader.Uniforms = UniformsUtils.Merge(new List<GLUniforms>
                                    {
                                        this.UniformsLib["common"],
                                        this.UniformsLib["specularmap"],
                                        this.UniformsLib["envmap"],
                                        this.UniformsLib["aomap"],
                                        this.UniformsLib["lightmap"],
                                        this.UniformsLib["emissivemap"],
                                        this.UniformsLib["bumpmap"],
                                        this.UniformsLib["normalmap"],
                                        this.UniformsLib["displacementmap"],
                                        this.UniformsLib["gradientmap"],
                                        this.UniformsLib["fog"],
                                        this.UniformsLib["lights"],
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
        private gl.GLShader StandardShader()
        {
            var shader = new gl.GLShader();

            shader.Uniforms = UniformsUtils.Merge(new List<GLUniforms>
                                    {
                                        this.UniformsLib["common"],
                                        this.UniformsLib["specularmap"],
                                        this.UniformsLib["envmap"],
                                        this.UniformsLib["aomap"],
                                        this.UniformsLib["lightmap"],
                                        this.UniformsLib["emissivemap"],
                                        this.UniformsLib["bumpmap"],
                                        this.UniformsLib["normalmap"],
                                        this.UniformsLib["displacementmap"],
                                        this.UniformsLib["roughnessmap"],
                                        this.UniformsLib["metalnessmap"],
                                        this.UniformsLib["gradientmap"],
                                        this.UniformsLib["fog"],
                                        this.UniformsLib["lights"],
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
        private gl.GLShader ToonShader()
        {
            var shader = new gl.GLShader();

            shader.Uniforms = UniformsUtils.Merge(new List<GLUniforms>
                                    {
                                        this.UniformsLib["common"],
                                        this.UniformsLib["specularmap"],                                       
                                        this.UniformsLib["aomap"],
                                        this.UniformsLib["lightmap"],
                                        this.UniformsLib["emissivemap"],
                                        this.UniformsLib["bumpmap"],
                                        this.UniformsLib["normalmap"],
                                        this.UniformsLib["displacementmap"],                                       
                                        this.UniformsLib["gradientmap"],
                                        this.UniformsLib["fog"],
                                        this.UniformsLib["lights"],
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
        private gl.GLShader MatcapShader()
        {
            var shader = new gl.GLShader();

            shader.Uniforms = UniformsUtils.Merge(new List<GLUniforms>
                                    {
                                        this.UniformsLib["common"],
                                        this.UniformsLib["bumpmap"],
                                        this.UniformsLib["normalmap"],
                                        this.UniformsLib["displacementmap"],
                                        this.UniformsLib["fog"],
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
        private gl.GLShader PointsShader()
        {
            var shader = new gl.GLShader();

            shader.Uniforms = UniformsUtils.Merge(new List<GLUniforms>
                                    {
                                        this.UniformsLib["points"],
                                        this.UniformsLib["fog"],                                       
                                    });

            shader.VertexShader = getChunk("points_vert");
            shader.FragmentShader = getChunk("points_frag");

            return shader;
        }
        #endregion

        #region Dashed Shader
        private gl.GLShader DashedShader()
        {
            var shader = new gl.GLShader();

            shader.Uniforms = UniformsUtils.Merge(new List<GLUniforms>
                                    {
                                        this.UniformsLib["common"],                                        
                                        this.UniformsLib["fog"],
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
        private gl.GLShader DepthShader()
        {
            var shader = new gl.GLShader();

            shader.Uniforms = UniformsUtils.Merge(new List<GLUniforms>
                                    {
                                        this.UniformsLib["common"],                                       
                                        this.UniformsLib["displacementmap"],                                        
                                    });

            shader.VertexShader = getChunk("depth_vert");
            shader.FragmentShader = getChunk("depth_frag");

            return shader;
        }
        #endregion

        #region Normal Shader
        private gl.GLShader NormalShader()
        {
            var shader = new gl.GLShader();

            shader.Uniforms = UniformsUtils.Merge(new List<GLUniforms>
                                    {
                                        this.UniformsLib["common"],                                       
                                        this.UniformsLib["bumpmap"],
                                        this.UniformsLib["normalmap"],
                                        this.UniformsLib["displacementmap"],                                        
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
        private gl.GLShader SpriteShader()
        {
            var shader = new gl.GLShader();

            shader.Uniforms = UniformsUtils.Merge(new List<GLUniforms>
                                    {
                                        this.UniformsLib["sprite"],
                                        this.UniformsLib["fog"],
                                    });

            shader.VertexShader = getChunk("sprite_vert");
            shader.FragmentShader = getChunk("sprite_frag");

            return shader;
        }
        #endregion

        #region Background Shader
        private gl.GLShader BackgroundShader()
        {
            var shader = new gl.GLShader();

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
        private gl.GLShader CubeShader()
        {
            var shader = new gl.GLShader();

            shader.Uniforms = UniformsUtils.Merge(new List<GLUniforms>
                                    {
                                        this.UniformsLib["envmap"],
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
        private gl.GLShader EquirectShader()
        {
            var shader = new gl.GLShader();

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
        private gl.GLShader DistanceRGBAShader()
        {
            var shader = new gl.GLShader();

            shader.Uniforms = UniformsUtils.Merge(new List<GLUniforms>
                                    {
                                        this.UniformsLib["common"],
                                        this.UniformsLib["displacementmap"],
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
        private gl.GLShader ShadowShader()
        {
            var shader = new gl.GLShader();

            shader.Uniforms = UniformsUtils.Merge(new List<GLUniforms>
                                    {
                                        this.UniformsLib["lights"],
                                        this.UniformsLib["fog"],
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
        private gl.GLShader PhysicalShader()
        {
            var shader = new gl.GLShader();

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

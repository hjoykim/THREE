using System.Collections.Generic;
using System.Drawing;

namespace THREE
{
    public class LensflareElement : GLShader
    {
        public Texture Texture;

        public float Size;

        public float Distance;

        public Color Color;

        public LensflareElement(Texture texture=null,float? size=null,float? distance=null,Color? color=null) : base() 
        {
            this.Texture = texture;

            this.Size = size != null ? (float)size : 1;

            this.Distance = distance != null ? (float)distance : 0;

            this.Color = color != null ? (Color)color : Color.Hex(0xffffff);

            this.Uniforms = new GLUniforms() 
            { 
                {"map",new GLUniform(){{"value",null}}},
                {"occlusionMap",new GLUniform(){{"value",null}}},
                {"color",new GLUniform(){{"value",null}}},
                {"scale",new GLUniform(){{"value",null}}},
                {"screenPosition",new GLUniform(){{"value",null}}}
            };

            this.VertexShader = @"                     
                     precision highp float;
		            uniform vec3 screenPosition;
		            uniform vec2 scale;
		            uniform sampler2D occlusionMap;
		            attribute vec3 position;
		            attribute vec2 uv;
		            varying vec2 vUV;
		            varying float vVisibility;
		            void main() {
		            	vUV = uv;
		            	vec2 pos = position.xy;
		            	vec4 visibility = texture2D( occlusionMap, vec2( 0.1, 0.1 ) );
		            	visibility += texture2D( occlusionMap, vec2( 0.5, 0.1 ) );
		            	visibility += texture2D( occlusionMap, vec2( 0.9, 0.1 ) );
		            	visibility += texture2D( occlusionMap, vec2( 0.9, 0.5 ) );
		            	visibility += texture2D( occlusionMap, vec2( 0.9, 0.9 ) );
		            	visibility += texture2D( occlusionMap, vec2( 0.5, 0.9 ) );
		            	visibility += texture2D( occlusionMap, vec2( 0.1, 0.9 ) );
		            	visibility += texture2D( occlusionMap, vec2( 0.1, 0.5 ) );
		            	visibility += texture2D( occlusionMap, vec2( 0.5, 0.5 ) );
		            	vVisibility =        visibility.r / 9.0;
		            	vVisibility *= 1.0 - visibility.g / 9.0;
		            	vVisibility *=       visibility.b / 9.0;
		            	gl_Position = vec4( ( pos * scale + screenPosition.xy ).xy, screenPosition.z, 1.0 );
		            }
            ";



            this.FragmentShader = @"                      
                      precision highp float;                    
		             uniform sampler2D map;
		             uniform vec3 color;
		             varying vec2 vUV;
		             varying float vVisibility;
		             void main() {
		        	    vec4 texture = texture2D( map, vUV );
		        	    texture.a *= vVisibility;
		        	    gl_FragColor = texture;
		        	    gl_FragColor.rgb *= color;
		            }

            

            
            ";
        }
    }
    public class Lensflare : Mesh
    {
        //public bool FrustumCulled = false;


        private List<LensflareElement> Elements = new List<LensflareElement>();

        private Vector3 positionScreen = new Vector3();

        private Vector3 positionView = new Vector3();

        private Vector2 scale = new Vector2();

        private Vector2 screenPositionPixels = new Vector2();

        private Box2 validArea = new Box2();

        private Vector4 viewport = new Vector4();

        RawShaderMaterial material1a, material1b, material2;

        DataTexture tempMap, occlusionMap;

        Mesh mesh1, mesh2;

        public Lensflare() :base()
        {
            this.FrustumCulled = false;

            this.RenderOrder = int.MaxValue;

            var geometry = LensflareGeometry();

            //var material = new MeshBasicMaterial() { Opacity = 0, Transparent = true };

            this.InitGeometry(geometry, new MeshBasicMaterial() { Opacity = 0, Transparent = true });

            this.type = "Lensflare";

            this.OnBeforeRender = BeforeRender;

            var positionScreen = new Vector3();

            var positionView = new Vector3();

            // textures
            tempMap = new DataTexture(new Bitmap(16, 16), 16, 16, Constants.RGBAFormat,Constants.UnsignedByteType);
            tempMap.MinFilter = Constants.NearestFilter;
            tempMap.MagFilter = Constants.NearestFilter;
            tempMap.WrapS = Constants.ClampToEdgeWrapping;
            tempMap.WrapT = Constants.ClampToEdgeWrapping;

            occlusionMap = new DataTexture(new Bitmap(16,16), 16, 16, Constants.RGBAFormat,Constants.UnsignedByteType);
            occlusionMap.MinFilter = Constants.NearestFilter;
            occlusionMap.MagFilter = Constants.NearestFilter;
            occlusionMap.WrapS = Constants.ClampToEdgeWrapping;
            occlusionMap.WrapT = Constants.ClampToEdgeWrapping;

            material1a = new RawShaderMaterial()
            {
                Uniforms = new GLUniforms(){ 
                    {"scale",new GLUniform(){{"value",null}}},
                    {"screenPosition",new GLUniform(){{"value",null}}},
                },
                VertexShader = @"
                    precision highp float;
                    uniform vec3 screenPosition;
                    uniform vec2 scale;
                    attribute vec3 position;
                    void main() {
                        gl_Position = vec4( position.xy * scale + screenPosition.xy, screenPosition.z, 1.0 );
                    }





             
                    ",
                FragmentShader = @"
                    precision highp float;
                    void main() {
                        gl_FragColor = vec4( 1.0, 0.0, 1.0, 1.0 );
                    }






                    ",
                DepthTest = true,
                DepthWrite = false,
                Transparent = false

            };

            material1b = new RawShaderMaterial()
            {
                Uniforms = new GLUniforms(){ 
                     {"map",new GLUniform(){{"value",tempMap}}},
                    {"scale",new GLUniform(){{"value",null}}},
                    {"screenPosition",new GLUniform(){{"value",null}}},
                },
                VertexShader = @"
                    precision highp float;
                    uniform vec3 screenPosition;
                    uniform vec2 scale;
                    attribute vec3 position;
                    attribute vec2 uv;
                    varying vec2 vUV;
                    void main() {
                        vUV = uv;
                        gl_Position = vec4( position.xy * scale + screenPosition.xy, screenPosition.z, 1.0 );
                    }






                
                    ",
                FragmentShader = @"
                    precision highp float;
                    uniform sampler2D map;
                    varying vec2 vUV;
                    void main() {
                        gl_FragColor = texture2D( map, vUV );
                    }







                    ",
                DepthTest = false,
                DepthWrite = false,
                Transparent = false

            };

            mesh1 = new Mesh(geometry, material1a);

            var shader = new LensflareElement();

            material2 = new RawShaderMaterial()
            {
                Uniforms = new GLUniforms() 
                { 
                    {"map", new GLUniform(){{"value",null}}},
                    {"occlusionMap",new GLUniform(){{"value",occlusionMap}}},
                    {"color",new GLUniform(){{"value",Color.Hex(0xffffff)}}},
                    {"scale",new GLUniform(){{"value",new Vector2()}}},
                    {"screenPosition",new GLUniform(){{"value",new Vector3()}}}
                },
                VertexShader = shader.VertexShader,
                FragmentShader = shader.FragmentShader,
                Blending = Constants.AdditiveBlending,
                Transparent = true,
                DepthWrite = false
            };

            mesh2 = new Mesh(geometry, material2);


        }

        private void BeforeRender(GLRenderer renderer, Scene scene, Camera camera,Geometry geometry,Material material, DrawRange? group=null,GLRenderTarget renderTarget=null)
        {
            renderer.GetCurrentViewport(viewport);

            var invAspect = viewport.W / viewport.Z;
            var halfViewportWidth = viewport.Z / 2.0f;
            var halfViewportHeight = viewport.W / 2.0f;

            var size = 16 / viewport.W;
            scale.Set(size * invAspect, size);

            validArea.Min.Set(viewport.X, viewport.Y);
            validArea.Max.Set(viewport.X + (viewport.Z - 16), viewport.Y + (viewport.W - 16));

            // calculate position in screen space

            positionView.SetFromMatrixPosition(this.MatrixWorld);
            positionView.ApplyMatrix4(camera.MatrixWorldInverse);

            if (positionView.Z > 0) return; // lensflare is behind the camera

            positionScreen.Copy(positionView).ApplyMatrix4(camera.ProjectionMatrix);

            // horizontal and vertical coordinate of the lower left corner of the pixels to copy

            screenPositionPixels.X = viewport.X + (positionScreen.X * halfViewportWidth) + halfViewportWidth - 8;
            screenPositionPixels.Y = viewport.Y + (positionScreen.Y * halfViewportHeight) + halfViewportHeight - 8;

            // screen cull

            if (validArea.ContainsPoint(screenPositionPixels))
            {

                // save current RGB to temp texture

                renderer.CopyFramebufferToTexture(screenPositionPixels, tempMap);
                
                // render pink quad

                var uniforms = material1a.Uniforms;
                (uniforms["scale"] as GLUniform)["value"] = scale;
                (uniforms["screenPosition"] as GLUniform)["value"]= positionScreen;

                renderer.RenderBufferDirect(camera, null, geometry, material1a, mesh1, null);

                // copy result to occlusionMap

                renderer.CopyFramebufferToTexture(screenPositionPixels, occlusionMap);

                // restore graphics

                uniforms = material1b.Uniforms;
                (uniforms["scale"] as GLUniform)["value"] = scale;
                (uniforms["screenPosition"] as GLUniform)["value"] = positionScreen;

                renderer.RenderBufferDirect(camera, null, geometry, material1b, mesh1, null);

                // render elements

                var vecX = -positionScreen.X * 2;
                var vecY = -positionScreen.Y * 2;

                for (int i = 0; i < Elements.Count; i++)
                {

                    var element = Elements[i];

                    uniforms = material2.Uniforms;

                    (uniforms["color"] as GLUniform)["value"] = element.Color;
                    (uniforms["map"]as GLUniform)["value"] = element.Texture;
                    Vector3 position = (Vector3)(uniforms["screenPosition"]as GLUniform)["value"];
                    position.X = positionScreen.X + vecX * element.Distance;
                    position.Y = positionScreen.Y + vecY * element.Distance;

                    size = element.Size / viewport.W;
                    invAspect = viewport.W / viewport.Z;

                    Vector2 elementScale = (Vector2)(uniforms["scale"] as GLUniform)["value"];
                    elementScale.Set(size * invAspect, size);

                    material2.UniformsNeedUpdate = true;

                    renderer.RenderBufferDirect(camera, null, geometry, material2, mesh2, null);

                }

            }
        }

        public void AddElement(LensflareElement element)
        {
            Elements.Add(element);
        }

        private BufferGeometry LensflareGeometry()
        {
            var geometry = new BufferGeometry();

            float[] floatArray = new float[]{
                -1, -1, 0, 0, 0,
		        1, -1, 0, 1, 0,
		        1, 1, 0, 1, 1,
		        -1, 1, 0, 0, 1
            };

            var interleavedBuffer = new InterleavedBuffer<float>(floatArray, 5);

            var index = new List<int>(){0,1,2,0,2,3};

            geometry.SetIndex(index);
            geometry.SetAttribute("position", new InterleavedBufferAttribute<float>(interleavedBuffer, 3, 0, false));
            geometry.SetAttribute("uv", new InterleavedBufferAttribute<float>(interleavedBuffer, 2, 3, false));


            return geometry;
        }

        public override void Dispose()
        {
            material1a.Dispose();
            material1b.Dispose();
            material2.Dispose();

            tempMap.Dispose();
            occlusionMap.Dispose();

            for (int i = 0; i < Elements.Count; i++)
            {
                Elements[i].Texture.Dispose();
            }
            base.Dispose();
        }
    }
}

using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using THREE;
using Typography.OpenFont.Tables;

namespace THREEExample
{
    [Example("Volume Cloud", ExampleCategory.ThreeJs, "Texture")]
    public class VolumeCloudExample : Example
    {
        Mesh mesh;
        public VolumeCloudExample()
        {
        }
        
        public override void InitCamera()
        {
            base.InitCamera();
            camera.Fov = 60;
            camera.Near = 0.1f;
            camera.Far = 100;
            camera.Position.Set(0, 0, 1.5f);
        }
        public SKBitmap CreateGradientTexture()
        {
            var bitmap = new SKBitmap(1, 32, SKColorType.Rgba8888, SKAlphaType.Premul);

            using (var canvas = new SKCanvas(bitmap))
            {
                var rect = new SKRect(0, 0, 1, 32);

                var colors = new SKColor[]
                {
                    SKColor.Parse("#014a84"),
                    SKColor.Parse("#0561a0"),
                    SKColor.Parse("#437ab6")
                };

                var positions = new float[] { 0.0f, 0.5f, 1.0f };

                using (var shader = SKShader.CreateLinearGradient(
                    new SKPoint(0, 0),      
                    new SKPoint(0, 32),     
                    colors,                 
                    positions,              
                    SKShaderTileMode.Clamp  
                ))
                {
                    using (var paint = new SKPaint())
                    {
                        paint.Shader = shader;
                        canvas.DrawRect(rect, paint);
                    }
                }
            }

            return bitmap;
        }
        public override void Init()
        {
            base.Init();

            // Texture, it need to sky texture but Bitmap texture could not support WSL mode
            // so, I create new texture generating function using SKBitmap, but It seem that does not working
            // if you want to get sky texture, please use other texture
     
            var sky = new THREE.Mesh(
                    new THREE.SphereGeometry(10),
                    new THREE.MeshBasicMaterial() { Map = new Texture() { Image = CreateGradientTexture() }, Side= Constants.BackSide }
				);

			scene.Add(sky );

            var size = 128;
            var data = new byte[size * size * size];

            int i = 0;
            var scale = 0.05f;
            var perlin = new ImprovedNoise();
            var vector = new THREE.Vector3();

            for (var z = 0; z < size; z++)
            {

                for (var y = 0; y < size; y++)
                {

                    for (var x = 0; x < size; x++)
                    {

                        var d = 1.0f - vector.Set(x, y, z).SubScalar(size / 2).DivideScalar(size).Length();
                        data[i] = (byte)((128 + 128 * perlin.Noise(x * scale / 1.5f, y * scale, z * scale / 1.5f)) * d * d);
                        i++;
                    }
                }

            }

            var texture = new THREE.DataTexture3D(data, size, size, size);
            texture.Format = Constants.RedFormat;
            texture.MinFilter = Constants.LinearFilter;
            texture.MagFilter = Constants.LinearFilter;
            texture.UnpackAlignment = 1;
            

            // Material

            string vertexShader = @"
            #version 400
            #define attribute in
            #define varying out
			in vec3 position;

            uniform mat4 modelMatrix;
            uniform mat4 modelViewMatrix;
            uniform mat4 projectionMatrix;
            uniform vec3 cameraPos;

			out vec3 vOrigin;
			out vec3 vDirection;

            void main()
            {
                vec4 mvPosition = modelViewMatrix * vec4(position, 1.0);

                vOrigin = vec3(inverse(modelMatrix) * vec4(cameraPos, 1.0)).xyz;
                vDirection = position - vOrigin;

                gl_Position = projectionMatrix * mvPosition;
            }
				";

            string fragmentShader = @"
            #version 400
            #define attribute out
            #define varying in
            precision highp float;
            precision highp sampler3D;

            uniform mat4 modelViewMatrix;
            uniform mat4 projectionMatrix;

					in vec3 vOrigin;
					in vec3 vDirection;

					out vec4 color;

            uniform vec3 base;
            uniform sampler3D map;

            uniform float threshold;
            uniform float range;
            uniform float opacity;
            uniform float steps;
            uniform float frame;

            uint wang_hash(uint seed)
            {
                seed = (seed ^ 61u) ^ (seed >> 16u);
                seed *= 9u;
                seed = seed ^ (seed >> 4u);
                seed *= 0x27d4eb2du;
                seed = seed ^ (seed >> 15u);
                return seed;
            }

            float randomFloat(inout uint seed)
            {
                return float(wang_hash(seed)) / 4294967296.;
            }

            vec2 hitBox(vec3 orig, vec3 dir)
            {
                const vec3 box_min = vec3(-0.5);
                const vec3 box_max = vec3(0.5);
                vec3 inv_dir = 1.0 / dir;
                vec3 tmin_tmp = (box_min - orig) * inv_dir;
                vec3 tmax_tmp = (box_max - orig) * inv_dir;
                vec3 tmin = min(tmin_tmp, tmax_tmp);
                vec3 tmax = max(tmin_tmp, tmax_tmp);
                float t0 = max(tmin.x, max(tmin.y, tmin.z));
                float t1 = min(tmax.x, min(tmax.y, tmax.z));
                return vec2(t0, t1);
            }

            float sample1(vec3 p)
            {
                return texture(map, p).r;
            }

            float shading(vec3 coord)
            {
                float step = 0.01;
                return sample1(coord + vec3(-step)) - sample1(coord + vec3(step));
            }

            void main()
            {
                vec3 rayDir = normalize(vDirection);
                vec2 bounds = hitBox(vOrigin, rayDir);

                if (bounds.x > bounds.y) discard;

                bounds.x = max(bounds.x, 0.0);

                vec3 p = vOrigin + bounds.x * rayDir;
                vec3 inc = 1.0 / abs(rayDir);
                float delta = min(inc.x, min(inc.y, inc.z));
                delta /= steps;

                // Jitter

                // Nice little seed from
                // https://blog.demofox.org/2020/05/25/casual-shadertoy-path-tracing-1-basic-camera-diffuse-emissive/
                uint seed = uint(gl_FragCoord.x) * uint(1973) + uint(gl_FragCoord.y) * uint(9277) + uint(frame) * uint(26699);
                vec3 size = vec3(textureSize(map, 0));
                float randNum = randomFloat(seed) * 2.0 - 1.0;
                p += rayDir * randNum * (1.0 / size);

                //

                vec4 ac = vec4(base, 0.0);

                for (float t = bounds.x; t < bounds.y; t += delta)
                {

                    float d = sample1(p + 0.5);

                    d = smoothstep(threshold - range, threshold + range, d) * opacity;

                    float col = shading(p + 0.5) * 3.0 + ((p.x + p.y) * 0.25) + 0.2;

                    ac.rgb += (1.0 - ac.a) * d * col;

                    ac.a += (1.0 - ac.a) * d;

                    if (ac.a >= 0.95) break;

                    p += rayDir * delta;

                }

                color = ac;

                if (color.a == 0.0) discard;

            }
				";

            var geometry = new THREE.BoxGeometry(1, 1, 1);
            GLUniforms uniforms = new GLUniforms
                    {
                        {"base", new GLUniform { { "value", new THREE.Color(0x798aa0) } } },
                        { "map", new GLUniform { { "value", texture } } },
                        { "cameraPos", new GLUniform { { "value", new THREE.Vector3() } } },
                        { "threshold", new GLUniform { { "value", 0.25f } } },
                        { "opacity", new GLUniform { { "value", 0.25f } } },
                        { "range", new GLUniform { { "value", 0.1f } } },
                        { "steps", new GLUniform { { "value", 100 } } },
                        { "frame", new GLUniform { { "value", 0 } } }
                    };
            
            var material = new THREE.RawShaderMaterial() 
                {                     
					Uniforms = new GLUniforms
                    {
                        {"base", new GLUniform { { "value", new THREE.Color(0x798aa0) } } },
                        { "map", new GLUniform { { "value", texture } } },
                        { "cameraPos", new GLUniform { { "value", new THREE.Vector3() } } },
                        { "threshold", new GLUniform { { "value", 0.25f } } },
                        { "opacity", new GLUniform { { "value", 0.25f } } },
                        { "range", new GLUniform { { "value", 0.1f } } },
                        { "steps", new GLUniform { { "value", 100 } } },
                        { "frame", new GLUniform { { "value", 0 } } }
                    },
                    VertexShader = vertexShader,
					FragmentShader = fragmentShader,                    
					Side = Constants.BackSide,
					Transparent= true

                } ;

			mesh = new THREE.Mesh(geometry, material);
			scene.Add(mesh );
        }
        public override void Render()
        {
            (((mesh.Material as RawShaderMaterial).Uniforms["cameraPos"] as GLUniform)["value"] as Vector3).Copy(camera.Position);
            mesh.Rotation.Y = - GetDelta() / 750;
            base.Render();
        }
    }
}

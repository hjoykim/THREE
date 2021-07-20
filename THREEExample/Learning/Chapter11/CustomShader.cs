using THREE.Materials;
using THREE.Renderers.gl;

namespace THREEExample.Learning.Chapter11
{
    public class CustomGrayScaleShader : ShaderMaterial
    {
        public CustomGrayScaleShader() : base()
        {
            Uniforms.Add("tDiffuse", new GLUniform { { "value", null } });
            Uniforms.Add("rPower", new GLUniform {{ "value", 0.2126f }});
            Uniforms.Add("gPower", new GLUniform { { "value", 0.7152f } });
            Uniforms.Add("bPower", new GLUniform { { "value", 0.0722f } });

            VertexShader = @"
                varying vec2 vUv; 


                void main() {

					vUv = uv;
			        gl_Position = projectionMatrix * modelViewMatrix * vec4( position, 1.0 );

		        }


            "
            ;

            FragmentShader = @"
              // pass in our custom uniforms
              uniform float rPower; 
              uniform float gPower;
              uniform float bPower;

              // pass in the image/texture we'll be modifying
              uniform sampler2D tDiffuse;

              // used to determine the correct texel we're working on
              varying vec2 vUv;

              // executed, in parallel, for each pixel
              void main() {

              // get the pixel from the texture we're working with (called a texel)
              vec4 texel = texture2D( tDiffuse, vUv );

              // calculate the new color
              float gray = texel.r*rPower + texel.g*gPower + texel.b*bPower;

              // return this new color
              gl_FragColor = vec4( vec3(gray), texel.w );

              }
            ";
        }
    }

    public class CustomBitShader : ShaderMaterial
    {
        public CustomBitShader() : base()
        {
            Uniforms.Add("tDiffuse", new GLUniform { { "value", null } });
            Uniforms.Add("bitSize", new GLUniform { { "value", 4 } });

            VertexShader = @"
                varying vec2 vUv; 


                void main() {

					vUv = uv;
			        gl_Position = projectionMatrix * modelViewMatrix * vec4( position, 1.0 );

		        }


            "
           ;

            FragmentShader = @"
              uniform int bitSize; 

              uniform sampler2D tDiffuse;

              varying vec2 vUv;

              void main() {

              vec4 texel = texture2D( tDiffuse, vUv );
              float n = pow(float(bitSize),2.0);
              float newR = floor(texel.r*n)/n;
              float newG = floor(texel.g*n)/n;
              float newB = floor(texel.b*n)/n;

              gl_FragColor = vec4( vec3(newR,newG,newB), 1.0);

              }
";
        }
    }

}

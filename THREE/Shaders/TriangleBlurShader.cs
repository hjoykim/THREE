
namespace THREE
{
    public class TriangleBlurShader : ShaderMaterial
    {
        public TriangleBlurShader() : base()
        {
            Uniforms.Add("tDiffuse", new GLUniform { { "value", null } });
            Uniforms.Add("delta", new GLUniform { { "value", new Vector2(1,1) } });
           


            VertexShader = @"
                varying vec2 vUv; 


                void main() {

					vUv = uv;
			        gl_Position = projectionMatrix * modelViewMatrix * vec4( position, 1.0 );

		        }


            "
            ;

            FragmentShader = @"
				#include <common>

				#define ITERATIONS 10.0

				uniform sampler2D tDiffuse;
				uniform vec2 delta;

				varying vec2 vUv;

				void main() {

					vec4 color = vec4( 0.0 );

					float total = 0.0;

				// randomize the lookup values to hide the fixed number of samples

					float offset = rand( vUv );

					for ( float t = -ITERATIONS; t <= ITERATIONS; t ++ ) {

						float percent = ( t + offset - 0.5 ) / ITERATIONS;
						float weight = 1.0 - abs( percent );

						color += texture2D( tDiffuse, vUv + delta * percent ) * weight;
						total += weight;

					}

					gl_FragColor = color / total;

				}


			";
        }
    }
}

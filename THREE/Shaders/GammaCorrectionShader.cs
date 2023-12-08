
namespace THREE
{
    public class GammaCorrectionShader : ShaderMaterial
    {
        public GammaCorrectionShader():base()
        {
            Uniforms.Add("tDiffuse", new GLUniform { { "value", null } });

            VertexShader = @"
                varying vec2 vUv; 


                        void main() {

			                vUv = uv;
			                gl_Position = projectionMatrix * modelViewMatrix * vec4( position, 1.0 );

		                }


                "
            ;

            FragmentShader = @"
                uniform sampler2D tDiffuse; 


                varying vec2 vUv;

		        void main() {

		        	vec4 tex = texture2D( tDiffuse, vUv );

		        	gl_FragColor = LinearTosRGB( tex ); // optional: LinearToGamma( tex, float( GAMMA_FACTOR ) );

		        }

            ";
        }
    }
}

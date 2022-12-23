
namespace THREE
{
    public class KaleidoShader : ShaderMaterial
    {
        public KaleidoShader() : base()
        {
            Uniforms.Add("tDiffuse", new GLUniform { { "value", null } });
            Uniforms.Add("sides", new GLUniform { { "value", 6.0f } });
            Uniforms.Add("angle", new GLUniform { { "value", 0.0f } });

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
				uniform float sides;
				uniform float angle;

				varying vec2 vUv;

				void main() {

					vec2 p = vUv - 0.5;
					float r = length(p);
					float a = atan(p.y, p.x) + angle;
					float tau = 2. * 3.1416 ;
					a = mod(a, tau/sides);
					a = abs(a - tau/sides/2.) ;
					p = r * vec2(cos(a), sin(a));
					vec4 color = texture2D(tDiffuse, p + 0.5);
					gl_FragColor = color;

				}


			";
        }
    }
}

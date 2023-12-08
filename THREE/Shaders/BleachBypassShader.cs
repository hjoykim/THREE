namespace THREE
{
    public class BleachBypassShader : ShaderMaterial
    {
        public BleachBypassShader()
        {
            Uniforms.Add("tDiffuse", new GLUniform { { "value", null } });
            Uniforms.Add("opacity", new GLUniform { { "value", 1.0f } });

            VertexShader = @"
                varying vec2 vUv; 


                        void main() {

			                vUv = uv;
			                gl_Position = projectionMatrix * modelViewMatrix * vec4( position, 1.0 );

		                }


                "
             ;

            FragmentShader = @"
				uniform float opacity; 

				uniform sampler2D tDiffuse;

				varying vec2 vUv;

				void main() {

					vec4 base = texture2D( tDiffuse, vUv );

					vec3 lumCoeff = vec3( 0.25, 0.65, 0.1 );
					float lum = dot( lumCoeff, base.rgb );
					vec3 blend = vec3( lum );

					float L = min( 1.0, max( 0.0, 10.0 * ( lum - 0.45 ) ) );

					vec3 result1 = 2.0 * base.rgb * blend;
					vec3 result2 = 1.0 - 2.0 * ( 1.0 - blend ) * ( 1.0 - base.rgb );

					vec3 newColor = mix( result1, result2, L );

					float A2 = opacity * base.a;
					vec3 mixRGB = A2 * newColor.rgb;
					mixRGB += ( ( 1.0 - A2 ) * base.rgb );

					gl_FragColor = vec4( mixRGB, base.a );

				}


			";
        }
    }
}

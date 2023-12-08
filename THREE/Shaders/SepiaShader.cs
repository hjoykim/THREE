namespace THREE
{
    public class SepiaShader : ShaderMaterial
    {
        public SepiaShader()
        {
            Uniforms.Add("tDiffuse", new GLUniform { { "value", null } });
            Uniforms.Add("amount", new GLUniform { { "value", 1.0f } });

            VertexShader = @"
                varying vec2 vUv; 


                        void main() {

			                vUv = uv;
			                gl_Position = projectionMatrix * modelViewMatrix * vec4( position, 1.0 );

		                }


                "
             ;
            
            FragmentShader = @"
            uniform float amount; 


            uniform sampler2D tDiffuse;

		    varying vec2 vUv;

		    void main() {

			    vec4 color = texture2D( tDiffuse, vUv );
			    vec3 c = color.rgb;

			    color.r = dot( c, vec3( 1.0 - 0.607 * amount, 0.769 * amount, 0.189 * amount ) );
			    color.g = dot( c, vec3( 0.349 * amount, 1.0 - 0.314 * amount, 0.168 * amount ) );
			    color.b = dot( c, vec3( 0.272 * amount, 0.534 * amount, 1.0 - 0.869 * amount ) );

			    gl_FragColor = vec4( min( vec3( 1.0 ), color.rgb ), color.a );

		    }
            "
            ;
        }
    }
}

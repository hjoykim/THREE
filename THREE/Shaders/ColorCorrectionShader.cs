
namespace THREE
{
    public class ColorCorrectionShader : ShaderMaterial
    {
        public ColorCorrectionShader():base()
        {
            Uniforms.Add("tDiffuse", new GLUniform { { "value", null } });
            Uniforms.Add("powRGB", new GLUniform { { "value", new Vector3(2, 2, 2) } });
            Uniforms.Add("mulRGB", new GLUniform { { "value", new Vector3(1, 1, 1) } });
            Uniforms.Add("addRGB", new GLUniform { { "value", new Vector3(0, 0, 0) } });

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

                uniform vec3 powRGB;
		        uniform vec3 mulRGB;
		        uniform vec3 addRGB;

		        varying vec2 vUv;

		        void main() {

		        	gl_FragColor = texture2D( tDiffuse, vUv );
		        	gl_FragColor.rgb = mulRGB * pow( ( gl_FragColor.rgb + addRGB ), powRGB );

		        }
            ";
        }
    }
}

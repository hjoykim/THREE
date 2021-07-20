using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using THREE.Materials;
using THREE.Renderers.gl;

namespace THREE.Shaders
{
    public class BrightnessContrastShader : ShaderMaterial
    {
        public BrightnessContrastShader() : base()
        {
            Uniforms.Add("tDiffuse", new GLUniform { { "value", null } });
            Uniforms.Add("brightness", new GLUniform { { "value", 0.0f } });
            Uniforms.Add("contrast", new GLUniform { { "value", 0.0f } });

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

                uniform float brightness;
		        uniform float contrast;

		        varying vec2 vUv;

		        void main() {

		        	gl_FragColor = texture2D( tDiffuse, vUv );

		        	gl_FragColor.rgb += brightness;

		        	if (contrast > 0.0) {
		        		gl_FragColor.rgb = (gl_FragColor.rgb - 0.5) / (1.0 - contrast) + 0.5;
		        	} else {
		        		gl_FragColor.rgb = (gl_FragColor.rgb - 0.5) * (1.0 + contrast) + 0.5;
		        	}

		        }

            ";
        }
    }
}

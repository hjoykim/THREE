﻿using System.Runtime.Serialization;

namespace THREE
{
	[Serializable]
    public class HueSaturationShader : ShaderMaterial
    {
        public HueSaturationShader() : base()
        {
            Uniforms.Add("tDiffuse", new GLUniform { { "value", null } });
            Uniforms.Add("hue", new GLUniform { { "value", 0.0f } });
            Uniforms.Add("saturation", new GLUniform { { "value", 0.0f } });

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
			uniform float hue;
			uniform float saturation;

			varying vec2 vUv;

			void main() {

				gl_FragColor = texture2D( tDiffuse, vUv );

			// hue
				float angle = hue * 3.14159265;
				float s = sin(angle), c = cos(angle);
				vec3 weights = (vec3(2.0 * c, -sqrt(3.0) * s - c, sqrt(3.0) * s - c) + 1.0) / 3.0;
				float len = length(gl_FragColor.rgb);
				gl_FragColor.rgb = vec3(
					dot(gl_FragColor.rgb, weights.xyz),
					dot(gl_FragColor.rgb, weights.zxy),
					dot(gl_FragColor.rgb, weights.yzx)
				);

			// saturation
				float average = (gl_FragColor.r + gl_FragColor.g + gl_FragColor.b) / 3.0;
				if (saturation > 0.0) {
					gl_FragColor.rgb += (average - gl_FragColor.rgb) * (1.0 - 1.0 / (1.001 - saturation));
				} else {
					gl_FragColor.rgb += (average - gl_FragColor.rgb) * (-saturation);
				}

			}


			";
        }

        public HueSaturationShader(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}

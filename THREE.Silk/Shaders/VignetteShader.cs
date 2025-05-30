﻿using System.Runtime.Serialization;

namespace THREE
{
	[Serializable]
    public class VignetteShader : ShaderMaterial
    {
        public VignetteShader() : base()
        {
            Uniforms.Add("tDiffuse", new GLUniform { { "value", null } });
            Uniforms.Add("offset", new GLUniform { { "value", 1.0f } });
            Uniforms.Add("darkness", new GLUniform { { "value", 1.0f } });


            VertexShader = @"
                varying vec2 vUv; 


                void main() {

					vUv = uv;
			        gl_Position = projectionMatrix * modelViewMatrix * vec4( position, 1.0 );

		        }


            "
            ;

            FragmentShader = @"
				uniform float offset; 
				uniform float darkness;

				uniform sampler2D tDiffuse;

				varying vec2 vUv;

				void main() {

				// Eskil's vignette

					vec4 texel = texture2D( tDiffuse, vUv );
					vec2 uv = ( vUv - vec2( 0.5 ) ) * vec2( offset );
					gl_FragColor = vec4( mix( texel.rgb, vec3( 1.0 - darkness ), dot( uv, uv ) ), texel.a );

				/*
				// alternative version from glfx.js
				// this one makes more dusty look (as opposed to burned)

					vec4 color = texture2D( tDiffuse, vUv );
					float dist = distance( vUv, vec2( 0.5 ) );
					color.rgb *= smoothstep( 0.8, offset * 0.799, dist *( darkness + offset ) );
					gl_FragColor = color;
				*/

				}


			";
        }

        public VignetteShader(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}

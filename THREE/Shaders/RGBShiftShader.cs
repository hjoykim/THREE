namespace THREE
{
    public class RGBShiftShader : ShaderMaterial
    {
        public RGBShiftShader() : base()
        {
            Uniforms.Add("tDiffuse", new GLUniform { { "value", null } });
            Uniforms.Add("amount", new GLUniform { { "value", 0.005f } });
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
				uniform float amount;
				uniform float angle;

				varying vec2 vUv;

				void main() {

					vec2 offset = amount * vec2( cos(angle), sin(angle));
					vec4 cr = texture2D(tDiffuse, vUv + offset);
					vec4 cga = texture2D(tDiffuse, vUv);
					vec4 cb = texture2D(tDiffuse, vUv - offset);
					gl_FragColor = vec4(cr.r, cga.g, cb.b, cga.a);

				}

			";
        }
    }
}

using System.Runtime.Serialization;

namespace THREE
{
    [Serializable]
    public class CopyShader : ShaderMaterial
    {
        public CopyShader()
        {
            Uniforms.Add("tDiffuse", new Uniform { { "value", null } });
            Uniforms.Add("opacity", new Uniform { { "value", 1.0f } });

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

			        vec4 texel = texture2D( tDiffuse, vUv );
			        gl_FragColor = opacity * texel;

		        }"
            ;
        }

        public CopyShader(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}

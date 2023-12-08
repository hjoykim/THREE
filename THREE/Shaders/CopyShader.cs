namespace THREE
{
    public class CopyShader : ShaderMaterial
    {
        public CopyShader()
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

			        vec4 texel = texture2D( tDiffuse, vUv );
			        gl_FragColor = opacity * texel;

		        }"
            ;
        }
    }
}

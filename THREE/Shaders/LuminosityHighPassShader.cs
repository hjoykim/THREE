
namespace THREE
{
    public class LuminosityHighPassShader : ShaderMaterial
    {
        public LuminosityHighPassShader()
        {
            Uniforms.Add("tDiffuse", new GLUniform { { "value", null } });
            Uniforms.Add("luminosityThreshold", new GLUniform { { "value", 1.0f } });
            Uniforms.Add("smoothWidth", new GLUniform { { "value", 1.0f } });
            Uniforms.Add("defaultColor", new GLUniform { { "value", new Color(0x000000) } });
            Uniforms.Add("defaultOpacity", new GLUniform { { "value", 0.0f } });

            VertexShader = @"
varying vec2 vUv;


void main() {

    vUv = uv;

	gl_Position = projectionMatrix * modelViewMatrix * vec4( position, 1.0 );

}

";

            FragmentShader = @"
uniform sampler2D tDiffuse; 
uniform vec3 defaultColor;
uniform float defaultOpacity;
uniform float luminosityThreshold;
uniform float smoothWidth;

varying vec2 vUv;

void main() {

	vec4 texel = texture2D( tDiffuse, vUv );

	vec3 luma = vec3( 0.299, 0.587, 0.114 );

	float v = dot( texel.xyz, luma );

	vec4 outputColor = vec4( defaultColor.rgb, defaultOpacity );

	float alpha = smoothstep( luminosityThreshold, luminosityThreshold + smoothWidth, v );

	gl_FragColor = mix( outputColor, texel, alpha );

}

";
           
        }
    }
}

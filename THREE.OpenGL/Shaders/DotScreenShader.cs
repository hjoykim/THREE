﻿
using System.Runtime.Serialization;

namespace THREE
{
    [Serializable]
    public class DotScreenShader : ShaderMaterial
    {
        public DotScreenShader()
        {
            Uniforms.Add("tDiffuse", new GLUniform { { "value", null } });
            Uniforms.Add("tSize", new GLUniform { { "value", new Vector2(256, 256) } });
            Uniforms.Add("center", new GLUniform { { "value", new Vector2(0.5f, 0.5f) } });
            Uniforms.Add("angle", new GLUniform { { "value", 1.57f } });
            Uniforms.Add("scale", new GLUniform { { "value", 1.0f } });

            VertexShader = @"
varying vec2 vUv; 

void main() {

    vUv = uv;
	gl_Position = projectionMatrix * modelViewMatrix * vec4( position, 1.0 );

}



"
;

            FragmentShader = @"
uniform vec2 center;
uniform float angle;
uniform float scale;
uniform vec2 tSize;

uniform sampler2D tDiffuse;

varying vec2 vUv;

float pattern() {

	float s = sin( angle ), c = cos( angle );

	vec2 tex = vUv * tSize - center;
	vec2 point = vec2( c * tex.x - s * tex.y, s * tex.x + c * tex.y ) * scale;

	return ( sin( point.x ) * sin( point.y ) ) * 4.0;

}

void main() {

	vec4 color = texture2D( tDiffuse, vUv );

	float average = ( color.r + color.g + color.b ) / 3.0;

	gl_FragColor = vec4( vec3( average * 10.0 - 5.0 + pattern() ), color.a );

}

"
;
        }

        public DotScreenShader(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}

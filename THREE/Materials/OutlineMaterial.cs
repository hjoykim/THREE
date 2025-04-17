using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace THREE
{
    [Serializable]
    public class OutlineMaterial : ShaderMaterial
    {
        private float angleThreshold;
        public float AngleThreshold 
        {
            get { return angleThreshold; } 
            set
            {
                angleThreshold = value;
                (Uniforms["uAngleThresh"] as GLUniform)["value"] = ((value / 180) * Math.PI) % Math.PI;
            } 
        }
        public bool Outline 
        {
            get { return (bool)(Uniforms["uOutline"] as GLUniform)["value"]; }
            set
            {
                (Uniforms["uOutline"] as GLUniform)["value"] = value;
            }
        }
        public new Color Color 
        {
            get { return (Color)(Uniforms["uColor"] as GLUniform)["value"]; }
            set
            {
                (Uniforms["uColor"] as GLUniform)["value"] = value;
            }
        }
        private string vertexShader = @"
attribute vec3 aN0;
attribute vec3 aN1;
attribute vec4 aOtherVert;
uniform float uAngleThresh;
uniform int uOutline;

varying vec4 vDebug;

void main() {
	vec4 mv = modelViewMatrix * vec4( position , 1. );
	
	gl_Position = projectionMatrix * mv;
	gl_Position.z -= 0.001; //wtf?

	vec3 viewDir = normalize(-mv.xyz);
	
	//check for null normals
	float l0 = length(aN0); 
	float l1 = length(aN1);

	if(l0*l1 < 0.0001) return; //end of mesh

	float dd = dot(normalize(aN0),normalize(aN1));

	if(acos(dd) > uAngleThresh) return;
	
	//find point
	bool isOther = aOtherVert.w > 0.5;
	vec3 a;
	vec3 b;

	if(isOther){
		a = aOtherVert.xyz;
		b = position;
	} else {
		a = position;
		b = aOtherVert.xyz;
	}

	vec3 ld = normalize(b - a);
	vec3 camToEdge = cameraPosition - a;
	float f = dot(camToEdge,ld);
	vec3 p = a + ld * f;
	p = normalize(p-cameraPosition);

	vec3 vp =  ( modelViewMatrix * vec4(p,   0.) ).xyz;
	vec3 vN0 = ( modelViewMatrix * vec4(aN0, 0.) ).xyz;
	vec3 vN1 = ( modelViewMatrix * vec4(aN1, 0.) ).xyz;

	vDebug.x = sign(dot(vN0,viewDir));
	vDebug.y = sign(dot(vN1,viewDir));

	bool outline = sign(dot(vN0,viewDir)) * sign(dot(vN1,viewDir)) < 0.;
	
	if(uOutline == 1 && outline) return;

	gl_Position.z = -2000.; //wtf?
}
";
        private string fragmentShader = @"
            uniform vec3 uColor;
            varying vec4 vDebug;

            void main(){
              gl_FragColor = vec4(uColor,1.);
            }
            ";
        public OutlineMaterial(float angleThreshold=0,bool outline=true, Color? color = null,Hashtable parameter = null) : base(parameter)
        {
            Uniforms = new GLUniforms
            {
                { "uAngleThresh", new GLUniform { {"value", 0 } } },
                { "uOutline", new GLUniform{{ "value", true } } },
                { "uColor", new GLUniform { {"value", new Color() } } }
            };
            VertexShader = vertexShader;
            FragmentShader = fragmentShader;
            AngleThreshold = angleThreshold;
            Outline = outline;
            if (color != null) Color = color.Value;
            else Color = Color.Hex(0xffffff);
        }
    }
}

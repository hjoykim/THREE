using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using THREE;
namespace THREE
{
    [Serializable]
    public class LineMaterial : ShaderMaterial
    {
        string Vertex = @"
		#include <common>
		#include <color_pars_vertex>
		#include <fog_pars_vertex>
		#include <logdepthbuf_pars_vertex>
		#include <clipping_planes_pars_vertex>

		uniform float linewidth;
		uniform vec2 resolution;

		attribute vec3 instanceStart;
		attribute vec3 instanceEnd;

		attribute vec3 instanceColorStart;
		attribute vec3 instanceColorEnd;

		varying vec2 vUv;

		#ifdef USE_DASH

			uniform float dashScale;
			attribute float instanceDistanceStart;
			attribute float instanceDistanceEnd;
			varying float vLineDistance;

		#endif

		void trimSegment( const in vec4 start, inout vec4 end ) {

			// trim end segment so it terminates between the camera plane and the near plane

			// conservative estimate of the near plane
			float a = projectionMatrix[ 2 ][ 2 ]; // 3nd entry in 3th column
			float b = projectionMatrix[ 3 ][ 2 ]; // 3nd entry in 4th column
			float nearEstimate = - 0.5 * b / a;

			float alpha = ( nearEstimate - start.z ) / ( end.z - start.z );

			end.xyz = mix( start.xyz, end.xyz, alpha );

		}

		void main() {

			#ifdef USE_COLOR

				vColor.xyz = ( position.y < 0.5 ) ? instanceColorStart : instanceColorEnd;

			#endif

			#ifdef USE_DASH

				vLineDistance = ( position.y < 0.5 ) ? dashScale * instanceDistanceStart : dashScale * instanceDistanceEnd;

			#endif

			float aspect = resolution.x / resolution.y;

			vUv = uv;

			// camera space
			vec4 start = modelViewMatrix * vec4( instanceStart, 1.0 );
			vec4 end = modelViewMatrix * vec4( instanceEnd, 1.0 );

			// special case for perspective projection, and segments that terminate either in, or behind, the camera plane
			// clearly the gpu firmware has a way of addressing this issue when projecting into ndc space
			// but we need to perform ndc-space calculations in the shader, so we must address this issue directly
			// perhaps there is a more elegant solution -- WestLangley

			bool perspective = ( projectionMatrix[ 2 ][ 3 ] == - 1.0 ); // 4th entry in the 3rd column

			if ( perspective ) {

				if ( start.z < 0.0 && end.z >= 0.0 ) {

					trimSegment( start, end );

				} else if ( end.z < 0.0 && start.z >= 0.0 ) {

					trimSegment( end, start );

				}

			}

			// clip space
			vec4 clipStart = projectionMatrix * start;
			vec4 clipEnd = projectionMatrix * end;

			// ndc space
			vec2 ndcStart = clipStart.xy / clipStart.w;
			vec2 ndcEnd = clipEnd.xy / clipEnd.w;

			// direction
			vec2 dir = ndcEnd - ndcStart;

			// account for clip-space aspect ratio
			dir.x *= aspect;
			dir = normalize( dir );

			// perpendicular to dir
			vec2 offset = vec2( dir.y, - dir.x );

			// undo aspect ratio adjustment
			dir.x /= aspect;
			offset.x /= aspect;

			// sign flip
			if ( position.x < 0.0 ) offset *= - 1.0;

			// endcaps
			if ( position.y < 0.0 ) {

				offset += - dir;

			} else if ( position.y > 1.0 ) {

				offset += dir;

			}

			// adjust for linewidth
			offset *= linewidth;

			// adjust for clip-space to screen-space conversion // maybe resolution should be based on viewport ...
			offset /= resolution.y;

			// select end
			vec4 clip = ( position.y < 0.5 ) ? clipStart : clipEnd;

			// back to clip space
			offset *= clip.w;

			clip.xy += offset;

			gl_Position = clip;

			vec4 mvPosition = ( position.y < 0.5 ) ? start : end; // this is an approximation

			#include <logdepthbuf_vertex>
			#include <clipping_planes_vertex>
			#include <fog_vertex>

		}
";
		string Fragment = @"
uniform vec3 diffuse;
		uniform float opacity;

		#ifdef USE_DASH

			uniform float dashSize;
			uniform float dashOffset;
			uniform float gapSize;

		#endif

		varying float vLineDistance;

		#include <common>
		#include <color_pars_fragment>
		#include <fog_pars_fragment>
		#include <logdepthbuf_pars_fragment>
		#include <clipping_planes_pars_fragment>

		varying vec2 vUv;

		void main() {

			#include <clipping_planes_fragment>

			#ifdef USE_DASH

				if ( vUv.y < - 1.0 || vUv.y > 1.0 ) discard; // discard endcaps

				if ( mod( vLineDistance + dashOffset, dashSize + gapSize ) > dashSize ) discard; // todo - FIX

			#endif

			float alpha = opacity;

			#ifdef ALPHA_TO_COVERAGE

			// artifacts appear on some hardware if a derivative is taken within a conditional
			float a = vUv.x;
			float b = ( vUv.y > 0.0 ) ? vUv.y - 1.0 : vUv.y + 1.0;
			float len2 = a * a + b * b;
			float dlen = fwidth( len2 );

			if ( abs( vUv.y ) > 1.0 ) {

				alpha = 1.0 - smoothstep( 1.0 - dlen, 1.0 + dlen, len2 );

			}

			#else

			if ( abs( vUv.y ) > 1.0 ) {

				float a = vUv.x;
				float b = ( vUv.y > 0.0 ) ? vUv.y - 1.0 : vUv.y + 1.0;
				float len2 = a * a + b * b;

				if ( len2 > 1.0 ) discard;

			}

			#endif

			vec4 diffuseColor = vec4( diffuse, alpha );

			#include <logdepthbuf_fragment>
			#include <color_fragment>

			gl_FragColor = vec4( diffuseColor.rgb, alpha );

			#include <tonemapping_fragment>
			#include <encodings_fragment>
			#include <fog_fragment>
			#include <premultiplied_alpha_fragment>

		}
";
		public new Color? color 
		{
			get {
				return (Color)(this.Uniforms["diffuse"] as GLUniform)["value"];
			}
			set
			{
				(this.Uniforms["diffuse"] as GLUniform)["value"] = value;
			}
		}
        public new float LineWidth
        {
            get
            {
                return (float)(this.Uniforms["linewidth"] as GLUniform)["value"];
            }
            set
            {
                (this.Uniforms["linewidth"] as GLUniform)["value"] = value;
            }
        }
        public bool Dashed
        {
            get
            {
                return  this.Defines.ContainsKey("USE_DASH");
            }
            set
            {
				if(this.Defines.ContainsKey("USE_DASH") != value)
				{
					this.NeedsUpdate = true;
				}
				if (value == true)
					this.Defines["USE_DASH"] = "";
				else
					this.Defines.Remove("USE_DASH");
                
            }
        }
        public float DashScale
        {
            get
            {
                return (float)(this.Uniforms["dashScale"] as GLUniform)["value"];
            }
            set
            {
                (this.Uniforms["dashScale"] as GLUniform)["value"] = value;
            }
        }
        public float DashSize
        {
            get
            {
                return (float)(this.Uniforms["dashSize"] as GLUniform)["value"];
            }
            set
            {
                (this.Uniforms["dashSize"] as GLUniform)["value"] = value;
            }
        }
        public float DashOffset
        {
            get
            {
                return (float)(this.Uniforms["dashOffset"] as GLUniform)["value"];
            }
            set
            {
                (this.Uniforms["dashOffset"] as GLUniform)["value"] = value;
            }
        }
        public float GapSize
        {
            get
            {
                return (float)(this.Uniforms["gapSize"] as GLUniform)["value"];
            }
            set
            {
                (this.Uniforms["gapSize"] as GLUniform)["value"] = value;
            }
        }
        public new float Opacity
        {
            get
            {
                return (float)(this.Uniforms["opacity"] as GLUniform)["value"];
            }
            set
            {
                (this.Uniforms["opacity"] as GLUniform)["value"] = value;
            }
        }
        public Vector2 Resolution
        {
            get
            {
                return (Vector2)(this.Uniforms["resolution"] as GLUniform)["value"];
            }
            set
            {
                (this.Uniforms["resolution"] as GLUniform)["value"] = value;
            }
        }

        public bool AlphaToCoverage
        {
            get
            {
                return this.Defines.ContainsKey("ALPHA_TO_COVERAGE");
            }
            set
            {
                if (this.Defines.ContainsKey("ALPHA_TO_COVERAGE") != value)
                {
                    this.NeedsUpdate = true;
                }
				if (value == true)
				{
					this.Defines["ALPHA_TO_COVERAGE"] = "";
					this.extensions.derivatives = true;
				}
				else
				{
					this.Defines.Remove("ALPHA_TO_COVERAGE");
					this.extensions.derivatives = false;
				}

            }
        }
        public LineMaterial(Hashtable parameters=null) : base(parameters) 
        {
            type = "LineMaterial";

            GLUniforms lineUniforms = new GLUniforms
            {
                {"linewidth",new GLUniform{{"value",1}}},
                {"resolution",new GLUniform{{"value",new Vector2(1,1)}}},
                {"dashed",new GLUniform{ { "value", false } } },
                {"dashScale",new GLUniform{{"value",1}} },
                {"dashSize",new GLUniform{{"value",1}} },
                {"dashOffset",new GLUniform{{"value",0}} },
                {"gapSize",new GLUniform{{"value",1}} },
                {"opacity",new GLUniform{{"value",1}} }
            };

			GLShader lineShader = new GLShader();
			lineShader.Uniforms = UniformsUtils.Merge(new List<GLUniforms> 
			{ 
				Global.ShaderLib.UniformsLib["common"], 
				Global.ShaderLib.UniformsLib["fog"], 
				lineUniforms 
			});

			Uniforms = UniformsUtils.CloneUniforms(lineShader.Uniforms);
			this.VertexShader = Vertex;
			this.FragmentShader = Fragment;							
			Clipping = true;
                
        }
    }
}

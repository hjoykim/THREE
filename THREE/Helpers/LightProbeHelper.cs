
namespace THREE
{
    public class LightProbeHelper : Mesh
    {
        LightProbe lightProbe;
        float size;
               

        public LightProbeHelper(LightProbe lightProbe,float size) 
        {
            this.lightProbe = lightProbe;
            this.size = size;

            this.Material = new ShaderMaterial()
            {
                type="LightProbeHelperMaterial",
                Name="LightProbeHelperMaterial",
                Uniforms = new GLUniforms()
                {
                    {"sh",new GLUniform(){{"value", lightProbe.sh.Coefficients}} },
                    {"intensity",new GLUniform(){{"value",lightProbe.Intensity}} }
                },
                VertexShader= @"

				varying vec3 vNormal;

				void main() {

					vNormal = normalize( normalMatrix * normal );

					gl_Position = projectionMatrix * modelViewMatrix * vec4( position, 1.0 );

				}

				",
                FragmentShader= @"
				#define RECIPROCAL_PI 0.318309886

				vec3 inverseTransformDirection( in vec3 normal, in mat4 matrix ) {

					// matrix is assumed to be orthogonal

					return normalize( ( vec4( normal, 0.0 ) * matrix ).xyz );

				}

				// source: https://graphics.stanford.edu/papers/envmap/envmap.pdf
				vec3 shGetIrradianceAt( in vec3 normal, in vec3 shCoefficients[ 9 ] ) {

					// normal is assumed to have unit length

					float x = normal.x, y = normal.y, z = normal.z;

					// band 0
					vec3 result = shCoefficients[ 0 ] * 0.886227;

					// band 1
					result += shCoefficients[ 1 ] * 2.0 * 0.511664 * y;
					result += shCoefficients[ 2 ] * 2.0 * 0.511664 * z;
					result += shCoefficients[ 3 ] * 2.0 * 0.511664 * x;

					// band 2
					result += shCoefficients[ 4 ] * 2.0 * 0.429043 * x * y;
					result += shCoefficients[ 5 ] * 2.0 * 0.429043 * y * z;
					result += shCoefficients[ 6 ] * ( 0.743125 * z * z - 0.247708 );
					result += shCoefficients[ 7 ] * 2.0 * 0.429043 * x * z;
					result += shCoefficients[ 8 ] * 0.429043 * ( x * x - y * y );

					return result;

				}

				uniform vec3 sh[ 9 ]; // sh coefficients

				uniform float intensity; // light probe intensity

				varying vec3 vNormal;

				void main() {

					vec3 normal = normalize( vNormal );

					vec3 worldNormal = inverseTransformDirection( normal, viewMatrix );

					vec3 irradiance = shGetIrradianceAt( worldNormal, sh );

					vec3 outgoingLight = RECIPROCAL_PI * irradiance * intensity;

					gl_FragColor = linearToOutputTexel( vec4( outgoingLight, 1.0 ) );

				}
			"
			};

			this.Geometry = new SphereBufferGeometry(1, 32, 16);

			this.type = "LightProbeHelper";

			this.OnBeforeRender = BeforeRender;

			this.Materials.Clear();
			this.Materials.Add(Material);
        }
		//public Action<GLRenderer, Scene, Camera, Geometry,Material,DrawRange?,GLRenderTarget> OnBeforeRender;
		private void BeforeRender(GLRenderer renderer,Scene scene,Camera camera,Geometry geometry,Material material,DrawRange? drawRange,GLRenderTarget renderTarget)
        {
			this.Position.Copy(this.lightProbe.Position);

			this.Scale.Set(1, 1, 1).MultiplyScalar(this.size);

			((this.Material as ShaderMaterial).Uniforms["intensity"] as GLUniform)["value"] = this.lightProbe.Intensity;
		}
        public override void Dispose()
        {
            base.Dispose();
			this.Geometry.Dispose();
			this.Material.Dispose();
        }
    }
}

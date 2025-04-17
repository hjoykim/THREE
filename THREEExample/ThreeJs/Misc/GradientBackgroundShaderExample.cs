using THREE;

namespace THREEExample.ThreeJs.Loader
{
    [Example("GradientBackgroundShader", ExampleCategory.ThreeJs, "Misc")]
    public class GradientBackgroundShaderExample : LightHintExample
    {
        string vertex = @"
            varying vec2 vUv;
			void main()	{
				vUv = uv;
				gl_Position = vec4( position, 1.0 );
			} 
";
        string fragment = @"
            uniform vec3 top_color;
            uniform vec3 bot_color;
            varying vec2 vUv;

			void main()	{
				gl_FragColor = vec4(bot_color * (1.0f - vUv.y) + top_color * vUv.y,1.0);
			}
";
        THREE.Color topColor;
        THREE.Color bottomColor;
        Camera backgroundCamera;
        Scene backgroundScene;
        int background_vao;
        int program;
        public GradientBackgroundShaderExample() 
        {
            backgroundCamera = new OrthographicCamera();
            backgroundScene = new Scene();
        }
        public override void Init()
        {
            base.Init();
            topColor = THREE.Color.Hex(0x57606F);
            bottomColor = THREE.Color.Hex(0xD3D3D3);

            GLUniforms uniforms = new GLUniforms
            {
                {"top_color", new GLUniform{{"value",topColor}}},
                {"bot_color",new GLUniform{{"value",bottomColor}}}
            };

            var material = new ShaderMaterial()
            {
                Uniforms = uniforms,
                VertexShader = vertex,
                FragmentShader = fragment,
                DepthTest = false,
                DepthWrite = false
            };
            var geometry = new PlaneBufferGeometry(2, 2);
            var mesh = new Mesh(geometry, material);
            backgroundScene.Add(backgroundCamera);
            backgroundScene.Add(mesh);

        }

        public override void Render()
        {
            renderer.AutoClear = false;
            renderer.Clear();
            renderer.Render(backgroundScene, backgroundCamera);
            base.Render();          
        }
    }
}

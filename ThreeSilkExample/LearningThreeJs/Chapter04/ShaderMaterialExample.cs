﻿
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using THREE;
using THREE.Silk;
using THREE.Silk.Example.Learning.Utils;
namespace THREE.Silk.Example
{
    [Example("11.ShaderMaterial", ExampleCategory.LearnThreeJS, "Chapter04")]
    public class ShaderMaterialExample : MaterialExampleTemplate
    {
        THREE.Vector2 Resolution = new THREE.Vector2();
        private string vertexShaderCode = @"
        uniform float time;
        varying vec2 vUv;
  
  
        void main()
        {
        vec3 posChanged = position;
        posChanged.x = posChanged.x*(abs(sin(time*1.0)));
        posChanged.y = posChanged.y*(abs(cos(time*1.0)));
        posChanged.z = posChanged.z*(abs(sin(time*1.0)));
        //gl_Position = projectionMatrix * modelViewMatrix * vec4(position*(abs(sin(time)/2.0)+0.5),1.0);
        gl_Position = projectionMatrix * modelViewMatrix * vec4(posChanged,1.0);
        }




        ";

        private string fragmentShader1 = @"
        precision highp float;
        uniform float time;
        uniform float alpha;
        uniform vec2 resolution;
        varying vec2 vUv;
  
        void main2(void)
        {
        vec2 position = vUv;
        float red = 1.0;
        float green = 0.25 + sin(time) * 0.25;
        float blue = 0.0;
        vec3 rgb = vec3(red, green, blue);
        vec4 color = vec4(rgb, alpha);
        gl_FragColor = color;
        }
  
        #define PI 3.14159
        #define TWO_PI (PI*2.0)
        #define N 68.5
  
        void main(void)
        {
        vec2 center = (gl_FragCoord.xy);
        center.x=-10.12*sin(time/200.0);
        center.y=-10.12*cos(time/200.0);
  
        vec2 v = (gl_FragCoord.xy - resolution/20.0) / min(resolution.y,resolution.x) * 15.0;
        v.x=v.x-10.0;
        v.y=v.y-200.0;
        float col = 0.0;
  
        for(float i = 0.0; i < N; i++)
        {
        float a = i * (TWO_PI/N) * 61.95;
        col += cos(TWO_PI*(v.y * cos(a) + v.x * sin(a) + sin(time*0.004)*100.0 ));
        }
  
        col /= 5.0;
  
        gl_FragColor = vec4(col*1.0, -col*1.0,-col*4.0, 1.0);
        }




        ";

        string fragmentShader2 = @"
        uniform float time;
        uniform vec2 resolution;
  
        // 2013-03-30 by @hintz
  
        #define CGFloat float
        #define M_PI 3.14159265359
  
        vec3 hsvtorgb(float h, float s, float v)
        {
        float c = v * s;
        h = mod((h * 6.0), 6.0);
        float x = c * (1.0 - abs(mod(h, 2.0) - 1.0));
        vec3 color;
  
        if (0.0 <= h && h < 1.0)
        {
        color = vec3(c, x, 0.0);
        }
        else if (1.0 <= h && h < 2.0)
        {
        color = vec3(x, c, 0.0);
        }
        else if (2.0 <= h && h < 3.0)
        {
        color = vec3(0.0, c, x);
        }
        else if (3.0 <= h && h < 4.0)
        {
        color = vec3(0.0, x, c);
        }
        else if (4.0 <= h && h < 5.0)
        {
        color = vec3(x, 0.0, c);
        }
        else if (5.0 <= h && h < 6.0)
        {
        color = vec3(c, 0.0, x);
        }
        else
        {
        color = vec3(0.0);
        }
  
        color += v - c;
  
        return color;
        }
  
        void main(void)
        {
  
        vec2 position = (gl_FragCoord.xy - 0.5 * resolution) / resolution.y;
        float x = position.x;
        float y = position.y;
  
        CGFloat a = atan(x, y);
  
        CGFloat d = sqrt(x*x+y*y);
        CGFloat d0 = 0.5*(sin(d-time)+1.5)*d;
        CGFloat d1 = 5.0;
  
        CGFloat u = mod(a*d1+sin(d*10.0+time), M_PI*2.0)/M_PI*0.5 - 0.5;
        CGFloat v = mod(pow(d0*4.0, 0.75),1.0) - 0.5;
  
        CGFloat dd = sqrt(u*u+v*v);
  
        CGFloat aa = atan(u, v);
  
        CGFloat uu = mod(aa*3.0+3.0*cos(dd*30.0-time), M_PI*2.0)/M_PI*0.5 - 0.5;
        // CGFloat vv = mod(dd*4.0,1.0) - 0.5;
  
        CGFloat d2 = sqrt(uu*uu+v*v)*1.5;
  
        gl_FragColor = vec4( hsvtorgb(dd+time*0.5/d1, sin(dd*time), d2), 1.0 );
        }




        ";

        string fragmentShader3 = @"
        uniform vec2 resolution;
        uniform float time;
  
        vec2 rand(vec2 pos)
        {
        return fract( 0.00005 * (pow(pos+2.0, pos.yx + 1.0) * 22222.0));
        }
        vec2 rand2(vec2 pos)
        {
        return rand(rand(pos));
        }
  
        float softnoise(vec2 pos, float scale)
        {
        vec2 smplpos = pos * scale;
        float c0 = rand2((floor(smplpos) + vec2(0.0, 0.0)) / scale).x;
        float c1 = rand2((floor(smplpos) + vec2(1.0, 0.0)) / scale).x;
        float c2 = rand2((floor(smplpos) + vec2(0.0, 1.0)) / scale).x;
        float c3 = rand2((floor(smplpos) + vec2(1.0, 1.0)) / scale).x;
  
        vec2 a = fract(smplpos);
        return mix(
        mix(c0, c1, smoothstep(0.0, 1.0, a.x)),
        mix(c2, c3, smoothstep(0.0, 1.0, a.x)),
        smoothstep(0.0, 1.0, a.y));
        }
  
        void main(void)
        {
        vec2 pos = gl_FragCoord.xy / resolution.y;
        pos.x += time * 0.1;
        float color = 0.0;
        float s = 1.0;
        for(int i = 0; i < 8; i++)
        {
        color += softnoise(pos+vec2(i)*0.02, s * 4.0) / s / 2.0;
        s *= 2.0;
        }
        gl_FragColor = vec4(color);
        }



        ";

        string fragmentShader4 = @"
        uniform float time;
        uniform vec2 resolution;
  
        vec2 rand(vec2 pos)
        {
        return
        fract(
        (
        pow(
        pos+2.0,
        pos.yx+2.0
        )*555555.0
        )
        );
        }
  
        vec2 rand2(vec2 pos)
        {
        return rand(rand(pos));
        }
  
        float softnoise(vec2 pos, float scale) {
        vec2 smplpos = pos * scale;
        float c0 = rand2((floor(smplpos) + vec2(0.0, 0.0)) / scale).x;
        float c1 = rand2((floor(smplpos) + vec2(1.0, 0.0)) / scale).x;
        float c2 = rand2((floor(smplpos) + vec2(0.0, 1.0)) / scale).x;
        float c3 = rand2((floor(smplpos) + vec2(1.0, 1.0)) / scale).x;
  
        vec2 a = fract(smplpos);
        return mix(mix(c0, c1, smoothstep(0.0, 1.0, a.x)),
        mix(c2, c3, smoothstep(0.0, 1.0, a.x)),
        smoothstep(0.0, 1.0, a.x));
        }
  
        void main( void ) {
        vec2 pos = gl_FragCoord.xy / resolution.y - time * 0.4;
  
        float color = 0.0;
        float s = 1.0;
        for (int i = 0; i < 6; ++i) {
        color += softnoise(pos + vec2(0.01 * float(i)), s * 4.0) / s / 2.0;
        s *= 2.0;
        }
        gl_FragColor = vec4(color,mix(color,cos(color),sin(color)),color,1);
        }



        
        ";

        string fragmentShader5 = @"
        uniform float time;
        uniform vec2 resolution;
  
        // tie nd die by Snoep Games.
  
        void main( void ) {
  
        vec3 color = vec3(1.0, 0., 0.);
        vec2 pos = (( 1.4 * gl_FragCoord.xy - resolution.xy) / resolution.xx)*1.5;
        float r=sqrt(pos.x*pos.x+pos.y*pos.y)/15.0;
        float size1=2.0*cos(time/60.0);
        float size2=2.5*sin(time/12.1);
  
        float rot1=13.00; //82.0+16.0*sin(time/4.0);
        float rot2=-50.00; //82.0+16.0*sin(time/8.0);
        float t=sin(time);
        float a = (60.0)*sin(rot1*atan(pos.x-size1*pos.y/r,pos.y+size1*pos.x/r)+time);
        a += 200.0*acos(pos.x*2.0+cos(time/2.0))+asin(pos.y*5.0+sin(time/2.0));
        a=a*(r/50.0);
        a=200.0*sin(a*5.0)*(r/30.0);
        if(a>5.0) a=a/200.0;
        if(a<0.5) a=a*22.5;
        gl_FragColor = vec4( cos(a/20.0),a*cos(a/200.0),sin(a/8.0), 1.0 );
        }



        ";

        string fragmentShader6 = @"
        uniform float time;
        uniform vec2 resolution;
  
  
        void main( void )
        {
  
        vec2 uPos = ( gl_FragCoord.xy / resolution.xy );//normalize wrt y axis
        //suPos -= vec2((resolution.x/resolution.y)/2.0, 0.0);//shift origin to center
  
        uPos.x -= 1.0;
        uPos.y -= 0.5;
  
        vec3 color = vec3(0.0);
        float vertColor = 2.0;
        for( float i = 0.0; i < 15.0; ++i )
        {
        float t = time * (0.9);
  
        uPos.y += sin( uPos.x*i + t+i/2.0 ) * 0.1;
        float fTemp = abs(1.0 / uPos.y / 100.0);
        vertColor += fTemp;
        color += vec3( fTemp*(10.0-i)/10.0, fTemp*i/10.0, pow(fTemp,1.5)*1.5 );
        }
  
        vec4 color_final = vec4(color, 1.0);
        gl_FragColor = color_final;
        }




        ";


        Mesh cube;
        float step = 0;
        public ShaderMaterialExample() : base()
        {

        }
        public override void InitLighting()
        {
            var ambientLight = new AmbientLight(THREE.Color.Hex(0x0c0c0c));
            scene.Add(ambientLight);

            var spotLight = new SpotLight(THREE.Color.Hex(0xffffff));
            spotLight.Position.Set(-40, 60, -10);
            spotLight.CastShadow = true;
            scene.Add(spotLight);
        }
        public override void InitCamera()
        {
            base.InitCamera();
            camera.Fov = 45.0f;
            camera.Aspect = this.AspectRatio;
            camera.Near = 0.1f;
            camera.Far = 1000.0f;
            camera.Position.Set(0, 20, 40);
            camera.LookAt(new THREE.Vector3(10, 0, 0));
        }
        public override void Init()
        {
            base.Init();
            Resolution.X = this.Width;
            Resolution.Y = this.Height;

            var cubeGeometry = new BoxGeometry(20, 20, 20);

            var meshMaterial1 = CreateMaterial(vertexShaderCode, fragmentShader1);

            var meshMaterial2 = CreateMaterial(vertexShaderCode, fragmentShader2);

            var meshMaterial3 = CreateMaterial(vertexShaderCode, fragmentShader3);

            var meshMaterial4 = CreateMaterial(vertexShaderCode, fragmentShader4);

            var meshMaterial5 = CreateMaterial(vertexShaderCode, fragmentShader5);

            var meshMaterial6 = CreateMaterial(vertexShaderCode, fragmentShader6);

            var material = new List<Material>() { meshMaterial1, meshMaterial2, meshMaterial3, meshMaterial4, meshMaterial5, meshMaterial6 };

            Material meshMaterial = new MeshBasicMaterial()
            {
                Color = THREE.Color.Hex(0x7777ff),
                Name = "Basic Material",
                FlatShading = true,
                Opacity = 0.01f,
                ColorWrite = true,
                Fog = true
            };

            cube = new Mesh(cubeGeometry, material);

            scene.Add(cube);

          
        }

        public override void OnResize(ResizeEventArgs clientSize)
        {
            base.OnResize(clientSize);
            Resolution.X = clientSize.Width;
            Resolution.X = clientSize.Height;

        }
        private ShaderMaterial CreateMaterial(string vertexShader, string fragmentShader)
        {
            GLUniforms uniforms = new GLUniforms
                {
                    { "time",       new GLUniform {{"value", 0.2f}}},
                    { "scale",      new GLUniform {{"value", 0.2f}}},

                    { "alpha",      new GLUniform {{"value", 0.6f}}},
                    { "resolution", new GLUniform {{"value",Resolution}}}
                };

            var meshMaterial = new ShaderMaterial()
            {
                Uniforms = uniforms,
                VertexShader = vertexShader,
                FragmentShader = fragmentShader,
                Transparent = true
            };

            return meshMaterial;
        }

        public override void Render()
        {
            if (!this.ImWantMouse)
                controls.Enabled = true;
            else
                controls.Enabled = false;

            controls?.Update();
            renderer?.Render(scene, camera);

            cube.Rotation.Y = step += 0.001f;
            cube.Rotation.X = step;
            cube.Rotation.Z = step;

            cube.Materials.ForEach(m =>
            {
                var time = (float)((m as ShaderMaterial).Uniforms["time"] as Dictionary<string,object>)["value"];
                time += 0.01f;
                ((m as ShaderMaterial).Uniforms["time"] as Dictionary<string,object>)["value"] = time;

            });
        }
    }
}

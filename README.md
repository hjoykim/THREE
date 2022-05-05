# THREE

C# port of Three.js r112
three.js is a very powerful, simple and useful OpenGL Graphics library. I always want to express my deep appreciation to the team that created this library.(https://github.com/mrdoob/three.js). There was a time when I wanted to show how to use these 3D geometry and mathematical theories with OpenGL libraries. In that case, the examples of three.js was a very good.

However, someone who are familiar with structural languages such as C++, Java, C#, etc find it difficult to understand how three.js works. So I decided to port three.js to C++, C#. However, it seemed that it would take too much time to port the massive examples of three.js Fortunately, I found very simple and key examples in Learning Three.js written by Jos Dirksen(https://github.com/josdirksen/learning-threejs) Most of three.js core have been completed, but some code have not yet been done because I am lazy. I don't know if I can afford it, the rest will be completed as soon as time permits. 


There is C++ port of Three.js   (https://github.com/hjoykim/ThreeCpp)

## Example code
        public override void Load(GLControl glControl)
        {
            base.Load(glControl);

            InitRenderer();

            InitCamera();

            InitCameraController();

            scene.Background = Color.Hex(0xffffff);

            var axes = new AxesHelper(20);

            scene.Add(axes);

            var planeGeometry = new PlaneGeometry(60, 20);
            var planeMaterial = new MeshBasicMaterial() { Color = Color.Hex(0xcccccc) };
            var plane = new Mesh(planeGeometry, planeMaterial);

            plane.Rotation.X = (float)(-0.5 * Math.PI);
            plane.Position.Set(15, 0, 0);

            scene.Add(plane);

            // create a cube
            var cubeGeometry = new BoxGeometry(4, 4, 4);
            var cubeMaterial = new MeshBasicMaterial(){ Color=Color.Hex(0xff0000), Wireframe= true};
            var cube = new Mesh(cubeGeometry, cubeMaterial);

            // position the cube
            cube.Position.Set(-4, 3, 0);

            // add the cube to the scene
        
		    scene.Add(cube);

      //      // create a sphere
            var sphereGeometry = new SphereGeometry(4, 20, 20);
            var sphereMaterial = new MeshBasicMaterial() { Color = Color.Hex(0x7777ff), Wireframe = true };
            var sphere = new Mesh(sphereGeometry, sphereMaterial);

      //      // position the sphere
            sphere.Position.Set(20, 4, 2);

      //      // add the sphere to the scene
            scene.Add(sphere);

        }
        public override void Render()
        {
            controls.Update();
            this.renderer.Render(scene, camera);
        }


### Example Screen Capture  
> All examples have a trackball controller.
 
> Rotation : Move mouse while mouse left button pressed
 
> Move : Move mouse while mouse middle button pressed
 
> Zoom : Mouse scroll
#### Chapter01 - First Scene
![image](https://user-images.githubusercontent.com/3807476/166918925-71e710fb-7d0d-4d96-9f75-5ea515eb8b71.png)
#### Chapter02 - Geometries
![image](https://user-images.githubusercontent.com/3807476/166918735-a847529e-46fc-41e3-b886-2701df6a046b.png)
#### Chapter03 - Lensflares Light
![image](https://user-images.githubusercontent.com/3807476/166921275-620f734a-46f1-4f8d-b23d-e3576fb251f2.png)
### Chapter04~Chapter08 - All the examples have been completed, but they have not been committed yet
> some examples are include DataLoader like as .obj
#### Chapter09 - Trackball controls
![image](https://user-images.githubusercontent.com/3807476/166919252-6cb474c5-a971-474c-bf7c-a8b565bc4cd4.png)
#### Chapter10 env map
![image](https://user-images.githubusercontent.com/3807476/166919731-03ea7a08-e275-452a-8d7f-054c0d3c8570.png)
#### Chapter11 SimplePass1
![image](https://user-images.githubusercontent.com/3807476/166920027-efa0ece8-1cfc-4f8a-a553-85a743c2d682.png)




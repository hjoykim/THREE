using THREE;
using Color = THREE.Color;
namespace THREEExample.Three.Geometries
{
    [Example("convex", ExampleCategory.ThreeJs,"geometry")]
    public class ConvexGeometryExample:Example
    {
        Group group;
        public ConvexGeometryExample() : base() { }
        public override void InitRenderer()
        {
            base.InitRenderer();
            renderer.SetClearColor(0, 1);
        }
        public override void InitCamera()
        {
            camera = new PerspectiveCamera(40, glControl.AspectRatio, 1, 1000);
            camera.Position.Set(15, 20, 30);
            scene.Add(camera);
        }
        public override void InitLighting()
        {
            scene.Add(new AmbientLight(0x222222));

            // light

            var light = new PointLight(0xffffff, 1);
            camera.Add(light);
        }
        public override void Init()
        {
            base.Init();

            BuildScene();
        }
        public void BuildScene()
        {
            scene.Add(new AxesHelper(20));

            var texture = TextureLoader.Load("../../../../assets/textures/sprites/disc.png");

            group = new Group();
           

            // points

            var geometry = new DodecahedronGeometry(10.0f);
            var vertices = geometry.Vertices;

            var pointsMaterial = new PointsMaterial()
            {

                Color = Color.Hex(0x0080ff),
                Map = texture,
                Size = 1,
                AlphaTest = 0.5f,
                DepthWrite=false

            };

            var pointsGeometry = new BufferGeometry().SetFromPoints(vertices);

            var points = new Points(pointsGeometry, pointsMaterial);
            group.Add(points);

            // convex hull

            var meshMaterial = new MeshLambertMaterial()
            {
                Color = Color.Hex(0xffffff),
                Opacity = 0.5f,
                Transparent = true

            };
            var meshGeometry = new ConvexBufferGeometry(vertices.ToArray());
            var mesh = new Mesh(meshGeometry, meshMaterial);
            mesh.Material.Side = Constants.BackSide; // back faces
            mesh.RenderOrder = 0;
            group.Add(mesh);

            mesh = new Mesh(meshGeometry, meshMaterial.Clone() as MeshLambertMaterial);
            mesh.Material.Side = Constants.FrontSide; // front faces
            mesh.RenderOrder = 1;
            group.Add(mesh);

            scene.Add(group);
        }
    }
}

namespace THREE.Silk.Example
{
    [Example("Terrain Fog", ExampleCategory.ThreeJs, "Geometry")]
    public class GeometryTerrainFogExample : GeometryTerrainExample 
    {
        public GeometryTerrainFogExample() : base()
        {
            scene.Fog = new THREE.Fog(0xefd1b5);
        }
        public override void InitCamera()
        {
            camera = new THREE.PerspectiveCamera(60, this.AspectRatio, 1, 10000);
            camera.Position.Set(100, 800, -800);
            camera.LookAt(-100, 810, -800);
        }
    }
}

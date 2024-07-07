using System.Collections;
using System.Collections.Generic;
using THREE;

namespace THREEExample.Three.Geometries
{
    [Example("EdgesGeometry",ExampleCategory.ThreeJs,"geometry")]
    public class EdgesGeometryExample : ExampleTemplate
    {
        public EdgesGeometryExample() : base() { }

        public override void InitCamera()
        {
            camera = new PerspectiveCamera(60, glControl.AspectRatio, 1, 100);
            camera.Position.SetScalar(10);
        }
        public override void InitRenderer()
        {
            base.InitRenderer();
            renderer.SetClearColor(0x000000, 1);
        }

        public override void Init()
        {
            base.Init();

            var boxEdges = new EdgesGeometry(new BoxBufferGeometry(5, 1, 5));

            var instGeom = new InstancedBufferGeometry();
            instGeom.SetAttribute("position", boxEdges.Attributes["position"] as BufferAttribute<float>);

            var instOffset = new List<float>();
            for(int i = -1; i <= 1; i++)
            {
                for(int j = -1; j <= 1; j++)
                {
                    instOffset.Add(j * 6, 0, i * 6);
                }
            }
            instGeom.SetAttribute("offset", new InstancedBufferAttribute<float>(instOffset.ToArray(), 3));
            instGeom.InstanceCount = int.MaxValue;

            var instMat = new LineBasicMaterial()
            {
                Color = Color.Hex(0xffff00),
                OnBeforeCompile = (Hashtable parameters, IGLRenderer render) =>
                {
                    parameters["vertexShader"] = string.Format(
                        @"attribute vec3 offset;
{0}", (parameters["vertexShader"] as string).Replace("#include <begin_vertex>", @"#include <begin_vertex>
      transformed += offset;"));

                }
            };
            var instLines = new LineSegments(instGeom, instMat);

            scene.Add(instLines);
        }

    }
}

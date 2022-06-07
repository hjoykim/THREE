using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using THREE.Materials;

namespace THREEExample.Learning.Chapter04
{
    [Example("13.Line-Material-Dashed", ExampleCategory.LearnThreeJS, "Chapter04")]
    public class LineMaterialDashedExample : LineMaterialExample
    {
        public LineMaterialDashedExample() : base()
        {

        }
        public override void BuildMeshMaterial()
        {
            meshMaterial = new LineDashedMaterial()
            {
                Opacity = 1.0f,
                LineWidth = 1,
                VertexColors = true
            };
        }
        public override void BuildMesh()
        {
            base.BuildMesh();
            line.ComputeLineDistances();
        }
    }
}

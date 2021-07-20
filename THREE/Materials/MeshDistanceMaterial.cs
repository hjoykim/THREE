using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using THREE.Math;
using THREE.Textures;

namespace THREE.Materials
{
    public class MeshDistanceMaterial : Material
    {
        public Vector3 ReferencePosition = Vector3.Zero();

        public float NearDistance = 1;

        public float FarDistance = 1000;


        public MeshDistanceMaterial() : base()
        {
            this.type = "MeshDistanceMaterial";

            this.Skinning = false;

            this.MorphTargets = false;

            this.DisplacementMap = null;

            this.DisplacementScale = 1;

            this.DisplacementBias = 0;

            this.Fog = false;
        }

    }
}

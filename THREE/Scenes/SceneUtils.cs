using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using THREE.Core;
using THREE.Materials;
using THREE.Objects;

namespace THREE.Scenes
{
    public class SceneUtils
    {
        public static Group CreateMultiMaterialObject(Geometry geometry, List<Material> materials)
        {
            Group group = new Group();

            for (int i = 0; i < materials.Count; i++)
            {
                group.Add(new Mesh(geometry, materials[i]));
            }
            return group;
        }
    }
}

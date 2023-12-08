using System.Collections.Generic;


namespace THREE
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

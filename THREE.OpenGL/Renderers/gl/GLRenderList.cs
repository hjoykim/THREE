using System.Collections;
using System.Collections.Generic;

namespace THREE
{
    [Serializable]
    public struct RenderItem
    {
        public int Id;

        public Object3D Object3D;

        public BufferGeometry Geometry;

        public Material Material;

        public GLProgram Program;

        public int GroupOrder;

        public int RenderOrder;

        public float Z;

        public DrawRange? Group;
    }

    [Serializable]
    public class GLRenderList
    {
        public List<RenderItem> Opaque = new List<RenderItem>();

        public List<RenderItem> Transmissive = new List<RenderItem>();

        public List<RenderItem> Transparent = new List<RenderItem>();

        public List<RenderItem> renderItems = new List<RenderItem>();

        public int renderItemsIndex = 0;

        private GLProgram defaultProgram = new GLProgram() { Id = -1 };

        private GLProperties properties;

        public GLRenderList(GLProperties properties)
        {
            this.properties = properties;
        }


        private RenderItem GetNextRenderItem(Object3D object3D, BufferGeometry geometry, Material material, int groupOrder, float z, DrawRange? group)
        {
            RenderItem renderItem;
            Hashtable materialProperties = properties.Get(material);
            GLProgram program = (GLProgram)materialProperties["program"];
            if (this.renderItemsIndex > this.renderItems.Count - 1)
            {
                renderItem = new RenderItem
                {
                    Id = object3D.Id,
                    Object3D = object3D,
                    Geometry = geometry,
                    Material = material,
                    Program = program!=null ? program :defaultProgram,
                    GroupOrder = groupOrder,
                    RenderOrder = object3D.RenderOrder,
                    Z = z,
                    Group = group
                };
                this.renderItems.Add(renderItem);
            }
            else
            {
                renderItem = this.renderItems[this.renderItemsIndex];
                renderItem.Id = object3D.Id;
                renderItem.Object3D = object3D;
                renderItem.Geometry = geometry;
                renderItem.Material = material;
                renderItem.Program = program!=null ? program : defaultProgram;
                renderItem.GroupOrder = groupOrder;
                renderItem.RenderOrder = object3D.RenderOrder;
                renderItem.Z = z;
                renderItem.Group = group;
            }

            this.renderItemsIndex++;

            return renderItem;
        }
        public void Push(Object3D object3D, BufferGeometry geometry, Material material, int groupOrder, float z, DrawRange? group)
        {
            RenderItem renderItem = this.GetNextRenderItem(object3D, geometry, material, groupOrder, z, group);
            if (material.Transmission > 0.0f)
                this.Transmissive.Add(renderItem);
            else if (material.Transparent == true)
                this.Transparent.Add(renderItem);
            else
                this.Opaque.Add(renderItem);
        }

        public void Unshift(Object3D object3D, BufferGeometry geometry, Material material, int groupOrder, float z, DrawRange? group)
        {
            RenderItem renderItem = this.GetNextRenderItem(object3D, geometry, material, groupOrder, z, group);
            if(material.Transmission > 0.0f)
                this.Transmissive.Insert(0,renderItem);
            else if (material.Transparent == true)
                this.Transparent.Insert(0, renderItem);
            else
                this.Opaque.Insert(0, renderItem);
        }

        public void Init()
        {
            Opaque.Clear();
            Transparent.Clear();
            Transmissive.Clear();
            renderItems.Clear();
            renderItemsIndex = 0;
        }
        public void Finish()
        {
            renderItems.Clear();
            renderItemsIndex = 0;
        }
        public void Sort()
        {
            if (this.Opaque.Count > 0)
            {
                this.Opaque.Sort(delegate (RenderItem a, RenderItem b)
                {
                    if (a.GroupOrder != b.GroupOrder)
                    {
                        return a.GroupOrder - b.GroupOrder;
                    }
                    else if (a.RenderOrder != b.RenderOrder)
                    {
                        return a.RenderOrder - b.RenderOrder;
                    }
                    else if (a.Program != b.Program)
                    {
                        return a.Program.Id - b.Program.Id;
                    }
                    else if (a.Material.Id != b.Material.Id)
                    {
                        return a.Material.Id - b.Material.Id;
                    }
                    else if (a.Z != b.Z)
                    {
                        return (int)(a.Z - b.Z);
                    }
                    else
                    {
                        return a.Id - b.Id;
                    }
                });
            }
            if (this.Transparent.Count > 0)
            {
                this.Transparent.Sort(delegate (RenderItem a, RenderItem b)
                {
                    if (a.GroupOrder != b.GroupOrder)
                    {
                        return a.GroupOrder - b.GroupOrder;

                    }
                    else if (a.RenderOrder != b.RenderOrder)
                    {

                        return a.RenderOrder - b.RenderOrder;

                    }
                    else if (a.Z != b.Z)
                    {

                        return (int)(b.Z - a.Z);

                    }
                    else
                    {

                        return a.Id - b.Id;

                    }
                });
            }
        }
    }
}

namespace THREE
{
    public class MeshMatcapMaterial : Material
    {
        public Texture Matcap;

        public MeshMatcapMaterial()
            : base()
        {
            this.type = "MeshMatcapMaterial";
        }
    }
}

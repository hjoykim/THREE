namespace THREE
{
    public class ShadowMaterial : Material
    {
        public ShadowMaterial()
        {
            this.type = "ShadowMaterial";

            this.Transparent = true;

            this.Color = new Color().SetHex(0x000000);
        }
    }
}

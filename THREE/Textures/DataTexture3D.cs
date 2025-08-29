namespace THREE
{
    [Serializable]
    public class DataTexture3D : Texture
    {
        public int Width;
        public int Height;
        public int Depth;

        public byte[] Data;
        public float[] DataFloat;
        public DataTexture3D(byte[] array, int width, int height, int depth) : base()
        {
            this.Data = array;
            this.Width = width;
            this.Height = height;
            this.Depth = depth;
            this.WrapR = Constants.ClampToEdgeWrapping;

            this.GenerateMipmaps = false;

            this.flipY = false;
            this.Type = Constants.UnsignedByteType;
            this.NeedsUpdate = true;
        }
        public DataTexture3D(float[] array, int width, int height, int depth) : base()
        {
            this.DataFloat = array;
            this.Width = width;
            this.Height = height;
            this.Depth = depth;
            this.Type = (int)Constants.FloatType;
            this.WrapR = Constants.ClampToEdgeWrapping;

            this.GenerateMipmaps = false;

            this.flipY = false;
            this.Type = Constants.FloatType;
            this.NeedsUpdate = true;
        }
    }
}

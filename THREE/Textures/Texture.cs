using OpenTK.Graphics.ES30;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace THREE
{
    public class MipMap  
    {
        public byte[] Data;

        public int Width;

        public int Height;

        public MipMap() { }

        public MipMap(MipMap other)
        {
            if (other.Data.Length > 0)
                this.Data = other.Data.ToArray();
        }
        public MipMap Clone()
        {
            return new MipMap(this);
        }
    }

    public class Texture : DisposableObject
    {
        #region Static Fields

        protected static int TextureIdCount;

        #endregion

        #region Fields

        public int Id = TextureIdCount++;

        public Guid Uuid = Guid.NewGuid();

        public string Name="";

        public Size Resolution { get; protected set; } // this fields are not existed in three.js

        public TextureTarget TextureTarget { get; private set; }// this fields are not existed in three.js

        public int TextureAddress { get; protected set; }


        public Bitmap Image;

        public Texture[] Images = new Texture[] { null, null, null, null, null, null };

        public List<MipMap> Mipmaps = new List<MipMap>();

        public Size ImageSize;

        public int Mapping = Constants.UVMapping;

        public int WrapS;
        public int WrapT;
        public int WrapR;

        public int MagFilter;
        public int MinFilter;
        public int MaxFilter;

        public float Anisotropy;

        public int Format = (int)Constants.RGBAFormat;
        public int Type;

        public Vector2 Offset = new Vector2(0, 0);
        public Vector2 Repeat = new Vector2(1, 1);
        public Vector2 Center = new Vector2(0, 0);
        public float Rotation = 0;

        public bool MatrixAutoUpdate = true;
        public Matrix3 Matrix = new Matrix3();

        public bool GenerateMipmaps = true;
        public bool PremultiplyAlpha = false;
        public bool flipY = true;
        public int UnpackAlignment = 4;

        private bool needsUpdate;

        public bool NeedsUpdate
        {
            get { return needsUpdate; }
            set
            {
                needsUpdate = value;
                if (value == true) version++;
            }
        }

        public string InternalFormat = null;

        public int Encoding = Constants.LinearEncoding;

        public int version = 0;

        public string SourceFilePath;


        private readonly int defaultMapping = Constants.UVMapping;

        #endregion


        //public bool __glInit = false;

        //public int __glTexture { get; set; }

        //public int __version;

        #region Constructors and Destructors

        public Texture()
        {
            this.Anisotropy = 1;

            this.WrapS = (int)Constants.ClampToEdgeWrapping;

            this.WrapT = (int)Constants.ClampToEdgeWrapping;

            this.MagFilter = (int)Constants.LinearFilter;

            this.MinFilter = (int)Constants.LinearMipMapLinearFilter;

            this.Type = (int)Constants.UnsignedByteType;

        }

        public Texture(string bitmapPath, bool flipY = true)
        {
            this.SourceFilePath = bitmapPath;

            using (var bitmap = Bitmap.FromFile(bitmapPath) as Bitmap)
            {
                HandleLoadingBitmapData(bitmap, flipY);
            }
        }
        /// <summary>
        /// Constructor
        /// </summary>
        public Texture(Bitmap image = null, int? mapping = null, int? wrapS = null, int? wrapT = null, int? magFilter = null, int? minFilter = null, int? format = null, int? type = null, int? anisotropy = null, int? encoding=null)
            : this()
        {

            this.Image = image;

            this.Mapping = mapping!=null ? (int)mapping:this.defaultMapping;

            this.WrapS = wrapS != null ? (int)wrapS : Constants.ClampToEdgeWrapping;
            this.WrapT = wrapT != null ? (int)wrapT : Constants.ClampToEdgeWrapping;

            this.MagFilter = magFilter!=null ? (int)magFilter : Constants.LinearFilter;
            this.MinFilter = minFilter != null ? (int)minFilter : Constants.LinearMipmapLinearFilter;

            this.Anisotropy = anisotropy != null ? (int)anisotropy : 1;

            this.Format = format != null ? (int)format : Constants.RGBAFormat;
            this.InternalFormat = null;
            this.Type = type != null ? (int)type : Constants.UnsignedByteType;

            this.Encoding = encoding != null ? (int)encoding : Constants.LinearEncoding;

        }

        /// <summary>
        /// Copy Constructor
        /// </summary>
        /// <param name="other"></param>
        protected Texture(Texture other) : this()
        {
            this.Image = other.Image;
            //this.Image = other.Image!=null ? (Bitmap)other.Image.Clone() : null;

            this.ImageSize = other.ImageSize;
            this.Images = other.Images;
            //if(other.Images.Length>0)
            //{
            //    for (int i = 0; i < other.Images.Length; i++)
            //        this.Images[i] = other.Images[i] != null ? (Texture)other.Images[i].Clone() : null;
            //}

            this.Mipmaps = other.Mipmaps;
            //this.Mipmaps = other.Mipmaps.Select(item => (MipMap)item.Clone()).ToList();

            this.Mapping = other.Mapping;

            this.WrapS = other.WrapS;
            this.WrapT = other.WrapT;

            this.MagFilter = other.MagFilter;
            this.MinFilter = other.MinFilter;

            this.Anisotropy = other.Anisotropy;

            this.Format = other.Format;
            this.InternalFormat = other.InternalFormat;
            this.Type = other.Type;

            this.Encoding = other.Encoding;

            this.version = other.version;
        }

        #endregion

        public void UpdateMatrix()
        {
            this.Matrix.SetUvTransform(this.Offset.X, this.Offset.Y, this.Repeat.X, this.Repeat.Y, this.Rotation, this.Center.X, this.Center.Y);
        }

        public Texture Clone()
        {
            return new Texture(this);
        }

        private void HandleLoadingBitmapData(Bitmap bitmap, bool flipY = true)
        {
            /* .net library has methods for converting many image formats so I exploit that by using 
              * .net to convert any filetype to a bitmap.  Then the bitmap is locked into memory so
              * that the garbage collector doesn't touch it, and it is read via OpenGL glTexImage2D. */
            if (flipY) bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);     // bitmaps read from bottom up, so flip it
            Resolution = bitmap.Size;

            // must be Format32bppArgb file format, so convert it if it isn't in that format
            var bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            // set the texture target and then generate the texture ID            
            TextureTarget = TextureTarget.Texture2D;
            TextureAddress = GL.GenTexture();

            GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1); // set pixel alignment
            GL.BindTexture(TextureTarget, TextureAddress);     // bind the texture to memory in OpenGL

            //Gl.TexParameteri(TextureTarget, TextureParameterName.GenerateMipmap, 0);
            TextureTarget2d target = (TextureTarget2d)Enum.Parse(typeof(TextureTarget), TextureTarget.ToString());
            GL.TexImage2D(target, 0, TextureComponentCount.Rgba, bitmapData.Width, bitmapData.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, bitmapData.Scan0);

            GL.TexParameter(TextureTarget, TextureParameterName.TextureMagFilter, MagFilter);
            GL.TexParameter(TextureTarget, TextureParameterName.TextureMinFilter, MinFilter);


            if (GenerateMipmaps) GL.GenerateMipmap(TextureTarget.Texture2D);


            bitmap.UnlockBits(bitmapData);
        }
        
    }
}

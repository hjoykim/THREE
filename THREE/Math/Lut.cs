using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;


namespace THREE
{
    public class Lut
    {
        public Hashtable ColorMapKeywords;
        float minV, maxV;
        public int? n;
        List<Color> lut = new List<Color>();
        List<float[]> map = new List<float[]>();

        public Lut(string colorMap = null, int? numberofcolors = null)
        {
            minV = 0;
            maxV = 1;

            ColorMapKeywords = new Hashtable() {
                { "rainbow",new List<float[]>() {new float[]{ 0.0f, 0x0000FF }, new float[] { 0.2f, 0x00FFFF }, new float[] {0.5f, 0x00FF00 }, new float[]{ 0.8f, 0xFFFF00 }, new float[]{ 1.0f, 0xFF0000 } } },
                { "cooltowarm", new List<float[]>(){ new float[] { 0.0f, 0x3C4EC2 }, new float[] { 0.2f, 0x9BBCFF }, new float[] { 0.5f, 0xDCDCDC }, new float[] { 0.8f, 0xF6A385 }, new float[] { 1.0f, 0xB40426 }} },
                { "blackbody", new List<float[]>(){ new float[] { 0.0f, 0x000000 }, new float[] { 0.2f, 0x780000 }, new float[] { 0.5f, 0xE63200 }, new float[] { 0.8f, 0xFFFF00 }, new float[] { 1.0f, 0xFFFFFF }} },
                { "grayscale", new List<float[]>(){ new float[] { 0.0f, 0x000000 }, new float[] { 0.2f, 0x404040 }, new float[] { 0.5f, 0x7F7F80 }, new float[] { 0.8f, 0xBFBFBF }, new float[] { 1.0f, 0xFFFFFF }} }
            };

            SetColorMap(colorMap, numberofcolors);

        }
        public Lut Set(Lut value)
        {

            if (value is Lut)
            {

                this.Copy(value);

            }

            return this;

        }

        public void SetMin(float min)
        {

            this.minV = min;


        }

        public void SetMax(float max)
        {

            this.maxV = max;


        }
        public Lut SetColorMap(string colormap, int? numberofcolors)
        {
            if (colormap != null && ColorMapKeywords.ContainsKey(colormap))
                map = (List<float[]>)ColorMapKeywords[colormap];
            else
                map = (List<float[]>)ColorMapKeywords["rainbow"];

            this.n = numberofcolors != null ? numberofcolors.Value : 32;

            var step = 1.0 / this.n.Value;

            lut.Clear();

            for (var i = 0.0; i <= 1.0; i += step)
            {

                for (var j = 0; j < this.map.Count - 1; j++)
                {

                    if (i >= this.map[j][0] && i < this.map[j + 1][0])
                    {

                        var min = this.map[j][0];
                        var max = this.map[j + 1][0];

                        var minColor = new Color(this.map[j][1]);
                        var maxColor = new Color(this.map[j + 1][1]);

                        var color = minColor.Lerp(maxColor,(float)((i - min) / (max - min)));

                        this.lut.Add(color);

                    }

                }

            }

            return this;

        }
        public Lut Copy(Lut lut)
        {

            this.lut = lut.lut;
            this.map = lut.map;
            this.n = lut.n;
            this.minV = lut.minV;
            this.maxV = lut.maxV;
            this.lut = new List<Color>(lut.lut);

            return this;
        }
        public Color GetColor(float alpha)
        {

            if (alpha <= this.minV)
            {

                alpha = this.minV;

            }
            else if (alpha >= this.maxV)
            {

                alpha = this.maxV;

            }

            alpha = (alpha - this.minV) / (this.maxV - this.minV);

            var colorPosition = (int)System.Math.Round(alpha * this.n.Value);

            if (colorPosition == this.n.Value)
                colorPosition -= 1;

            return this.lut[colorPosition];

        }
        public void AddColorMap(string colormapName, List<float[]> arrayOfColors)
        {

            ColorMapKeywords[colormapName] = arrayOfColors;

        }
        /*
         * createCanvas: function () {

		var canvas = document.createElement( 'canvas' );
		canvas.width = 1;
		canvas.height = this.n;

		this.updateCanvas( canvas );

		return canvas;

	},
         
        */
        public Texture CreateTexture()
        {
            Texture texture = new Texture();
            Bitmap bitmap = new Bitmap(1, this.n.Value);
            texture.Image = bitmap;
            texture.ImageSize.Width = 1;
            texture.ImageSize.Height = this.n.Value;
            texture.Format = Constants.RGBAFormat;
            texture.NeedsUpdate = true;

            UpdateTexture(texture);

            return texture;
        }
        /**
         updateCanvas: function ( canvas ) {

		    var ctx = canvas.getContext( '2d', { alpha: false } );

		    var imageData = ctx.getImageData( 0, 0, 1, this.n );

		    var data = imageData.data;

		    var k = 0;

		    var step = 1.0 / this.n;

		    for ( var i = 1; i >= 0; i -= step ) {

			    for ( var j = this.map.length - 1; j >= 0; j -- ) {

				    if ( i < this.map[ j ][ 0 ] && i >= this.map[ j - 1 ][ 0 ] ) {

					    var min = this.map[ j - 1 ][ 0 ];
					    var max = this.map[ j ][ 0 ];

					    var minColor = new Color( this.map[ j - 1 ][ 1 ] );
					    var maxColor = new Color( this.map[ j ][ 1 ] );

					    var color = minColor.lerp( maxColor, ( i - min ) / ( max - min ) );

					    data[ k * 4 ] = Math.round( color.r * 255 );
					    data[ k * 4 + 1 ] = Math.round( color.g * 255 );
					    data[ k * 4 + 2 ] = Math.round( color.b * 255 );
					    data[ k * 4 + 3 ] = 255;

					    k += 1;

				    }

			    }

		    }

		    ctx.putImageData( imageData, 0, 0 );

		    return canvas;

	    }
         */
        public void UpdateTexture(Texture texture)
        {
            byte[] data = new byte[4 * this.n.Value];

            var k = 0;

            var step = 1.0 / this.n.Value;

            for(var i = 1.0; i >= 0.0; i -= step)
            {
                for(var j = this.map.Count - 1; j >= 0; j--)
                {
                    if (i < this.map[j][0] && i >= this.map[j - 1][0])
                    {
                        var min = this.map[j - 1][0];
                        var max = this.map[j][0];

                        var minColor = new Color(this.map[j - 1][1]);
                        var maxColor = new Color(this.map[j][1]);

                        var color = minColor.Lerp(maxColor, (float)((i - min) / (max - min)));

                        data[k * 4] = (byte)System.Math.Round(color.R * 255);
                        data[k * 4 + 1] = (byte)System.Math.Round(color.G * 255);
                        data[k * 4 + 2] = (byte)System.Math.Round(color.B * 255);
                        data[k * 4 + 3] = 255;

                        k += 1;
                    }
                }
            }

            unsafe
            {
                fixed(byte* ptr = data)
                {
                    Bitmap image = new Bitmap(1, this.n.Value, 4, PixelFormat.Format32bppArgb, new IntPtr(ptr));
                    //image.Save(@"sprite.png");
                    texture.Image = image;
                }
            }
        }
    }

}

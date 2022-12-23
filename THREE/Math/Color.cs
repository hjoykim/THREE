using System;

namespace THREE
{

    public enum ColorKeywords
    {
        aliceblue=0xf0f8ff,
        antiquewhite=0xfaebd7,
        aqua=0x00ffff,
        aquamarine=0x7fffd4,
        azure=0xf0ffff,
	    beige=0xf5f5dc,
        bisque=0xffe4c4,
        black=0x000000,
        blanchedalmond=0xffebcd,
        blue=0x0000ff,
        blueviolet=0x8a2be2,
	    brown=0xa52a2a,
        burlywood=0xdeb887,
        cadetblue=0x5f9ea0,
        chartreuse=0x7fff00,
        chocolate=0xd2691e,
        coral=0xff7f50,
	    cornflowerblue=0x6495ed,
        cornsilk=0xfff8dc,
        crimson=0xdc143c,
        cyan=0x00ffff,
        darkblue=0x00008b,
        darkcyan=0x008b8b,
	    darkgoldenrod=0xb8860b,
        darkgray=0xa9a9a9,
        darkgreen=0x006400,
        darkgrey=0xa9a9a9,
        darkkhaki=0xbdb76b,
        darkmagenta=0x8b008b,
        darkolivegreen=0x556b2f,
        darkorange=0xff8c00,
        darkorchid=0x9932cc,
        darkred=0x8b0000,
        darksalmon=0xe9967a,
        darkseagreen=0x8fbc8f,
	    darkslateblue=0x483d8b,
        darkslategray=0x2f4f4f,
        darkslategrey=0x2f4f4f,
        darkturquoise=0x00ced1,
        darkviolet=0x9400d3,
	    deeppink=0xff1493,
        deepskyblue=0x00bfff,
        dimgray=0x696969,
        dimgrey=0x696969,
        dodgerblue=0x1e90ff,
        firebrick=0xb22222,
	    floralwhite=0xfffaf0,
        forestgreen=0x228b22,
        fuchsia=0xff00ff,
        gainsboro=0xdcdcdc,
        ghostwhite=0xf8f8ff,
        gold=0xffd700,
	    goldenrod=0xdaa520,
        gray=0x808080,
        green=0x008000,
        greenyellow=0xadff2f,
        grey=0x808080,
        honeydew=0xf0fff0,
        hotpink=0xff69b4,
	    indianred=0xcd5c5c,
        indigo=0x4b0082,
        ivory=0xfffff0,
        khaki=0xf0e68c,
        lavender=0xe6e6fa,
        lavenderblush=0xfff0f5,
        lawngreen=0x7cfc00,
	    lemonchiffon=0xfffacd,
        lightblue=0xadd8e6,
        lightcoral=0xf08080,
        lightcyan=0xe0ffff,
        lightgoldenrodyellow=0xfafad2,
        lightgray=0xd3d3d3,
	    lightgreen=0x90ee90,
        lightgrey=0xd3d3d3,
        lightpink=0xffb6c1,
        lightsalmon=0xffa07a,
        lightseagreen=0x20b2aa,
        lightskyblue=0x87cefa,
	    lightslategray=0x778899,
        lightslategrey=0x778899,
        lightsteelblue=0xb0c4de,
        lightyellow=0xffffe0,
        lime=0x00ff00,
        limegreen=0x32cd32,
	    linen=0xfaf0e6,
        magenta=0xff00ff,
        maroon=0x800000,
        mediumaquamarine=0x66cdaa,
        mediumblue=0x0000cd,
        mediumorchid=0xba55d3,
	    mediumpurple=0x9370db,
        mediumseagreen=0x3cb371,
        mediumslateblue=0x7b68ee,
        mediumspringgreen=0x00fa9a,
        mediumturquoise=0x48d1cc,
	    mediumvioletred=0xc71585,
        midnightblue=0x191970,
        mintcream=0xf5fffa,
        mistyrose=0xffe4e1,
        moccasin=0xffe4b5,
        navajowhite=0xffdead,
	    navy=0x000080,
        oldlace=0xfdf5e6,
        olive=0x808000,
        olivedrab=0x6b8e23,
        orange=0xffa500,
        orangered=0xff4500,
        orchid=0xda70d6,
	    palegoldenrod=0xeee8aa,
        palegreen=0x98fb98,
        paleturquoise=0xafeeee,
        palevioletred=0xdb7093,
        papayawhip=0xffefd5,
        peachpuff=0xffdab9,
	    peru=0xcd853f,
        pink=0xffc0cb,
        plum=0xdda0dd,
        powderblue=0xb0e0e6,
        purple=0x800080,
        rebeccapurple=0x663399,
        red=0xff0000,
        rosybrown=0xbc8f8f,
	    royalblue=0x4169e1,
        saddlebrown=0x8b4513,
        salmon=0xfa8072,
        sandybrown=0xf4a460,
        seagreen=0x2e8b57,
        seashell=0xfff5ee,
	    sienna=0xa0522d,
        silver=0xc0c0c0,
        skyblue=0x87ceeb,
        slateblue=0x6a5acd,
        slategray=0x708090,
        slategrey=0x708090,
        snow=0xfffafa,
	    springgreen=0x00ff7f,
        steelblue=0x4682b4,
        tan=0xd2b48c,
        teal=0x008080,
        thistle=0xd8bfd8,
        tomato=0xff6347,
        turquoise=0x40e0d0,
	    violet=0xee82ee,
        wheat=0xf5deb3,
        white=0xffffff,
        whitesmoke=0xf5f5f5,
        yellow=0xffff00,
        yellowgreen=0x9acd32,

    }
    public struct HSL 
    {
        public float H;
        
        public float S;

        public float L;

    }
    public struct Color
    {
        public float R;

        public float G;

        public float B;

       
        public Color(int hex)
        {
            hex = (int)System.Math.Floor((Decimal)hex);

            this.R = (hex >> 16 & 255) / 255f;
            this.G = (hex >> 8 & 255) / 255f;
            this.B = (hex & 255) / 255f;
        }
        public Color(float r, float? g=null, float? b=null)
        {
            R = 0;
            G = 0;
            B = 0;
            if (g == null && b==null)
            {
                Set((int)r);
            }
            else
            {
                SetRGB(r, g.Value, b.Value);
            }
        }

        public float Hue2RGB(float p, float q, float t)
        {
            if (t < 0) t += 1;
            if (t > 1) t -= 1;
            if (t < 1.0f / 6.0f) return p + (q - p) * 6 * t;
            if (t < 1.0f / 2.0f) return q;
            if (t < 2.0f / 3.0f) return p + (q - p) * 6 * (2.0f / 3.0f - t);
            return p;
        }

        public float SRGBToLinear( float c ) 
        {
	        return ( c < 0.04045 ) ? c * 0.0773993808f : (float)System.Math.Pow( c * 0.9478672986 + 0.0521327014, 2.4 );
        }

        public float LinearToSRGB(float c ) 
        {
	        return ( c < 0.0031308f ) ? c * 12.92f : 1.055f * ( (float)System.Math.Pow( c, 0.41666 ) ) - 0.055f;
        }

        public Color Set(object value )
        {

		    if ( value!=null && value is Color ) 
            {
			    this = (Color)value;
		    } 
            else if ( value is int) 
            {
			    this.SetHex((int)value);
		    } 

		    return this;
	    }
        public Color SetScalar(float scalar ) 
        {

		    this.R = scalar;
		    this.G = scalar;
		    this.B = scalar;

		    return this;
	    }
        public static Color Hex(int hex)
        {
            hex = (int)System.Math.Floor((Decimal)hex);

            
            var R = (hex >> 16 & 255) / 255f;
            var G = (hex >> 8 & 255) / 255f;
            var B = (hex & 255) / 255f;

            return new Color(R, G, B);
        }
        public Color SetHex(int hex )
        {

		    hex = (int)System.Math.Floor((Decimal)hex );

		    this.R = ( hex >> 16 & 255 ) / 255f;
		    this.G = ( hex >> 8 & 255 ) / 255f;
		    this.B = ( hex & 255 ) / 255f;

		    return this;
	    }
        public Color SetRGB(float r, float g, float b ) {

		    this.R = r;
		    this.G = g;
		    this.B = b;

		    return this;
	    }
        public float EuclideanModulo(float n,float m)
        {
            return ((n%m)+m)%m;
        }
        public Color SetHSL(float h, float s, float l ) {

		    // h,s,l ranges are in 0.0 - 1.0
		    h = EuclideanModulo( h, 1 );
		    s = s.Clamp(0, 1 );
		    l = l.Clamp(0, 1 );

		    if ( s == 0 ) 
            {

			    this.R = this.G = this.B = l;

		    } 
            else 
            {

			    var p = l <= 0.5 ? l * ( 1 + s ) : l + s - ( l * s );
			    var q = ( 2 * l ) - p;

			    this.R = Hue2RGB( q, p, h + 1.0f / 3.0f );
			    this.G = Hue2RGB( q, p, h );
			    this.B = Hue2RGB( q, p, h - 1.0f / 3.0f );

		    }

		    return this;
	    }
        public Color SetColorName(ColorKeywords style ) {

		    // color keywords
		    var hex = (int)style;

            return SetHex(hex);		   
	    }

        public static Color ColorName(ColorKeywords hex)
        {
            return new Color().SetColorName(hex);
        }

        public Color Copy(Color other)
        {
            this.R = other.R;
            this.G = other.G;
            this.B = other.B;

            return this;
        }
        public Color CopyGammaToLinear(Color color, float? gammaFactor ) 
        {

		    if ( gammaFactor == null ) gammaFactor = 2.0f;

		    this.R = (float)Math.Pow( color.R, gammaFactor.Value );
		    this.G =  (float)Math.Pow( color.G, gammaFactor.Value );
		    this.B =  (float)Math.Pow( color.B, gammaFactor.Value );

		    return this;

	    }

	    public Color CopyLinearToGamma(Color color, float? gammaFactor )
        {

		    if ( gammaFactor == null ) gammaFactor = 2.0f;

		    var safeInverse = ( gammaFactor > 0 ) ? ( 1.0f / gammaFactor ) : 1.0f;

		    this.R = (float)Math.Pow( color.R, (float)safeInverse );
		    this.G = (float)Math.Pow( color.G, (float)safeInverse );
		    this.B = (float)Math.Pow( color.B, (float)safeInverse );

		    return this;

	    }

	    public Color ConvertGammaToLinear(float? gammaFactor)
        {

		    return this.CopyGammaToLinear( this, gammaFactor );

	    }

	    public Color ConvertLinearToGamma(float? gammaFactor ) {

		    this.CopyLinearToGamma( this, gammaFactor );

		    return this;

	    }

	    public Color CopySRGBToLinear(Color color ) 
        {

		    this.R = SRGBToLinear( color.R );
		    this.G = SRGBToLinear( color.G );
		    this.B = SRGBToLinear( color.B );

		    return this;

	    }

	    public Color CopyLinearToSRGB(Color color ) 
        {

		    this.R = LinearToSRGB( color.R );
		    this.G = LinearToSRGB( color.G );
		    this.B = LinearToSRGB( color.B );

		    return this;
	    }

	    public Color ConvertSRGBToLinear() 
        {

		    this.CopySRGBToLinear( this );

		    return this;

	    }

	    public Color ConvertLinearToSRGB() 
        {

		    this.CopyLinearToSRGB( this );

		    return this;

	    }

	    public int GetHex() 
        {

		    return (int)( this.R * 255 ) << 16 ^ (int)( this.G * 255 ) << 8 ^ (int)( this.B * 255 ) << 0;

	    }

	    public string GetHexString() 
        {
            var id =GetHex();
            
		    return "#"+id.ToString("X6");

	    }

	    public HSL GetHSL() 
        {

		// h,s,l ranges are in 0.0 - 1.0

		    var r = this.R;
            var g = this.G;
            var b = this.B;

		    var max = Math.Max( r, Math.Max(g, b ));
		    var min = Math.Min( r, Math.Min(g, b ));

		    float hue=0, saturation=0;
		    float lightness = ( min + max ) / 2.0f;

		    if ( min == max ) 
            {

			    hue = 0;
			    saturation = 0;

		    } 
            else 
            {

			    var delta = max - min;

			    saturation = lightness <= 0.5 ? delta / ( max + min ) : delta / ( 2 - max - min );

                if(max==r)
                {
                    hue = ( g - b ) / delta + ( g < b ? 6 : 0 );
                }
                else if(max==g)
                {
                     hue = ( b - r ) / delta + 2;
                }
                else if(max==b)
                {
                    hue = ( r - g ) / delta + 4;
                }			

			    hue /= 6;

		    }

            HSL target = new HSL();
		    target.H = hue;
		    target.S = saturation;
		    target.L = lightness;

		    return target;

        }

        public Color OffsetHSL(float h, float s, float l ) 
        {

            HSL _hslA = this.GetHSL();

		    this.GetHSL();

		    _hslA.H += h; _hslA.S += s; _hslA.L+= l;

		    this.SetHSL( _hslA.H, _hslA.S, _hslA.L );

		    return this;
	    }

	    public Color Add(Color color ) 
        {

		    this.R += color.R;
		    this.G += color.G;
		    this.B += color.B;

		    return this;

	    }

	    public Color AddColors(Color color1, Color color2 ) {

		    this.R = color1.R + color2.R;
		    this.G = color1.G + color2.G;
		    this.B = color1.B + color2.B;

		    return this;

	    }

	    public Color AddScalar(float s ) {

		    this.R += s;
		    this.G += s;
		    this.B += s;

		    return this;

	    }

	    public Color Sub(Color color ) {

		    this.R = Math.Max( 0, this.R - color.R );
		    this.G = Math.Max( 0, this.G - color.G );
		    this.B = Math.Max( 0, this.B - color.B );

		    return this;

	    }

        public static Color operator -(Color left,Color right)
        {
            Color result = new Color();
            result.R = Math.Max( 0, left.R - right.R );
		    result.G = Math.Max( 0, left.G - right.G );
		    result.B = Math.Max( 0, left.B - right.B );

            return result;
        }

	    public Color Multiply(Color color ) {

		    this.R *= color.R;
		    this.G *= color.G;
		    this.B *= color.B;

		    return this;

	    }

        public static Color operator *(Color left,Color right)
        {
            Color result = new Color();

            result.R = left.R * right.R;
		    result.G = left.G * right.G;
		    result.B = left.B * right.B;

            return result;
        }

	    public Color MultiplyScalar(float s )
        {

		    this.R *= s;
		    this.G *= s;
		    this.B *= s;

		    return this;

	    }

        public static Color operator *(Color left,float s)
        {
            Color result = new Color();

            result.R = left.R * s;
		    result.G = left.G * s;
		    result.B = left.B * s;
            
            return result;
        }

	    public Color Lerp(Color color, float alpha ) 
        {

		    this.R += ( color.R - this.R ) * alpha;
		    this.G += ( color.G - this.G ) * alpha;
		    this.B += ( color.B - this.B ) * alpha;

		    return this;

	    }

	    public Color LerpHSL(Color color, float alpha ) 
        {

		    HSL _hslA = this.GetHSL();
		    HSL _hslB = color.GetHSL();
            

		    var h = _hslA.H.Lerp(_hslB.H, alpha );
		    var s = _hslA.S.Lerp(_hslB.S, alpha );
		    var l = _hslA.L.Lerp(_hslB.L, alpha );

		    this.SetHSL( h, s, l );

		    return this;

	    }

	    public bool Equals(Color c ) 
        {

		    return ( c.R == this.R ) && ( c.G == this.G ) && ( c.B == this.B );

	    }

	    public Color FromArray(float[] array, int? offset=null ) 
        {

		    if ( offset == null ) offset = 0;

            int index = offset.Value;
		    this.R = array[ index ];
		    this.G = array[ index + 1 ];
		    this.B = array[ index + 2 ];

		    return this;

	    }

	    public float[] ToArray(float[] array, int? offset ) {

		    if ( array == null ) array = new float[3];
		    if ( offset == null ) offset = 0;

		    array[ (int)offset ] = this.R;
		    array[ (int)offset + 1 ] = this.G;
		    array[ (int)offset + 2 ] = this.B;

		    return array;

	    }
        
    }
}

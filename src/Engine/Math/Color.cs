namespace BlinkLab.Engine.Math;

public readonly struct Color
{
	public Color(float red, float green, float blue, float alpha)
	{
		R = System.Math.Clamp(red, 0, 1);
		G = System.Math.Clamp(green, 0, 1); ;
		B = System.Math.Clamp(blue, 0, 1); ;
		A = System.Math.Clamp(alpha, 0, 1); ;
	}

		public Color(float red, float green, float blue)
	: this(red, green, blue, 1) { }

	public Color(float grayscale, float alpha)
	: this(grayscale, grayscale, grayscale, alpha) { }
	public Color(float grayscale)
	: this(grayscale, 1) { }

	public Color(byte red, byte green, byte blue, byte alpha)
	: this(red / byte.MaxValue, green / byte.MaxValue, blue / byte.MaxValue, alpha / byte.MaxValue) { }
	public Color(byte red, byte green, byte blue)
	: this(red, green, blue, byte.MaxValue) { }

	public readonly float R;
	public readonly float G;
	public readonly float B;
	public readonly float A;

	public static Color Lerp(Color a, Color b, float t)
	=> new(
		a.R + (b.R - a.R) * t,
		a.G + (b.G - a.G) * t,
		a.B + (b.B - a.B) * t,
		a.A + (b.A - a.A) * t
	);

	public static Color Transparent => new(0f, 0f, 0f, 0f);

	public static Color Red => new(1f, 0f, 0f);
	public static Color Blue => new(0f, 1f, 0f);
	public static Color Green => new(0f, 0f, 1f);

	public static Color DarkRed => new(0.5f, 0f, 0f);
	public static Color DarkBlue => new(0f, 0.5f, 0f);
	public static Color DarkGreen => new(0f, 0f, 0.5f);

	public static Color Black => new(0f, 0f, 0f);
	public static Color White => new(1f, 1f, 1f);
	public static Color Gray => new(0.5f, 0.5f, 0.5f);

	public static Color Yellow => new(1f, 1f, 0f);
	public static Color Cyan => new(0f, 1f, 1f);
	public static Color Magenta => new(1f, 0f, 1f);

	public static Color Orange => new(1f, 0.5f, 0f);
	public static Color Emerald => new(0f, 1f, 0.5f);
	public static Color LightBlue => new(0f, 0.5f, 1f);
	public static Color VividMagenta => new(0.5f, 0f, 1f);
}

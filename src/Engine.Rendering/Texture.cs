using OpenTK.Graphics.OpenGL;
using StbImageSharp;

namespace BlinkLab.Engine.Rendering;

public class Texture : IDisposable
{
	public int Handle { get; private set; }
	private bool disposed;

	private Texture(byte[] bytes)
	{
		ImageResult image;
		using var str = new MemoryStream(bytes);
		image = ImageResult.FromStream(str, ColorComponents.RedGreenBlueAlpha);

		Handle = GL.GenTexture();
		GL.BindTexture(TextureTarget.Texture2D, Handle);

		GL.TexImage2D(
			TextureTarget.Texture2D,
			0,
			PixelInternalFormat.Rgba,
			image.Width,
			image.Height,
			0,
			PixelFormat.Rgba,
			PixelType.UnsignedByte,
			image.Data
		);

		GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
		GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

		GL.BindTexture(TextureTarget.Texture2D, 0);

		ErrorHandler.CatchError();
	}

	public static Texture LoadTexture(string path)
	{
		byte[] imageBytes = File.ReadAllBytes(path);
		return new(imageBytes);
	}

	public void Dispose()
	{
		if (disposed) return;

		GL.DeleteTexture(Handle);
		Handle = 0;
		GC.SuppressFinalize(this);
		disposed = true;
	}

	~Texture()
	{
		Dispose();
	}
}

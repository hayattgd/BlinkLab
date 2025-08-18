using OpenTK.Graphics.OpenGL;
using StbImageSharp;

namespace BlinkLab.Engine.Rendering;

public class Texture : IDisposable
{
	public Texture(string path)
	{
		byte[] imageBytes = File.ReadAllBytes(path);

		ImageResult image;
		using var str = new MemoryStream(imageBytes);
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

		GL.BindTexture(TextureTarget.Texture2D, 0);
	}

	~Texture()
	{
		Dispose();
	}

	public int Handle { get; private set; }
	private bool disposed;

	public void Dispose()
	{
		if (disposed) return;

		GL.DeleteTexture(Handle);
		Handle = 0;
		GC.SuppressFinalize(this);
		disposed = true;
	}
}

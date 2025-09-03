using OpenTK.Graphics.OpenGL;

namespace BlinkLab.Engine.Rendering;

public static class ErrorHandler
{
	public static void CatchGLError()
	{
		var err = GL.GetError();
		if (err != ErrorCode.NoError) { Console.WriteLine($"OpenGL Error: {err}"); }
	}
}

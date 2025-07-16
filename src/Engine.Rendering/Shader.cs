using OpenTK.Graphics.OpenGL;

namespace BlinkLab.Engine.Rendering;

public class Shader : IDisposable
{
	public int Handle { get; private set; }

	public Shader(string vertexsrc, string fragmentsrc)
	{
		int vertexshader = CompileShader(ShaderType.VertexShader, vertexsrc);
		int fragmentshader = CompileShader(ShaderType.FragmentShader, fragmentsrc);

		Handle = GL.CreateProgram();
		GL.AttachShader(Handle, vertexshader);
		GL.AttachShader(Handle, fragmentshader);
		GL.LinkProgram(Handle);

		GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out int status);
		if (status != 1)
		{
			throw new Exception($"({status}) Shader link error: {GL.GetProgramInfoLog(Handle)}");
		}

		GL.DetachShader(Handle, vertexshader);
		GL.DetachShader(Handle, fragmentshader);
		GL.DeleteShader(vertexshader);
		GL.DeleteShader(fragmentshader);
	}

	public int CompileShader(ShaderType type, string src)
	{
		int shader = GL.CreateShader(type);
		GL.ShaderSource(shader, src);
		GL.CompileShader(shader);
		GL.GetShader(shader, ShaderParameter.CompileStatus, out int status);
		if (status != (int)All.True)
		{
			throw new Exception($"({status}) Shader compile error: {GL.GetShaderInfoLog(shader)}");
		}

		return shader;
	}

	public void Use() => GL.UseProgram(Handle);

	public void Dispose()
	{
		GL.DeleteProgram(Handle);
	}
}

using System.Reflection;
using BlinkLab.Engine.Rendering;
using ImGuiNET;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace BlinkLab.Editor.UI;

public class ImGuiController : IDisposable
{
	private int _vertexArray;
	private int _vertexBuffer;
	private int _indexBuffer;

	private int _vertexBufferSize = 10000;
	private int _indexBufferSize = 2000;

	private int _fontTex;

	private Shader _shader;
	private int _projMatrixLocation;
	private int _texLocation;

	public GameWindow window { get; private set; }

	public ImGuiController(GameWindow window)
	{
		nint ctx = ImGui.CreateContext();
		ImGui.SetCurrentContext(ctx);
		ImGui.StyleColorsDark();

		var io = ImGui.GetIO();

		io.ConfigFlags |= ImGuiConfigFlags.DockingEnable;
		io.ConfigDockingAlwaysTabBar = true;

		this.window = window;
		window.MouseWheel += (e) =>
		{
			io.MouseWheel = e.OffsetY;
		};
		window.TextInput += (e) =>
		{
			io.AddInputCharactersUTF8(e.AsString);
		};
		window.KeyDown += (e) =>
		{
			io.AddKeyEvent(OpenTKToImGuiKey(e.Key), true);
		};
		window.KeyUp += (e) =>
		{
			io.AddKeyEvent(OpenTKToImGuiKey(e.Key), false);
		};

		io.Fonts.AddFontFromFileTTF("/usr/share/fonts/TTF/OpenSans-Regular.ttf", 20);
		io.Fonts.AddFontDefault();
		io.Fonts.Build();

		io.Fonts.GetTexDataAsRGBA32(out IntPtr pixels, out int width, out int height, out _);

		_fontTex = GL.GenTexture();
		GL.BindTexture(TextureTarget.Texture2D, _fontTex);
		GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, pixels);
		GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
		GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Linear);

		io.Fonts.SetTexID((IntPtr)_fontTex);
		GL.BindTexture(TextureTarget.Texture2D, 0);

		//Setup resources
		_vertexArray = GL.GenVertexArray();
		GL.BindVertexArray(_vertexArray);

		_vertexBuffer = GL.GenBuffer();
		GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBuffer);
		GL.BufferData(BufferTarget.ArrayBuffer, _vertexBufferSize, IntPtr.Zero, BufferUsageHint.DynamicDraw);

		_indexBuffer = GL.GenBuffer();
		GL.BindBuffer(BufferTarget.ElementArrayBuffer, _indexBuffer);
		GL.BufferData(BufferTarget.ArrayBuffer, _indexBufferSize, IntPtr.Zero, BufferUsageHint.DynamicDraw);

		GL.EnableVertexAttribArray(0);
		GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 20, 0);

		GL.EnableVertexAttribArray(1);
		GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 20, 8);

		GL.EnableVertexAttribArray(2);
		GL.VertexAttribPointer(2, 4, VertexAttribPointerType.UnsignedByte, true, 20, 16);

		GL.BindVertexArray(0);

		string shaderpath = Path.Combine(Application.ResourcePath, "Shaders");

		string vertshader = File.ReadAllText(Path.Combine(shaderpath, "basic.vert"));
		string fragshader = File.ReadAllText(Path.Combine(shaderpath, "basic.frag"));

		_shader = new Shader(vertshader, fragshader);
		_projMatrixLocation = GL.GetUniformLocation(_shader.Handle, "uProjection");
		_texLocation = GL.GetUniformLocation(_shader.Handle, "uTexture");
	}

	public ImGuiKey OpenTKToImGuiKey(Keys key)
	{
		ImGuiKey imguikey;
		if (Enum.TryParse(key.ToString(), out imguikey))
		{

		}
		else
		{
			imguikey = key switch
			{
				Keys.Right => ImGuiKey.RightArrow,
				Keys.Left => ImGuiKey.LeftArrow,
				Keys.Up => ImGuiKey.UpArrow,
				Keys.Down => ImGuiKey.DownArrow,
				Keys.LeftControl => ImGuiKey.LeftCtrl,
				Keys.RightControl => ImGuiKey.RightCtrl,
				_ => ImGuiKey.None
			};
		}

		return imguikey;
	}

	public void Update(float dt)
	{
		var io = ImGui.GetIO();
		io.DisplaySize = new(window.Size.X, window.Size.Y);
		io.DeltaTime = dt > 0.0f ? dt : 1.0f / 60.0f;

		io.AddKeyEvent(ImGuiKey.ModCtrl, window.KeyboardState.IsKeyDown(Keys.LeftControl) || window.KeyboardState.IsKeyDown(Keys.RightControl));
		io.AddKeyEvent(ImGuiKey.ModAlt, window.KeyboardState.IsKeyDown(Keys.LeftAlt) || window.KeyboardState.IsKeyDown(Keys.RightAlt));
		io.AddKeyEvent(ImGuiKey.ModShift, window.KeyboardState.IsKeyDown(Keys.LeftShift) || window.KeyboardState.IsKeyDown(Keys.RightShift));
		io.AddKeyEvent(ImGuiKey.ModSuper, window.KeyboardState.IsKeyDown(Keys.LeftSuper) || window.KeyboardState.IsKeyDown(Keys.RightSuper));

		io.MouseDown[0] = window.IsMouseButtonDown(MouseButton.Left);
		io.MouseDown[1] = window.IsMouseButtonDown(MouseButton.Right);
		io.MouseDown[2] = window.IsMouseButtonDown(MouseButton.Middle);

		io.MousePos = new(window.MousePosition.X, window.MousePosition.Y);

		CursorShape shape = ImGui.GetMouseCursor() switch
		{
			ImGuiMouseCursor.TextInput => CursorShape.IBeam,
			ImGuiMouseCursor.ResizeAll => CursorShape.ResizeAll,
			ImGuiMouseCursor.ResizeNS => CursorShape.ResizeNS,
			ImGuiMouseCursor.ResizeEW => CursorShape.ResizeEW,
			ImGuiMouseCursor.ResizeNESW => CursorShape.ResizeNESW,
			ImGuiMouseCursor.ResizeNWSE => CursorShape.ResizeNWSE,
			ImGuiMouseCursor.Hand => CursorShape.PointingHand,
			ImGuiMouseCursor.NotAllowed => CursorShape.NotAllowed,
			_ => CursorShape.Arrow
		};

		unsafe
		{
			var cursor = GLFW.CreateStandardCursor(shape);
			GLFW.SetCursor(window.WindowPtr, cursor);
		}

		ImGui.NewFrame();
	}

	public void Render()
	{
		ImGui.Render();
		RenderDrawData(ImGui.GetDrawData());
	}

	private unsafe void RenderDrawData(ImDrawDataPtr drawdata)
	{
		var io = ImGui.GetIO();
		drawdata.ScaleClipRects(io.DisplayFramebufferScale);

		GL.Enable(EnableCap.Blend);
		GL.BlendEquation(BlendEquationMode.FuncAdd);
		GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
		GL.Disable(EnableCap.CullFace);
		GL.Disable(EnableCap.DepthTest);
		GL.Enable(EnableCap.ScissorTest);

		_shader.Use();
		GL.Uniform1(_texLocation, 0);
		GL.BindVertexArray(_vertexArray);

		Matrix4 projection = Matrix4.CreateOrthographicOffCenter(
			0f, io.DisplaySize.X,
			io.DisplaySize.Y, 0,
			-1, 1
		);
		GL.UniformMatrix4(_projMatrixLocation, false, ref projection);

		for (int n = 0; n < drawdata.CmdListsCount; n++)
		{
			var cmdlist = drawdata.CmdLists[n];

			GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBuffer);
			GL.BufferData(BufferTarget.ArrayBuffer, cmdlist.VtxBuffer.Size * sizeof(ImDrawVert), cmdlist.VtxBuffer.Data, BufferUsageHint.StreamDraw);

			GL.BindBuffer(BufferTarget.ElementArrayBuffer, _indexBuffer);
			GL.BufferData(BufferTarget.ElementArrayBuffer, cmdlist.IdxBuffer.Size * sizeof(ushort), cmdlist.IdxBuffer.Data, BufferUsageHint.StreamDraw);

			int idxoffset = 0;

			for (int cmdI = 0; cmdI < cmdlist.CmdBuffer.Size; cmdI++)
			{
				var drawcmd = cmdlist.CmdBuffer[cmdI];

				GL.ActiveTexture(TextureUnit.Texture0);
				GL.BindTexture(TextureTarget.Texture2D, (int)drawcmd.TextureId);
				GL.Scissor(
					(int)drawcmd.ClipRect.X,
					(int)(io.DisplaySize.Y - drawcmd.ClipRect.W),
					(int)(drawcmd.ClipRect.Z - drawcmd.ClipRect.X),
					(int)(drawcmd.ClipRect.W - drawcmd.ClipRect.Y)
				);

				GL.DrawElementsBaseVertex(
					PrimitiveType.Triangles,
					(int)drawcmd.ElemCount,
					DrawElementsType.UnsignedShort,
					(IntPtr)(idxoffset * sizeof(ushort)),
					(int)drawcmd.VtxOffset
				);

				idxoffset += (int)drawcmd.ElemCount;
			}
		}

		GL.Disable(EnableCap.ScissorTest);
	}

	public void Dispose()
	{
		GL.DeleteBuffer(_vertexBuffer);
		GL.DeleteBuffer(_indexBuffer);
		GL.DeleteTexture(_fontTex);
	}
}

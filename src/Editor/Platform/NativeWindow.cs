using BlinkLab.Editor.UI;
using BlinkLab.Engine.Rendering;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace BlinkLab.Editor.Platform;

public class NativeWindow : GameWindow
{
	ImGuiController imGuiController;

	public NativeWindow()
	: base(GameWindowSettings.Default, new NativeWindowSettings()
	{
		Title = "BlinkLab"
	})
	{
		imGuiController = new(this);
	}

	protected override void OnLoad()
	{
		base.OnLoad();
		GL.ClearColor(0.15f, 0.15f, 0.15f, 1f);
	}

	protected override void OnRenderFrame(FrameEventArgs args)
	{
		base.OnRenderFrame(args);
		GL.Viewport(0, 0, Size.X, Size.Y);
		GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

		imGuiController.Update((float)args.Time);

		Application.Update(args);

		imGuiController.Render();
		SwapBuffers();

		ErrorHandler.CatchError();
	}

	public override void Dispose()
	{
		base.Dispose();
		imGuiController.Dispose();
	}
}

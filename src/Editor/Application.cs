using System.Reflection;
using BlinkLab.Engine.Debug;
using BlinkLab.Engine;
using BlinkLab.Editor.Platform;
using OpenTK.Windowing.Common;
using BlinkLab.Editor.UI;

namespace BlinkLab.Editor;

public static class Application
{
	public static string BasePath => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? throw new Exception("Couldn't get executable path");
	public static string ResourcePath => Path.Combine(BasePath, "res");
	public readonly static Logger logger = new("Editor");

	public static Project? Project { get; private set; }
	private static readonly NativeWindow window = new();
	internal static readonly UIManager uiManager = new();
	private static readonly MenuBar menuBar = new();

	public static void Main()
	{
		window.Run();
		Quit();
	}

	public static void Update(FrameEventArgs args)
	{
		uiManager.Update();
		menuBar.Draw();
	}

	public static void Quit()
	{
		window.Close();
		window.Dispose();
		Environment.Exit(0);
	}
}

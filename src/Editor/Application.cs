using System.Reflection;
using BlinkLab.Engine.Debug;
using BlinkLab.Engine;
using BlinkLab.Editor.Platform;
using OpenTK.Windowing.Common;
using BlinkLab.Editor.UI;
using System.Text.Json;

namespace BlinkLab.Editor;

public static class Application
{
	public struct Config
	{
		public enum Theme
		{
			Dark,
			Light
		}

		public Theme theme;
		public string fontPath;
		public int fontSize;

		public string newProjectPath;
	}

	public static string BasePath => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? throw new Exception("Couldn't get executable path");

	public static string ConfigPath => Path.Combine(BasePath, "config.json");
	public static string ResourcePath => Path.Combine(BasePath, "res");

	public static Config Configuration { get; internal set; }

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

	public static string SaveConfig()
	{
		string content = JsonSerializer.Serialize(Configuration);
		File.WriteAllText(ConfigPath, content);
		return content;
	}

	public static void LoadConfig()
	{
		string json;

		if (!File.Exists(ConfigPath))
		{
			File.Create(ConfigPath).Dispose();
			json = SaveConfig();
		}
		else
		{
			json = File.ReadAllText(ConfigPath);
		}

		try
		{
			Configuration = JsonSerializer.Deserialize<Config>(json);
		}
		catch (JsonException ex)
		{
			logger.Error(ex.Message);
			Configuration = new();
		}
	}

	public static void Update(FrameEventArgs args)
	{
		uiManager.Update();
		menuBar.Draw();
	}

	public static void LoadProject(string path)
	{
		Project = new(path);
	}

	public static void Quit()
	{
		window.Close();
		window.Dispose();
		Environment.Exit(0);
	}
}

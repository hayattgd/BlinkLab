using System.Reflection;
using System.Runtime.InteropServices;
using BlinkLab.Engine.Debug;
using BlinkLab.Engine;
using BlinkLab.Editor.Platform;
using OpenTK.Windowing.Common;
using BlinkLab.Editor.UI;
using System.Text.Json;

namespace BlinkLab.Editor;

public static class Application
{
	[Serializable]
	public struct Config
	{
		public Config()
		{
			theme = Theme.Dark;
			fontPath = DefaultFont;
			fontSize = 20;
			newProjectPath = HomeDirectory;
			loadProjectPath = HomeDirectory;
			editorCommandLine = "code";
		}

		public enum Theme
		{
			Dark,
			Light
		}

		public Theme theme { get; set; }
		public string fontPath { get; set; }
		public int fontSize { get; set; }

		public string newProjectPath { get; set; }
		public string loadProjectPath { get; set; }

		public string editorCommandLine { get; set; }

		public void UpdateNewProjectPath(string path)
		{
			newProjectPath = path;
		}

		public void UpdateLoadProjectPath(string path)
		{
			loadProjectPath = path;
		}
	}

	public static string DefaultFont
	{
		get
		{
			if (File.Exists(Path.Combine(FontsPath, "OpenSans-Regular.ttf")))
			{
				return Path.Combine(FontsPath, "OpenSans-Regular.ttf");
			}
			return Path.Combine(FontsPath, "Arial.ttf");
		}
	}

	public static string FontsPath
	{
		get
		{
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				return "C:/Windows/Fonts/";
			}
			else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
			{
				return "/System/Library/Fonts/Supplemental/";
			}
			else
			{
				return "/usr/share/fonts/TTF/";
			}
		}
	}

	public static string ProjectPath => Project?.path ?? HomeDirectory;
	public static string HomeDirectory => Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
	public static string BasePath => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? throw new Exception("Couldn't get executable path");

	public static string ConfigPath => Path.Combine(BasePath, "config.json");
	public static string ResourcePath => Path.Combine(BasePath, "res");

	public static Config Configuration { get; internal set; }

	public readonly static Logger logger = new("Editor");

	public static Project? Project { get; private set; }
	public static bool IsProjectLoaded => Project != null;
	private static NativeWindow? window;
	internal static readonly UIManager uiManager = new();
	private static readonly MenuBar menuBar = new();

	public static void Main()
	{
		LoadConfig();
		window = new();
		window?.Run();
		Quit();
	}

	public static void SaveConfig()
	{
		string content = JsonSerializer.Serialize(Configuration);
		File.WriteAllText(ConfigPath, content);
	}

	public static void LoadConfig()
	{
		string json;

		if (!File.Exists(ConfigPath))
		{
			File.Create(ConfigPath).Dispose();
			Configuration = new();
			SaveConfig();
			return;
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
		window?.Close();
		window?.Dispose();
		SaveConfig();
		Environment.Exit(0);
	}
}

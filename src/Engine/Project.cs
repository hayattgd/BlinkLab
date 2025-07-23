using System.Diagnostics;
using BlinkLab.Engine.Debug;
using BlinkLab.Engine.World;

namespace BlinkLab.Engine;

public class Project
{
	public Project(string path)
	{
		this.path = path;
	}

	public struct ProjectConfig
	{
		public string name;
	}

	public static async Task SetupProject(ProjectConfig config, string UserScriptcsprojPath, string EnginedllPath, Logger logger, string path)
	{
		try
		{
			logger.Info($"Name: {config.name}");
			logger.Info($"Path: {path}");

			string projectpath = path;
			Directory.CreateDirectory(projectpath);
			Directory.CreateDirectory(Path.Combine(projectpath, "Complied"));
			Directory.CreateDirectory(Path.Combine(projectpath, "Complied", "Assembly"));
			Directory.CreateDirectory(Path.Combine(projectpath, "Complied", "Builds"));
			Directory.CreateDirectory(Path.Combine(projectpath, "Project"));

			logger.Info("Directory configured.");
			logger.Info("Running dotnet CLI...");

			logger.Info("Copying UserScript.csproj...");
			string UserScript = await File.ReadAllTextAsync(UserScriptcsprojPath);
			UserScript = UserScript.Replace("ENGINE_PATH", EnginedllPath);
			File.WriteAllText(Path.Combine(projectpath, "UserScript.csproj"), UserScript);

			logger.Info("Configuring solution...");
			await RunDotnetCLI($"new sln -n \"{config.name}\"", logger, projectpath);
			await RunDotnetCLI($"sln add ./UserScript.csproj", logger, projectpath);

			logger.Info("New project created.");
		}
		catch (Exception ex)
		{
			logger.Error(ex.ToString());
		}
	}

	private static async Task RunDotnetCLI(string arguments, Logger logger, string path)
	{
		logger.Info($"> dotnet {arguments}");
		using var proc = new Process
		{
			StartInfo = new ProcessStartInfo
			{
				FileName = "dotnet",
				Arguments = arguments,
				WorkingDirectory = path,
				UseShellExecute = false,
				CreateNoWindow = true,
				RedirectStandardOutput = true,
				RedirectStandardError = true
			}
		};
		proc.OutputDataReceived += (sender, args) => logger.Info(args.Data ?? "");
		proc.ErrorDataReceived += (sender, args) => logger.Error(args.Data ?? "");
		proc.Start();

		proc.BeginOutputReadLine();
		proc.BeginErrorReadLine();

		await proc.WaitForExitAsync();
		if (proc.ExitCode != 0) { throw new Exception($"Execution of dotnet CLI failed ({proc.ExitCode})"); }
	}

	public string path;

	private readonly Entity _root = new("root");
	public Entity Root => _root;
	public readonly static Logger logger = new("Player");
}

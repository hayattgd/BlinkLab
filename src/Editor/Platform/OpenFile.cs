using System.Diagnostics;
using System.Runtime.InteropServices;

namespace BlinkLab.Editor.Platform;

public static class NativeApplication
{
	public static List<string> CodeExtensions =>
	[
		"cs",
		"md",
		"sln",
		"csproj",
		"frag",
		"vert",
		"txt",
		"gitignore",
		"editorconfig",
		"json",
		"yml",
		"sh"
	];

	public static void OpenFile(string path)
	{
		if (CodeExtensions.Any((x) => x == path.Split('.')[^1]))
		{
			Process.Start(new ProcessStartInfo()
			{
				FileName = Application.Configuration.editorCommandLine,
				Arguments = path,
				UseShellExecute = false
			});
		}
		else
		{
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				Process.Start(new ProcessStartInfo()
				{
					FileName = "start",
					Arguments = path,
					UseShellExecute = false
				});
			}
			else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
			{
				Process.Start(new ProcessStartInfo()
				{
					FileName = "open",
					Arguments = path,
					UseShellExecute = false
				});
			}
			else
			{
				Process.Start(new ProcessStartInfo()
				{
					FileName = "xdg-open",
					Arguments = path,
					UseShellExecute = false
				});
			}
		}
	}
}

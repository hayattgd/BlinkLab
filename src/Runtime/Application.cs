using System.Reflection;

namespace BlinkLab.Editor;

public static class Application
{
	public static string BasePath => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? throw new Exception("Couldn't get executable path.");
	public static string ResourcePath => Path.Combine(BasePath, "res");

	public static void Main()
	{
		Console.WriteLine("Hello World!");
	}
}

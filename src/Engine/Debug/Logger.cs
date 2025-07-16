using System.Collections.Immutable;
using BlinkLab.Engine.Math;

namespace BlinkLab.Engine.Debug;

public class Logger
{
	public Logger(string name)
	{
		this.name = name;
#if DEBUG
		filter = LogLevel.Debug;
#else
		filter = LogLevel.Info;
#endif
	}

	public readonly struct Log
	{
		public Log(string text, string prefix, LogLevel level)
		{
			str = text;
			this.prefix = prefix;
			this.level = level;
		}

		public readonly LogLevel level;
		public readonly string prefix;
		public readonly string timestamp = DateTime.Now.ToString("HH:mm:ss");
		private readonly string str;

		public string Text => $"[{timestamp}][{level}][{prefix}] {str}";
		public string Ttytext => $"{escapecode}{Text}";

		public Color color => level switch
		{
			LogLevel.Debug => Color.Gray,
			LogLevel.Warning => Color.Yellow,
			LogLevel.Error => Color.Red,
			LogLevel.Fatal => Color.Magenta,
			_ => Color.White
		};

		public string escapecode => level switch
		{
			LogLevel.Debug => "\x1b[90m",
			LogLevel.Warning => "\x1b[90m",
			LogLevel.Error => "\x1b[31m",
			LogLevel.Fatal => "\x1b[35m",
			_ => "\x1b[0m"
		};
	}

	public enum LogLevel
	{
		Debug,
		Info,
		Warning,
		Error,
		Fatal
	}

	public readonly string name;

	private List<Log> logs = [];
	public ImmutableList<Log> Logs => logs.ToImmutableList();
	public LogLevel filter;

	public void AddLog(string text, LogLevel level)
	{
		if (string.IsNullOrEmpty(text)) { return; }
		var log = new Log(text, name, level);

		logs.Add(log);
		if ((int)level < (int)filter) { return; }
		Console.WriteLine(log.Ttytext);
	}

	public void Debug(string text) => AddLog(text, LogLevel.Debug);
	public void Info(string text) => AddLog(text, LogLevel.Info);
	public void Warning(string text) => AddLog(text, LogLevel.Warning);
	public void Error(string text) => AddLog(text, LogLevel.Error);
	public void Fatal(string text) => AddLog(text, LogLevel.Fatal);

	public void Clear()
	{
		logs.Clear();
	}
}

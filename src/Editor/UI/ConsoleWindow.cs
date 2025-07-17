using BlinkLab.Engine.Debug;
using ImGuiNET;

namespace BlinkLab.Editor.UI;

public class ConsoleWindow : EditorWindow
{
	public ConsoleWindow(Logger logger)
	{
		Title = $"{logger.name} Console";
		this.logger = logger;
	}

	public Logger logger;

	public override void Draw()
	{
		bool open = true;

		ImGui.Begin(Title, ref open);
		if (ImGui.SmallButton("Clear"))
		{
			logger.Clear();
		}

		ImGui.BeginChild("console", new(0, 0), ImGuiChildFlags.AlwaysUseWindowPadding | ImGuiChildFlags.Borders);
		foreach (var log in logger.Logs)
		{
			ImGui.TextColored(new(log.color.R, log.color.G, log.color.B, log.color.A), log.Text);
		}
		ImGui.EndChild();
		ImGui.End();

		if (!open) { Close(); }
	}
}

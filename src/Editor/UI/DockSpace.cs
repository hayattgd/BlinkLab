using ImGuiNET;

namespace BlinkLab.Editor.UI;

public static class DockSpace
{
	public static void Draw()
	{
		var size = ImGui.GetIO().DisplaySize - new System.Numerics.Vector2(0, 25);
		ImGui.SetNextWindowSize(size);
		ImGui.SetNextWindowBgAlpha(0);
		ImGui.SetNextWindowPos(new(0, 25));
		ImGui.PushStyleVarX(ImGuiStyleVar.WindowPadding, 0);
		ImGui.PushStyleVarY(ImGuiStyleVar.WindowPadding, 0);
		ImGui.Begin("DockSpace", ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoFocusOnAppearing | ImGuiWindowFlags.NoBringToFrontOnFocus | ImGuiWindowFlags.NoDocking);
		ImGui.DockSpace(1, size, ImGuiDockNodeFlags.PassthruCentralNode);
		ImGui.End();
		ImGui.PopStyleVar();
		ImGui.PopStyleVar();
	}
}

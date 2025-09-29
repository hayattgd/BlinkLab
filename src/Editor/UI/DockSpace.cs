using ImGuiNET;

namespace BlinkLab.Editor.UI;

public static class DockSpace
{
	public static void Draw()
	{
		ImGui.SetNextWindowSize(ImGui.GetIO().DisplaySize - new System.Numerics.Vector2(0, 25));
		ImGui.SetNextWindowBgAlpha(0);
		ImGui.SetNextWindowPos(new(0, 25));
		// ImGui.Begin("DockSpace");
		ImGui.Begin("DockSpace", ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoFocusOnAppearing);
		ImGui.End();
	}
}

using ImGuiNET;

namespace BlinkLab.Editor.UI;

public class AssetsWindow : EditorWindow
{
	public AssetsWindow()
	{
		Title = $"Assets";
	}

	public override void Draw()
	{
		var open = true;

		ImGui.Begin(Title, ref open);

		for (int i = 0; i < 200; i++)
		{
			var cursor = ImGui.GetCursorPos();
			ImGui.Image(ImGui.GetIO().Fonts.TexID, new(75, 62));
			ImGui.SetCursorPos(cursor + new System.Numerics.Vector2(0, 55));
			ImGui.Text($"{i}");
			ImGui.SetCursorPos(cursor);
			ImGui.Selectable($"##Asset{i}", false, ImGuiSelectableFlags.AllowOverlap, new(75, 75));
			var spacesAvailable = ImGui.GetContentRegionAvail().X - cursor.X;
			if (spacesAvailable > 155)
			{
				ImGui.SameLine();
			}
		}

		ImGui.End();

		if (!open) { Close(); }
	}
}

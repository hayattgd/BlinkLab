using BlinkLab.Engine.Rendering;
using ImGuiNET;

namespace BlinkLab.Editor.UI;

public class AssetsWindow : EditorWindow
{
	private static Texture? FileIcon;
	private static Texture? FolderIcon;
	private static Texture? ScriptIcon;

	public AssetsWindow()
	{
		Title = $"Assets";
	}

	public static Texture GetTextureFromExtension(string ext) => ext switch
	{
		"cs" => ScriptIcon ?? throw new Exception("Icon isn't loaded yet"),
		_ => FileIcon ?? throw new Exception("Icon isn't loaded yet")
	};

	public override void Draw()
	{
		if (FileIcon == null)
		{
			FileIcon = Texture.LoadTexture(Path.Combine(Application.ResourcePath, "Icons", "File.png"));
			FolderIcon = Texture.LoadTexture(Path.Combine(Application.ResourcePath, "Icons", "Folder.png"));
			ScriptIcon = Texture.LoadTexture(Path.Combine(Application.ResourcePath, "Icons", "Script.png"));
		}

		var open = true;

		ImGui.Begin(Title, ref open);

		for (int i = 0; i < 200; i++)
		{
			var cursor = ImGui.GetCursorPos();
			ImGui.SetCursorPos(cursor + new System.Numerics.Vector2(5, 0));
			ImGui.Image(ScriptIcon?.Handle ?? throw new Exception("Icon isn't loaded yet"), new(64, 64));
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

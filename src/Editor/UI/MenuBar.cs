using ImGuiNET;

namespace BlinkLab.Editor.UI;

public class MenuBar : IUIElements
{
	public void Draw()
	{
		if (ImGui.BeginMainMenuBar())
		{
			if (ImGui.BeginMenu("File"))
			{
				if (ImGui.MenuItem("New", "Ctrl+N"))
				{
					New();
				}
				if (ImGui.MenuItem("Load", "Ctrl+O"))
				{
					Load();
				}
				if (ImGui.MenuItem("Quit"))
				{
					Application.Quit();
				}
				ImGui.EndMenu();
			}
			ImGui.EndMainMenuBar();
		}

		if (ImGui.IsKeyDown(ImGuiKey.ModCtrl))
		{
			if (ImGui.IsKeyPressed(ImGuiKey.N))
			{
				New();
			}
			if (ImGui.IsKeyPressed(ImGuiKey.O))
			{
				Load();
			}
		}
	}

	public static void New()
	{
		Application.uiManager.AddUI(new NewProjectDialog());
	}
	public static void Load()
	{
		var dialog = new FileSelectDialog("Choose a project");
		dialog.AfterPrompt += (path, canceled) =>
		{
			if (canceled) { return; }
			Application.LoadProject(path);
		};

		Application.uiManager.AddUI(dialog);
	}
}

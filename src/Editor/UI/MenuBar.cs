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
		}
	}

	public static void New()
	{
		Application.uiManager.AddUI(new NewProjectDialog());
	}
}

using ImGuiNET;

namespace BlinkLab.Editor.UI;

public class MenuBar : IUIElements
{
	public struct Menu
	{
		public string name;
		public EditorAction[] actions;
	}

	public struct EditorAction
	{
		public string name;
		public bool ctrlKey;
		public bool shiftKey;
		public bool altKey;
		public ImGuiKey key;
		public Action function;
	}

	readonly Menu[] menus = [
		new()
		{
			name = "File",
			actions = [
				new()
				{
					name = "New",
					ctrlKey = true,
					key = ImGuiKey.N,
					function = () => {
						Application.uiManager.AddUI(new NewProjectDialog());
					}
				},
				new()
				{
					name = "Load",
					ctrlKey = true,
					key = ImGuiKey.O,
					function = () => {
						var dialog = new FileSelectDialog("Choose a project");
						dialog.canOpen = (path) =>
						{
							return File.Exists(Path.Combine(path, "UserScript.csproj"));
						};

						dialog.AfterPrompt += (path, canceled) =>
						{
							if (canceled) { return; }
							Application.LoadProject(path);
						};

						Application.uiManager.AddUI(dialog);
					}
				}
			]
		}
	];

	public void Draw()
	{
		ImGui.BeginMainMenuBar();

		foreach (var menu in menus)
		{
			if (ImGui.BeginMenu(menu.name))
			{
				foreach (var item in menu.actions)
				{
					string shortcutKey = "";
					if (item.ctrlKey) { shortcutKey += "Ctrl + "; }
					if (item.shiftKey) { shortcutKey += "Shfit + "; }
					if (item.altKey) { shortcutKey += "Alt + "; }
					shortcutKey += item.key.ToString();

					if (ImGui.MenuItem(item.name, shortcutKey))
					{
						item.function();
					}
				}
				ImGui.EndMenu();
			}
		}

		ImGui.EndMainMenuBar();

		foreach (var menu in menus)
		{
			foreach (var item in menu.actions)
			{
				if (
						item.key != ImGuiKey.None &&
						(!item.ctrlKey || ImGui.IsKeyDown(ImGuiKey.ModCtrl)) &&
						(!item.shiftKey || ImGui.IsKeyDown(ImGuiKey.ModShift)) &&
						(!item.altKey || ImGui.IsKeyDown(ImGuiKey.ModAlt)) &&
						ImGui.IsKeyPressed(item.key)
					)
				{
					item.function();
				}
			}
		}
	}
}

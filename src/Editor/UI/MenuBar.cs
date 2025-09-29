using ImGuiNET;

namespace BlinkLab.Editor.UI;

public class MenuBar : IUIElements
{
	public struct Menu
	{
		public string name;
		public EditorAction[] actions;
	}

	public struct EditorAction(bool isSeparator)
	{
		public string name = "";
		public bool ctrlKey = false;
		public bool shiftKey = false;
		public bool altKey = false;
		public ImGuiKey key = ImGuiKey.None;
		public Action function = () => {};

		public bool isSeparator = isSeparator;
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
					function = () => Application.uiManager.AddUI(new NewProjectDialog())
				},
				new()
				{
					name = "Load",
					ctrlKey = true,
					key = ImGuiKey.O,
					function = () => {
						var dialog = new FileSelectDialog("Choose a project", Application.Configuration.loadProjectPath);
						dialog.canOpen = (path) =>
						{
							return File.Exists(Path.Combine(path, "UserScript.csproj"));
						};

						dialog.AfterPrompt += (path, canceled) =>
						{
							if (canceled) { return; }
							Application.Configuration.UpdateNewProjectPath(path);
							Application.LoadProject(path);
						};

						Application.uiManager.AddUI(dialog);
					}
				},
				new(true),
				new()
				{
					name = "Quit",
					function = Application.Quit
				},
			]
		},
		new()
		{
			name = "Edit",
			actions = [
				new()
				{
					name = "Undo",
					ctrlKey = true,
					key = ImGuiKey.Z,
					function = () => {

					}
				},
				new()
				{
					name = "Redo",
					ctrlKey = true,
					shiftKey = true,
					key = ImGuiKey.Z,
					function = () => {

					}
				},
				new(true),
				new()
				{
					name = "Preferences",
					ctrlKey = true,
					key = ImGuiKey.Comma,
					function = () => {

					}
				},
			]
		},
		new()
		{
			name = "Window",
			actions = [
				new()
				{
					name = "Assets",
					function = () =>
						Application.uiManager.AddUI(new FileSelectDialog("Assets", Application.ProjectPath, new FileSelectDialog.Option()
						{
							dontCloseAfterPrompt = true,
							noPrompt = true,
							openFileInApp = true,
							mode = FileSelectDialog.FileSelectMode.Both
						}))
				},
				new()
				{
					name = "Structure",
					function = () => Application.uiManager.AddUI(new StructureWindow())
				},
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
					if (item.isSeparator)
					{
						ImGui.Separator();
						continue;
					}

					string shortcutKey = "";
					if (item.key != ImGuiKey.None)
					{
						if (item.ctrlKey) { shortcutKey += "Ctrl + "; }
						if (item.shiftKey) { shortcutKey += "Shfit + "; }
						if (item.altKey) { shortcutKey += "Alt + "; }
						shortcutKey += item.key.ToString();
					}

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

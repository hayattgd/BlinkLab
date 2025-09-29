using BlinkLab.Editor.Utilities;
using BlinkLab.Engine;
using BlinkLab.Engine.Debug;
using ImGuiNET;

namespace BlinkLab.Editor.UI;

public class NewProjectDialog : EditorWindow
{
	public NewProjectDialog()
	{
		Title = "New Project";
	}

	string path = Application.Configuration.newProjectPath;
	string name = "Project";
	bool createSubFolder = true;

	string message = "";
	bool isDialogOpen = false;

	public override void Draw(int id)
	{
		bool openfolderdialog = false;
		bool create = false;
		bool isOpen = true;

		ImGui.Begin($"{Title}##{id}", ref isOpen);

		ImGui.InputText("Name", ref name, byte.MaxValue);

		ImGui.InputText("Path", ref path, (uint)short.MaxValue);
		ImGui.SameLine();
		ImGui.BeginDisabled(isDialogOpen);
		if (ImGui.SmallButton("Change") && !isDialogOpen)
		{
			openfolderdialog = true;
			isDialogOpen = true;
		}
		ImGui.EndDisabled();

		ImGui.Checkbox("Create subfolder", ref createSubFolder);

		ImGui.BeginDisabled(isDialogOpen);

		if (ImGui.Button("Cancel") || !isOpen)
		{
			Application.logger.Debug("Project creation canceled.");
			Close();
		}
		ImGui.SameLine();

		var finalpath = createSubFolder ? Path.Combine(path, name) : path;
		if (ImGui.Button("Create"))
		{
			if (Directory.Exists(finalpath) && (Directory.GetDirectories(finalpath).Length > 0 || Directory.GetFiles(finalpath).Length > 0))
			{
				message = "Specified directory isn't empty.";
			}
			else if (createSubFolder && !Directory.Exists(path))
			{
				message = "Specified path doesn't exist.";
			}
			else if (!PathUtility.IsValidFileOrDirectoryName(name, out char[] invalidchars))
			{
				string chars = "";
				chars += invalidchars[0];
				if (invalidchars.Length > 1)
				{
					for (int i = 1; i < invalidchars.Length; i++)
					{
						chars += $", {invalidchars[i]}";
					}
				}
				message = $"You can't use {chars} in Project name.";
			}
			else if (name != name.Trim())
			{
				message = "Project name cant end or start with white-space.";
			}
			else
			{
				create = true;
				Close();
			}
		}

		ImGui.EndDisabled();

		if (!string.IsNullOrEmpty(message))
		{
			ImGui.TextColored(new(1, 0, 0, 1), message);
		}

		ImGui.End();

		if (openfolderdialog)
		{
			if (manager == null) { throw new NullReferenceException(); }
			FileSelectDialog dialog = new("Choose a project path");
			dialog.AfterPrompt += (choosenPath, canceled) =>
			{
				isDialogOpen = false;
				if (canceled) { return; }
				path = choosenPath;
			};

			manager.AddUI(dialog);
		}
		if (create)
		{
			if (manager == null) { throw new NullReferenceException(); }

			Application.Configuration.UpdateNewProjectPath(finalpath);

			Logger logger = new("Project");

			ConsoleWindow console = new(logger);

			manager.AddUI(console);

			Task.Run(async () =>
			{
				await Project.SetupProject(
					new()
					{
						name = name
					},
					Path.Combine(Application.ResourcePath, "Templates", "UserScript.csproj"),
					Path.Combine(Application.BasePath, "BlinkLab.Engine.dll"),
					logger,
					finalpath
				);

				Application.LoadProject(path);
			});
		}
	}
}

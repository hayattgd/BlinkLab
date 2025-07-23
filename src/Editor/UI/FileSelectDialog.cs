using ImGuiNET;

namespace BlinkLab.Editor.UI;

public class FileSelectDialog : EditorWindow
{
	public FileSelectDialog(string name)
	{
		Title = name;
	}

	public FileSelectDialog(string name, string path)
	{
		Title = name;
		this.path = path;
	}

	public struct Entry
	{
		public string path;
		public readonly string Name => path.Split(Path.DirectorySeparatorChar)[^1];
	}

	public struct Option
	{
		public Option()
		{
			OnlyDirectory = false;
		}

		public bool OnlyDirectory;
	}
	
	/// <summary>
	/// string: Path,
	/// bool: Canceled
	/// </summary>
	public event Action<string, bool>? AfterPrompt;

	/// <summary>
	/// Return true if its able to open, false if not.
	/// Leave blank if its always true.
	/// 
	/// string: Path
	/// </summary>
	public Func<string, bool>? canOpen;

	bool canOpenThisPath;
	string loadedpath = "";
	string path = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
	int selectedidx = -1;

	Entry[] directories = [];
	Entry[] files = [];

	private void ReloadContents()
	{
		if (string.IsNullOrEmpty(path)) { path = "/"; }
		selectedidx = -1;
		List<Entry> directoryList = [];
		foreach (var directory in Directory.GetDirectories(path))
		{
			directoryList.Add(new() { path = directory });
		}
		directories = directoryList.ToArray();

		List<Entry> fileList = [];
		foreach (var file in Directory.GetFiles(path))
		{
			fileList.Add(new() { path = file });
		}
		files = fileList.ToArray();
		loadedpath = path;

		if (canOpen != null)
		{
			canOpenThisPath = canOpen(path);
		}
		else
		{
			canOpenThisPath = true;
		}
	}

	public override void Start()
	{
		ReloadContents();
	}

	public override void Draw()
	{
		bool isOpen = true;
		ImGui.Begin(Title, ref isOpen, ImGuiWindowFlags.NoScrollbar);
		ImGui.PushItemWidth(-1f);
		ImGui.InputText("###Path", ref path, byte.MaxValue, ImGuiInputTextFlags.ReadOnly);
		ImGui.PopItemWidth();
		ImGui.BeginChild("content", ImGui.GetContentRegionAvail() - new System.Numerics.Vector2(0, (ImGui.GetTextLineHeightWithSpacing() - ImGui.GetTextLineHeight()) * 2 + ImGui.GetTextLineHeight()), ImGuiChildFlags.Borders);

		if (path != "/" && ImGui.Selectable("(..) Parent Directory"))
		{
			var directories = path.Split(Path.DirectorySeparatorChar).ToList();

			directories.RemoveAt(directories.Count - 1);
			path = string.Join(Path.DirectorySeparatorChar, directories);

			ReloadContents();
		}

		bool somethingselected = false;
		try
		{
			for (int i = 0; i < directories.Length; i++)
			{
				if (directories[i].Name[0] == '.') { continue; }
				bool previouslySelected = selectedidx == i;
				bool selected = previouslySelected;
				if (ImGui.Selectable($"{directories[i].Name}{Path.DirectorySeparatorChar}", ref selected))
				{
					if (previouslySelected && !selected)
					{
						path = $"{directories[i].path}";
						ReloadContents();
					}
				}

				if (selected)
				{
					selectedidx = i;
					somethingselected = true;
				}
			}
			for (int i = 0; i < files.Length; i++)
			{
				if (files[i].Name[0] == '.') { continue; }
				bool selected = selectedidx == i + directories.Length;

				ImGui.Selectable($"{files[i].Name}", ref selected);

				if (selected)
				{
					selectedidx = i + directories.Length;
					somethingselected = true;
				}
			}
		}
		catch (UnauthorizedAccessException ex)
		{
			Application.logger.Error(ex.Message);
			path = loadedpath;
			ReloadContents();
		}
		if (!somethingselected) { selectedidx = -1; }
		ImGui.EndChild();
		if (ImGui.Button("Cancel") || !isOpen)
		{
			Close();
			AfterPrompt?.Invoke("", true);
		}
		ImGui.SameLine();
		if (!canOpenThisPath) { ImGui.BeginDisabled(); }
		if (ImGui.Button("Open"))
		{
			Close();

			string finalpath;
			if (selectedidx >= 0)
			{
				if (selectedidx > directories.Length)
				{
					finalpath = files[selectedidx - directories.Length].path;
				}
				else
				{
					finalpath = directories[selectedidx].path;
				}
			}
			else
			{
				finalpath = path;
			}

			Application.logger.Debug(finalpath);
			AfterPrompt?.Invoke(finalpath, false);
		}
		if (!canOpenThisPath) { ImGui.EndDisabled(); }
		ImGui.End();
	}
}

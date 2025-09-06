using BlinkLab.Editor.Platform;
using BlinkLab.Engine.Rendering;
using ImGuiNET;

namespace BlinkLab.Editor.UI;

public class FileSelectDialog : EditorWindow
{
	public FileSelectDialog()
	{
		Title = "Choose path";
	}

	public FileSelectDialog(string name)
	{
		Title = name;
	}

	public FileSelectDialog(string name, string path)
	{
		Title = name;
		this.path = path;
	}

	public FileSelectDialog(string name, string path, Option opt)
	{
		Title = name;
		this.path = path;
		this.opt = opt;
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
			mode = FileSelectMode.OnlyFile;
			dontCloseAfterPrompt = false;
			noPrompt = false;
		}

		public FileSelectMode mode;
		public bool dontCloseAfterPrompt;
		public bool noPrompt;
		public bool openFileInApp;

		public readonly bool ShowFile => (mode == FileSelectMode.OnlyFile) || (mode == FileSelectMode.Both);
	}

	public enum FileSelectMode
	{
		OnlyDirectory,
		OnlyFile,
		Both
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
	string path = Application.ProjectPath;
	int selectedidx = -1;

	public Option opt;

	Entry[] directories = [];
	Entry[] files = [];

	private static Texture? folderIcon;
	private readonly static Dictionary<string, Texture> fileIcons = [];

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

		if (opt.ShowFile)
		{
			List<Entry> fileList = [];
			foreach (var file in Directory.GetFiles(path))
			{
				fileList.Add(new() { path = file });
			}
			files = fileList.ToArray();
			loadedpath = path;
		}

		if (canOpen != null)
		{
			canOpenThisPath = canOpen(path);
		}
		else
		{
			canOpenThisPath = true;
		}
	}

	private static Texture LoadIcon(string key, string path)
	{
		if (fileIcons.TryGetValue(key, out Texture? tex)) { return tex; }

		tex = Texture.LoadTexture(path);
		fileIcons.Add(key, tex);
		return tex;
	}

	public Texture GetIconFromExtension(string ext) => ext switch
	{
		"sh" => LoadIcon(ext, Path.Combine(Application.ResourcePath, "Icons", "ShellScript.png")),
		"bat" => LoadIcon(ext, Path.Combine(Application.ResourcePath, "Icons", "ShellScript.png")),
		"cmd" => LoadIcon(ext, Path.Combine(Application.ResourcePath, "Icons", "ShellScript.png")),
		"cs" => LoadIcon(ext, Path.Combine(Application.ResourcePath, "Icons", "Script.png")),
		_ => LoadIcon(ext, Path.Combine(Application.ResourcePath, "Icons", "File.png"))
	};

	public override void Start()
	{
		folderIcon ??= Texture.LoadTexture(Path.Combine(Application.ResourcePath, "Icons", "Folder.png"));

		ReloadContents();
	}

	public override void Draw()
	{
		bool isOpen = true;
		ImGui.Begin(Title, ref isOpen, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse);
		ImGui.PushItemWidth(-1f);
		ImGui.InputText("###Path", ref path, byte.MaxValue, ImGuiInputTextFlags.ReadOnly);
		ImGui.PopItemWidth();
		ImGui.BeginChild("content", ImGui.GetContentRegionAvail() - new System.Numerics.Vector2(0, !opt.noPrompt ? (ImGui.GetTextLineHeightWithSpacing() - ImGui.GetTextLineHeight()) * 2 + ImGui.GetTextLineHeight() : 0), ImGuiChildFlags.Borders);

		if (path != "/" && ImGui.Selectable("(..) Parent Directory"))
		{
			var directories = path.Split(Path.DirectorySeparatorChar).ToList();

			directories.RemoveAt(directories.Count - 1);
			path = string.Join(Path.DirectorySeparatorChar, directories);

			ReloadContents();
		}

		bool somethingselected = false;
		bool opened = false;
		bool reloadlater = false;
		try
		{
			for (int i = 0; i < directories.Length; i++)
			{
				if (directories[i].Name[0] == '.') { continue; }
				bool previouslySelected = selectedidx == i;
				bool selected = previouslySelected;

				float cursorX = ImGui.GetCursorPosX();
				string ext = directories[i].Name.Split('.')[^1];
				ImGui.Image(folderIcon?.Handle ?? throw new Exception("Icon didn't loaded properly."), new(ImGui.GetTextLineHeight()));
				ImGui.SameLine();
				ImGui.SetCursorPosX(cursorX);

				if (ImGui.Selectable($"      {directories[i].Name}{Path.DirectorySeparatorChar}", ref selected))
				{
					if (previouslySelected && !selected)
					{
						path = $"{directories[i].path}";
						somethingselected = true;
						selectedidx = i;
						reloadlater = true;
					}
				}

				if (selected)
				{
					selectedidx = i;
					somethingselected = true;
				}
			}
			if (opt.ShowFile)
			{
				for (int i = 0; i < files.Length; i++)
				{
					if (files[i].Name[0] == '.') { continue; }
					bool previouslySelected = selectedidx == i + directories.Length;
					bool selected = previouslySelected;

					float cursorX = ImGui.GetCursorPosX();
					Texture icon;
					if (files[i].Name.Contains('.'))
					{
						string ext = files[i].Name.Split('.')[^1];
						icon = GetIconFromExtension(ext);
					}
					else
					{
						icon = GetIconFromExtension("");
					}

					ImGui.Image(icon.Handle, new(ImGui.GetTextLineHeight()));
					ImGui.SameLine();
					ImGui.SetCursorPosX(cursorX);

					if (ImGui.Selectable($"      {files[i].Name}", ref selected))
					{
						if (previouslySelected && !selected)
						{
							opened = true;
							somethingselected = true;
							selectedidx = i + directories.Length;
						}
					}

					if (selected)
					{
						selectedidx = i + directories.Length;
						somethingselected = true;
					}
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
		bool canceled = false;
		if (!opt.noPrompt)
		{
			canceled = ImGui.Button("Cancel");
			ImGui.SameLine();
			if (!canOpenThisPath) { ImGui.BeginDisabled(); }
			if (ImGui.Button("Open"))
			{
				opened = true;
			}
		}

		if (opened)
		{
			if (!opt.dontCloseAfterPrompt) { Close(); }

			string finalpath;
			if (selectedidx >= 0)
			{
				if (selectedidx >= directories.Length)
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

			if (opt.openFileInApp)
			{
				NativeApplication.OpenFile(finalpath);
			}
			AfterPrompt?.Invoke(finalpath, false);
		}

		if (reloadlater)
		{
			ReloadContents();
		}
		
		if (canceled || !isOpen)
			{
				Close();
				AfterPrompt?.Invoke("", true);
			}

		if (!canOpenThisPath) { ImGui.EndDisabled(); }
		ImGui.End();
	}
}

using ImGuiNET;

namespace BlinkLab.Editor.UI;

public abstract class EditorWindow : IUIElements
{
	public string Title { get; protected set; } = "";

	internal UIManager? manager;
	public bool IsDocked { get; private set; }

	public void Close()
	{
		manager!.CloseWindow(this);
	}

	protected void Begin(string name, ref bool open, ImGuiWindowFlags flags = ImGuiWindowFlags.None)
	{
		if (IsDocked) { flags |= ImGuiWindowFlags.NoBringToFrontOnFocus; }
		ImGui.Begin(name, ref open, flags);
		IsDocked = ImGui.IsWindowDocked();
	}

	protected void End()
	{
		ImGui.End();
	}

	public void Draw() { Draw(-1); }

	public abstract void Draw(int id);

	public virtual void Start() { }
	public virtual void Closing() { }
}

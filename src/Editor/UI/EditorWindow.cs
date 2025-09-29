namespace BlinkLab.Editor.UI;

public abstract class EditorWindow : IUIElements
{
	public string Title { get; protected set; } = "";

	internal UIManager? manager;

	public void Close()
	{
		if (manager == null) { throw new NullReferenceException(); }
		manager.CloseWindow(this);
	}

	public void Draw() { Draw(-1); }

	public abstract void Draw(int id);

	public virtual void Start() { }
	public virtual void Closing() { }
}

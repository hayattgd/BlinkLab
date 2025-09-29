using ImGuiNET;

namespace BlinkLab.Editor.UI;

public class UIManager
{
	private readonly List<IUIElements> uis = [];
	private readonly List<IUIElements> uisToAdd = [];
	private readonly List<IUIElements> uisToRemove = [];

	public void Update()
	{
		for (int i=0; i<uis.Count; i++)
		{
			try
			{
				ImGui.PushID(i);
				if (uis[i] is EditorWindow window)
				{
					window.Draw(i);
				}
				else
				{
					uis[i].Draw();
				}
				ImGui.PopID();
			}
			catch (Exception ex)
			{
				Application.logger.Error($"Error while drawing UI: {ex}");
				throw;
			}
		}

		foreach (var ui in uisToAdd)
		{
			uis.Add(ui);
		}
		uisToAdd.Clear();

		foreach (var ui in uisToRemove)
		{
			uis.Remove(ui);
		}
		uisToRemove.Clear();
	}

	public void AddUI(IUIElements ui)
	{
		uisToAdd.Add(ui);
		if (ui is EditorWindow window)
		{
			window.manager = this;
			window.Start();
		}
	}

	public void CloseWindow(IUIElements ui)
	{
		uisToRemove.Add(ui);
		if (ui is EditorWindow window)
		{
			window.Closing();
		}
	}
}

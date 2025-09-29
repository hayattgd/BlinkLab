using System.Collections.ObjectModel;
using BlinkLab.Engine;
using BlinkLab.Engine.World;
using ImGuiNET;

namespace BlinkLab.Editor.UI;

public class StructureWindow : EditorWindow
{
	public class ChildElement
	{
		internal ChildElement(Entity entity)
		{
			children = [];
			instance = entity;
		}
		public bool selected;
		public List<ChildElement> children;
		public Entity instance;
	}

	public ChildElement? selected;

	public List<ChildElement> structure = [];

	private ChildElement LoadEntity(Entity entity)
	{
		var element = new ChildElement(entity);

		if (entity.ChildrenCount == 0) { return element; }

		foreach (var child in entity.Children)
		{
			element.children.Add(LoadEntity(child));
		}

		return element;
	}

	private void ReloadStructure()
	{
		structure.Clear();
		if (!Application.IsProjectLoaded) { return; }
		Application.Project!.Root.Changed -= ReloadStructure; //Make sure ReloadStructure is registered once in Root
		Application.Project.Root.Changed += ReloadStructure;

		foreach (var item in Application.Project.Root.Children)
		{
			structure.Add(LoadEntity(item));
		}
	}

	private void DrawChildren(ReadOnlyCollection<ChildElement> children)
	{
		foreach (var child in children)
		{
			DrawElement(child);
		}
	}

	private void DrawElement(ChildElement element)
	{
		if (ImGui.TreeNode(element.instance.Name))
		{
			DrawChildren(element.children.AsReadOnly());
			ImGui.TreePop();
		}
	}

	public override void Start()
	{
		ReloadStructure();
	}

	public override void Draw()
	{
		bool open = true;
		ImGui.Begin("Structure", ref open);
		bool rclicked = ImGui.IsWindowHovered(ImGuiHoveredFlags.AllowWhenBlockedByPopup) && ImGui.IsMouseClicked(ImGuiMouseButton.Right) && !ImGui.IsAnyItemHovered();
		if (rclicked)
		{
			ImGui.OpenPopup("StructureMenu");
		}

		if (Application.IsProjectLoaded)
		{
			DrawChildren(structure.AsReadOnly());
		}

		if (ImGui.BeginPopup("StructureMenu"))
		{
			if (ImGui.BeginMenu("Create", Application.IsProjectLoaded))
			{
				if (ImGui.MenuItem("Empty")) { var e = new Entity("Empty"); e.SetParent(Application.Project?.Root!); }
				ImGui.EndMenu();
			}
			ImGui.EndPopup();
		}
		ImGui.End();

		if (!open) { Close(); }
	}
}

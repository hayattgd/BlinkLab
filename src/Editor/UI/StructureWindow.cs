using System.Collections.ObjectModel;
using BlinkLab.Engine.World;
using ImGuiNET;
using Microsoft.VisualBasic.FileIO;

namespace BlinkLab.Editor.UI;

public class StructureWindow : EditorWindow
{
	public StructureWindow()
	{
		Title = "Structure";
	}

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

	int lastId;
	public ChildElement? selected;
	private ChildElement? target;

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
		lastId++;
		bool hovered = false;
		if (ImGui.TreeNodeEx($"{element.instance.Name}##{lastId}", element.selected ? ImGuiTreeNodeFlags.Selected : ImGuiTreeNodeFlags.None))
		{
			hovered = ImGui.IsItemHovered();

			DrawChildren(element.children.AsReadOnly());
			ImGui.TreePop();
		}
		else
		{
			hovered = ImGui.IsItemHovered();
		}

		if (hovered && ImGui.IsMouseReleased(ImGuiMouseButton.Left))
		{
			element.selected = true;
			selected = element;
		}

		if (hovered && ImGui.IsMouseReleased(ImGuiMouseButton.Right))
		{
			ImGui.OpenPopup("StructureMenu");
			element.selected = true;
			selected = element;
			target = element;
		}

		if ((!hovered) && (!ImGui.IsWindowHovered()) && (!ImGui.IsAnyItemHovered()) && ImGui.IsMouseClicked(ImGuiMouseButton.Left))
		{
			element.selected = false;
			selected = null;
			target = null;
		}

		if (selected != element)
		{
			element.selected = false;
		}
	}

	public override void Start()
	{
		ReloadStructure();
	}

	public override void Draw(int id)
	{
		bool open = true;
		Begin($"{Title}##{id}", ref open);
		bool rclicked = ImGui.IsWindowHovered(ImGuiHoveredFlags.AllowWhenBlockedByPopup) && ImGui.IsMouseReleased(ImGuiMouseButton.Right);
		if (rclicked)
		{
			ImGui.OpenPopup("StructureMenu");
		}

		lastId = 0;

		if (Application.IsProjectLoaded)
		{
			DrawChildren(structure.AsReadOnly());
		}

		if (ImGui.BeginPopup("StructureMenu"))
		{
			if (ImGui.BeginMenu("Create", Application.IsProjectLoaded))
			{
				if (ImGui.MenuItem("Empty")) { var e = new Entity("Empty"); e.SetParent(target?.instance ?? Application.Project?.Root!); }
				ImGui.EndMenu();
			}
			if (ImGui.MenuItem("Delete", target != null)) { target?.instance.Destroy(); }
			ImGui.EndPopup();
		}
		End();

		if (!open) { Close(); }
	}
}

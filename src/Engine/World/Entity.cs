using System.Collections.ObjectModel;
using BlinkLab.Engine.Scripting;

namespace BlinkLab.Engine.World;

public class Entity(string name)
{
	public string Name = name;
	public List<ScriptableComponent> Components = [];
	public Entity Parent
	{
		get => _parent ?? this;
		set => SetParent(value);
	}

	private List<Entity> _children = [];
	private Entity? _parent = Project.LoadedProject?.Root ?? null;

	public event Action? Changed;
	public event Action? OnDestroy;
	public ReadOnlyCollection<Entity> Children => _children.AsReadOnly();
	public int ChildrenCount => _children.Count;

	public Entity GetChild(int idx) => _children[idx];
	public void SetParent(Entity entity)
	{
		_parent?.RemoveChild(this);
		_parent = entity;
		entity?.AddChild(this);
		InvokeChanged();
	}

	public void Destroy()
	{
		Destroy(true);
	}

	private void Destroy(bool isroot)
	{
		if (isroot)
		{
			Parent.InvokeChangedAfterRemove(this);
		}
		_parent = null;
		Changed = null;

		Components.Clear();

		foreach (var child in Children)
		{
			child.Destroy(false);
		}
		_children.Clear();

		OnDestroy?.Invoke();
		OnDestroy = null;
	}

	private void InvokeChangedAfterRemove(Entity entity)
	{
		RemoveChild(entity);
		InvokeChanged();
	}

	private void AddChild(Entity entity)
	{
		_children.Add(entity);
	}
	private void RemoveChild(Entity entity)
	{
		_children.Remove(entity);
	}
	private void InvokeChanged()
	{
		Changed?.Invoke();
		if (_parent == null) { return; }
		Parent?.InvokeChanged();
	}
}

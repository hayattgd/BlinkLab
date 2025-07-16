using BlinkLab.Engine.Scripting;

namespace BlinkLab.Engine.World;

public class Entity
{
	public Entity(string name)
	{
		Name = name;
	}

	public string Name = "Entity";

	public List<ScriptableComponent> Components = [];
	public List<Entity> Children = [];
}

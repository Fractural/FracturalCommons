using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public interface IComponentUser
{
	ComponentContainer Components { get; }
}

public class ComponentContainer : Node
{
    [Export]
    private List<NodePath> componentContainerPaths;
	[Export]
	private List<NodePath> componentPaths;

	public List<Node> Components { get; set;  } 

	public override void _Ready()
	{
		foreach (NodePath path in componentContainerPaths)
			foreach (Node child in GetNode(path).GetChildren())
				Components.Add(child);

		foreach (NodePath path in componentPaths)
			Components.Add(GetNode(path));
	}

	public IEnumerable<T> GetComponents<T>() where T : Node
	{
		return Components.Where(x => x is T).Select(x => x as T);
	}

	public T GetComponent<T>() where T : Node
	{
		return (T) Components.Where(x => x is T).FirstOrDefault();
	}

	public bool HasComponent<T>() where T : Node
	{
		return Components.Any(x => x is T);
	}
}

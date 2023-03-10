using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu()]
public class BehaviourTree : ScriptableObject
{
    public Node rootNode;
    public Node.State treeState = Node.State.Running;
	public List<Node> nodes = new List<Node>();
	public Blackboard blackboard = new Blackboard();
    public Node.State Update()
	{
		if (rootNode.state == Node.State.Running)
		{
			treeState = rootNode.Update();
		}
		return treeState;
	}
	//populate the child arrays of nodes
	public List<Node> GetChildren(Node parent)
	{
		List<Node> children = new List<Node>();
		RootNode root = parent as RootNode;
		if (root != null && root.child != null)
		{
			children.Add(root.child);
			return children;
		}
		DecoratorNode decorator = parent as DecoratorNode;
		if (decorator && decorator.child != null)
		{
			children.Add(decorator.child);
			return children;
		}
		CompositeNode composite = parent as CompositeNode;
		if (composite)
		{
			return composite.children;
		}

		return children;
	}

	//Travel through all children to set up their children and build tree
	public void Traverse(Node node, System.Action<Node> visiter)
	{
		if (node)
		{
			visiter.Invoke(node);
			var children = GetChildren(node);
			children.ForEach((n) => Traverse(n, visiter));
		}
	}

	//duplicate nodes and traverse through children
	public BehaviourTree Clone()
	{
		BehaviourTree tree = Instantiate(this);
		tree.rootNode = tree.rootNode.Clone();
		tree.nodes = new List<Node>();
		Traverse(tree.rootNode, (n) =>
		{
			tree.nodes.Add(n);
		});
		return tree;
	}

	//binds the current agent to each node and sets the blackboard to be shared.
	public void Bind(AiAgent agent)
	{
		Traverse(rootNode, node =>
		{
			node.agent = agent;
			node.blackboard = blackboard;
		});
	}
#if UNITY_EDITOR
	#region EditorStuff

	public Node CreateNode(Type type)
	{
		Node node = ScriptableObject.CreateInstance(type) as Node;
		node.name = type.Name;
		node.guid = GUID.Generate().ToString();
		Undo.RecordObject(this, "Behaviour Tree (CreateNode)");
		nodes.Add(node);
		if (!Application.isPlaying)
		{
			AssetDatabase.AddObjectToAsset(node, this);
		}
		Undo.RegisterCreatedObjectUndo(node, "Behaviour Tree (CreateNode)");
		AssetDatabase.SaveAssets();

		return node;
	}


	public void DeleteNode(Node node)
	{

		Undo.RecordObject(this, "Behaviour Tree (RemoveNode)");
		nodes.Remove(node);

		AssetDatabase.RemoveObjectFromAsset(node);
		Undo.DestroyObjectImmediate(node);
		AssetDatabase.SaveAssets();
	}

	public void AddChild(Node parent, Node child)
	{
		RootNode root = parent as RootNode;
		if (root != null)
		{
			Undo.RecordObject(root, "Behaviour Tree (AddChild)");
			root.child = child;
			EditorUtility.SetDirty(root);

		}

		DecoratorNode decorator = parent as DecoratorNode;
		if (decorator != null)
		{
			Undo.RecordObject(decorator, "Behaviour Tree (AddChild)");
			decorator.child = child;
			EditorUtility.SetDirty(decorator);
		}
		
		CompositeNode composite = parent as CompositeNode;
		if (composite != null)
		{
			Undo.RecordObject(composite, "Behaviour Tree (AddChild)");
			composite.children.Add(child);
			EditorUtility.SetDirty(composite);
		}
	}
	public void RemoveChild(Node parent, Node child)
	{
		RootNode root = parent as RootNode;
		if (root != null)
		{
			Undo.RecordObject(root, "Behaviour Tree (RemoveChild)");
			root.child = null;
			EditorUtility.SetDirty(root);
		}

		DecoratorNode decorator = parent as DecoratorNode;
		if (decorator)
		{
			Undo.RecordObject(decorator, "Behaviour Tree (RemoveChild)");
			decorator.child = null;
			EditorUtility.SetDirty(decorator);
		}

		CompositeNode composite = parent as CompositeNode;
		if (composite)
		{
			Undo.RecordObject(composite, "Behaviour Tree (RemoveChild)");
			composite.children.Remove(child);
			EditorUtility.SetDirty(composite);
		}
	}
	#endregion
#endif
}


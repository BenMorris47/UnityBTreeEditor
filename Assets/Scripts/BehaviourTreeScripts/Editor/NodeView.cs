using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;

public class NodeView : UnityEditor.Experimental.GraphView.Node
{
	public Action<NodeView> OnNodeSelected;
	public Node node;
	public Port input;
	public Port output;
	public NodeView(Node node) :base("Assets/Scripts/BehaviourTreeScripts/Editor/NodeView.uxml")
	{
		this.node = node;
		this.title = node.name;
		this.viewDataKey = node.guid;
		style.left = node.position.x;
		style.top = node.position.y;

		CreateInputPorts();
		CreateOutputPorts();
		SetupClasses();

		Label descriptionLable = this.Q<Label>("description");
		descriptionLable.bindingPath = "description";
		descriptionLable.Bind(new SerializedObject(node));
	}

	private void SetupClasses()
	{
		if (node is ActionNode)
		{
			AddToClassList("action");
		}
		else if (node is CompositeNode)
		{
			AddToClassList("composite");
		}
		else if (node is DecoratorNode)
		{
			AddToClassList("decorator");
		}
		else if (node is RootNode)
		{
			AddToClassList("root");
		}

	}

	private void CreateInputPorts()
	{
		if (node is ActionNode)
		{
			input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
		}
		else if (node is CompositeNode)
		{
			input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
		}
		else if (node is DecoratorNode)
		{
			input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
		}
		else if (node is RootNode)
		{
			
		}

		if (input != null)
		{
			input.portName = "";
			input.style.flexDirection = FlexDirection.Column;
			inputContainer.Add(input);
		}

	}

	private void CreateOutputPorts()
	{
		if (node is ActionNode)
		{

		}
		else if (node is CompositeNode)
		{
			output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(bool));
		}
		else if (node is DecoratorNode)
		{
			output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(bool));
		}
		else if (node is RootNode)
		{
			output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(bool));
		}

		if (output != null)
		{
			output.portName = "";
			output.style.flexDirection = FlexDirection.ColumnReverse;
			outputContainer.Add(output);
		}
	}

	public override void SetPosition(Rect newPos)
	{
		base.SetPosition(newPos);
		Undo.RecordObject(node, "Behaviour Tee (Set Position)");
		node.position.x = newPos.xMin;
		node.position.y = newPos.yMin;
		EditorUtility.SetDirty(node);
	}

	public override void OnSelected()
	{
		base.OnSelected();
		OnNodeSelected?.Invoke(this);
	}

	public void SortChildren()
	{
		CompositeNode composite = node as CompositeNode;
		if (composite)
		{
			composite.children.Sort(SortByHorizontalPosition);
		}
	}

	private int SortByHorizontalPosition(Node x, Node y)
	{
		//x is the left node and y is the right node
		return x.position.x < y.position.x ? -1 : 1;
	}

	public void UpdateState()
	{
		if (Application.isPlaying)
		{
			RemoveFromClassList("running");
			RemoveFromClassList("faliure");
			RemoveFromClassList("success");
			switch (node.state)
			{
				case Node.State.Running:
					if (node.started)
					{
						AddToClassList("running");
					}
					break;
				case Node.State.Failure:
					AddToClassList("faliure");
					break;
				case Node.State.Success:
					AddToClassList("success");
					break;
				default:
					break;
			}
		}
		
	}
}

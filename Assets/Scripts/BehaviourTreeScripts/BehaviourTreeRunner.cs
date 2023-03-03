using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourTreeRunner : MonoBehaviour
{
    public BehaviourTree tree;
	public AiAgent agent;

	private void Start()
	{
		//Create a clone of the BTree and attach it to the current agent
		tree = tree.Clone();
		tree.Bind(agent);
	}

	private void Update()
	{
		//Asses the tree every frame
		tree.Update();
	}
}

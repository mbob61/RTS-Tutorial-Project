using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

[UnityEngine.RequireComponent(typeof(CharacterManager))]
public class CharacterBT : BTree
{
    private CharacterManager manager;

    private void Awake()
    {
        manager = GetComponent<CharacterManager>();

    }

    protected override Node SetupTree()
    {
        Node root;

        root = new Parallel(new List<Node> {
            new Sequence(new List<Node>
            {
                new CheckUnitIsMine(manager),
                new TaskTrySetDestination(manager)
            }),
            new Sequence(new List<Node>
            {
                new CheckUnitHasDestination(),
                new TaskMoveToDestination(manager)
            })
        });
        return root;
    }
}

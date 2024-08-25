using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

[UnityEngine.RequireComponent(typeof(BuildingManager))]
public class BuildingBT : BTree
{
    private BuildingManager buildingManager;

    private void Awake()
    {
        buildingManager = GetComponent<BuildingManager>();
    }

    protected override Node SetupTree()
    {
        Node root;

        // build the tree
        root =
            new Sequence(new List<Node> {
                new CheckUnitIsMine(buildingManager),
                new Timer(
                    GameManager.instance.resourceProductionRate,
                    new List<Node>()
                    {
                        new TaskProduceResources(buildingManager)
                    },
                    delegate
                    {
                        EventManager.TriggerEvent("UpdateResourceTexts");
                    }
                )
            });

        return root;
    }
}

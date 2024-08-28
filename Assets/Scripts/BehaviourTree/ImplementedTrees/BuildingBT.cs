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

        root = new Parallel();

        if (buildingManager.Unit.Data.attackDamage > 0)
        {
            Sequence attackSequence = new Sequence(
                new List<Node>
                {
                    new CheckEnemyInAttackRange(buildingManager),
                    new Timer(buildingManager.Unit.Data.attackRate, new List<Node>
                    {
                        new TaskAttack(buildingManager)
                    })
                });

            root.Attach(attackSequence);
            root.Attach(new CheckEnemyInFOVRange(buildingManager));

        }

        Sequence resourceProductionSequence =
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

        // build the tree
        root.Attach(resourceProductionSequence);

        return root;
    }
}

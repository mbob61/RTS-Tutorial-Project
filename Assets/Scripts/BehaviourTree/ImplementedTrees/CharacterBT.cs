using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

[UnityEngine.RequireComponent(typeof(CharacterManager))]
public class CharacterBT : BTree
{
    private CharacterManager manager;
    private TaskTrySetDestinationOrTarget trySetDestinationOrTargetNode;


    private void OnEnable()
    {
        EventManager.AddListener("TargetFormationOffsets", OnTargetFormationOffsets);
        EventManager.AddListener("TargetFormationPositions", OnTargetFormationPositions);
    }

    private void OnDisable()
    {
        EventManager.RemoveListener("TargetFormationOffsets", OnTargetFormationOffsets);
        EventManager.RemoveListener("TargetFormationPositions", OnTargetFormationPositions);
    }

    private void Awake()
    {
        manager = GetComponent<CharacterManager>();
    }

    protected override Node SetupTree()
    {
        Node root;

        // prepare our subtrees...
        trySetDestinationOrTargetNode = new TaskTrySetDestinationOrTarget(manager);
        Sequence trySetDestinationOrTargetSequence = new Sequence(new List<Node> {
            new CheckUnitIsMine(manager),
            trySetDestinationOrTargetNode,
        });


        Sequence moveToDestinationSequence = new Sequence(new List<Node> {
            new CheckUnitHasDestination(),
            new TaskMoveToDestination(manager),
        });

        Selector attackOrBuildSelector = new Selector();
        if (manager.Unit.Data.attackDamage > 0)
        {
            Sequence attackSequence = new Sequence(new List<Node> {
                new Inverter(new List<Node>
                {
                    new CheckTargetIsOnMyTeam(manager),
                }),
                new CheckUnitInRange(manager, true),
                new Timer(
                    manager.Unit.Data.attackRate,
                    new List<Node>()
                    {
                        new TaskAttack(manager)
                    }
                ),
            });

            attackOrBuildSelector.Attach(attackSequence);
        }

        CharacterData cd = (CharacterData)manager.Unit.Data;
        if (cd.buildPower > 0)
        {
            Sequence buildSequence = new Sequence(new List<Node> {
                new CheckTargetIsOnMyTeam(manager),
                new CheckUnitInRange(manager, false),
                new Timer(
                    cd.buildRate,
                    new List<Node>()
                    {
                        new TaskBuild(manager)
                    }
                ),
            });
            attackOrBuildSelector.Attach(buildSequence);
        }


        Sequence moveToTargetSequence = new Sequence(new List<Node> {
            new CheckHasTarget()
        });
        if (manager.Unit.Data.attackDamage > 0)
        {
            moveToTargetSequence.Attach(new Selector(new List<Node> {
                attackOrBuildSelector,
                new TaskFollow(manager),
            }));
        }
        else
        {
            moveToTargetSequence.Attach(new TaskFollow(manager));
        }

        // ... then stitch them together under the root
        root = new Selector(new List<Node> {
            new Parallel(new List<Node> {
                trySetDestinationOrTargetSequence,
                new Selector(new List<Node>
                {
                    moveToDestinationSequence,
                    moveToTargetSequence,
                }),
            }),
            new CheckEnemyInFOVRange(manager),
        });

        return root;
    }

    private void OnTargetFormationOffsets(object data)
    {
        List<UnityEngine.Vector2> targetOffsets = (List<Vector2>)data;
        // extract offset for this unit from the list
        // and use in the BT data
        trySetDestinationOrTargetNode.SetFormationTargetOffset(targetOffsets);

    }

    private void OnTargetFormationPositions(object data)
    {
        List<UnityEngine.Vector3> targetPositions = (List<Vector3>)data;
        // extract position for this unit from the list
        // and use in the BT data
        trySetDestinationOrTargetNode.SetFormationTargetPosition(targetPositions);

    }

    public void StartBuildingConstruction(Transform buildingTransform)
    {
        Debug.Log("am i here");
        trySetDestinationOrTargetNode.SetFormationTargetOffset(new List<Vector2>() {
              UnityEngine.Vector2.zero
                  }, buildingTransform);
    }
}
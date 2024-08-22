using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterManager : UnitManager
{
    private Character character;
    public override Unit Unit {
        get => character;
        set => character = value is Character ? (Character)value : null;
    }

    [SerializeField] private NavMeshAgent agent;

    public void MoveTo(Vector3 position)
    {
        NavMeshPath path = new NavMeshPath();
        agent.CalculatePath(position, path);
        if (path.status == NavMeshPathStatus.PathInvalid)
        {
            contextualSource.PlayOneShot(((CharacterData)Unit.Data).onMoveInvalidSound);
            return;
        }
        agent.destination = position;
        contextualSource.PlayOneShot(((CharacterData)Unit.Data).onMoveValidSound);

    }


}

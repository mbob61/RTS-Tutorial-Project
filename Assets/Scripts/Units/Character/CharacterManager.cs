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
        agent.destination = position;
    }

    
}

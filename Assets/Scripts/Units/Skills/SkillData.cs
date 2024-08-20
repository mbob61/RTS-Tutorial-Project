using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum SkillType
{
    INSTANTIATE_CHARACTER
}

[CreateAssetMenu(fileName = "Skill", menuName = "Scriptable Objects/Skills", order = 4)]
public class SkillData : ScriptableObject
{
    public string code;
    public string skillName;
    public string description;
    public SkillType type;
    public UnitData unitReference;
    public float castTime;
    public float cooldown;
    public Sprite sprite;

    public void Trigger(GameObject source, GameObject target = null)
    {
        switch (type)
        {
            case SkillType.INSTANTIATE_CHARACTER:
                {
                    
                    BoxCollider boxCollider = source.GetComponent<BoxCollider>();
                    Vector3 instantiationPosition = new Vector3(
                        source.transform.position.x - boxCollider.size.x * 0.7f,
                        source.transform.position.y,
                        source.transform.position.z - boxCollider.size.z * 0.7f
                        );

                    CharacterData data = (CharacterData)unitReference;
                    Character character = new Character(data);
                    //character.Transform.position = instantiationPosition;
                    character.Transform.GetComponent<NavMeshAgent>().Warp(instantiationPosition);
                    character.Transform.GetComponent<CharacterManager>().Initialize(character);
                }
                break;
            default:
                break;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : UnitManager
{
    private Character character;
    public override Unit Unit {
        get => character;
        set => character = value is Character ? (Character)value : null;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

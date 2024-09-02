using System;
using System.Collections;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Reflection;
using UnityEngine;

[System.Serializable]
public class PlayerData : BinarySerializable
{
    public string name;
    public Color color;

    protected PlayerData(SerializationInfo info, StreamingContext context) : base(info, context)
    {

    }
}
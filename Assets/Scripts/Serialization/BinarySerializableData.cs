using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[Serializable]
public class BinarySerializableData
{
    private static List<Type> serializableTypes = new List<Type>()
    {
        typeof(int),
        typeof(float),
        typeof(bool),
        typeof(string),
        typeof(InputBinding),
        typeof(PlayerData)
    };

    public Dictionary<string, object> properties;

    public BinarySerializableData(
        ScriptableObject obj,
        List<string> fieldsToSerialize,
        bool serializeAll = false)
    {
        // extract the serializable data into "properties"
        properties = new Dictionary<string, object>();

        Type T = obj.GetType();
        foreach( FieldInfo field in T.GetFields())
        {
            if (!serializeAll && !fieldsToSerialize.Contains(field.Name))
            {
                continue;
            }
            object value;
            if (Serialize(field, obj, out value))
            {
                properties[field.Name] = value;
            }
        }

    }

    private static bool IsTypeSerializable(Type tested)
    {
        return serializableTypes.Contains(tested) ||
            tested.IsArray && serializableTypes.Contains(tested.GetElementType());
    }

    private static bool _IsOfType(Type tested, Type reference)
    {
        return
            tested == reference ||
            tested.IsArray && tested.GetElementType() == reference;
    }

    public static bool Serialize(FieldInfo field, object obj, out object value)
    {
        return SerializeValue(field.FieldType, field.GetValue(obj), out value);
    }

    public static bool Deserialize(FieldInfo field, object data, out object value)
    {
        Type T = field.FieldType;
        if (IsTypeSerializable(T))
        {
            value = data;
            return true;
        }
        else if (_IsOfType(T, typeof(Color)))
        {
            float[] c = (float[])data;
            value = new Color(c[0], c[1], c[2], c[3]);
            return true;
        }

        value = null;
        return false;
    }

    public static Type GetSerializedType(FieldInfo field)
    {
        Type T = field.FieldType;
        if (IsTypeSerializable(T))
        {
            return T;
        }

        object serialized;
        SerializeValue(T, T.IsValueType ? Activator.CreateInstance(T) : null, out serialized);

        return serialized.GetType();

    }

    private static bool SerializeValue(Type T, object inValue, out object outValue)
    {
        if (IsTypeSerializable(T))
        {
            outValue = inValue;
            return true;
        }
        else if (_IsOfType(T, typeof(Color)))
        {
            Color c = (Color)inValue;
            outValue = new float[] { c.r, c.g, c.b, c.a };
            return true;
        }

        outValue = null;
        return false;
    }
}

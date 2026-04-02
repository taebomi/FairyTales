using System;
using UnityEngine;

[Serializable]
public struct SerializableVector2
{
    public float x, y;
    
    public SerializableVector2(Vector2 vector2)
    {
        x = vector2.x;
        y = vector2.y;
    }
    
    public SerializableVector2(float x, float y)
    {
        this.x = x;
        this.y = y;
    }

    // serializableVector2 -> Vector2 로 암시적 형변환
    public static implicit operator Vector2(SerializableVector2 serializableVector2)
    {
        return new Vector2(serializableVector2.x, serializableVector2.y);
    }

    // Vector2 -> serializableVector2 로 암시적 형변환
    public static implicit operator SerializableVector2(Vector2 vector2)
    {
        return new SerializableVector2(vector2.x, vector2.y);
    }
    // serializableVector3 -> Vector3 로 암시적 형변환
    public static implicit operator Vector3(SerializableVector2 serializableVector2)
    {
        return new Vector3(serializableVector2.x, serializableVector2.y, 0f);
    }
        
    // Vector3 -> serializableVector2 로 암시적 형변환
    public static implicit operator SerializableVector2(Vector3 vector3)
    {
        return new SerializableVector2(vector3.x, vector3.y);
    }
}
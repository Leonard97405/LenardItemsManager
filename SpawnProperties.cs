using System;
using MapGeneration;
using UnityEngine;

namespace LenardItemsManager
{
    [Serializable]

    public class SpawnProperties
    {
        public RoomName RoomName { get; set; }
        public SerializableVector3 Offset { get; set; }
        public float Chance { get; set; } = 100f;
    }
    
    public class SerializableVector3
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public SerializableVector3() { }

        public SerializableVector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Vector3 ToUnityVector3()
            => new Vector3(X, Y, Z);
    }

}
using System;
using MapGeneration;
using UnityEngine;

namespace LenardItemsManager
{
    [Serializable]

    public class SpawnProperties
    {
        public RoomName RoomName { get; set; }
        public Vector3 Offset { get; set; }
        public float Chance { get; set; } = 100f;
    }
}
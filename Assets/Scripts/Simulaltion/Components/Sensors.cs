using System;
using UnityEngine;

namespace Components
{
    public class Sensors : MonoBehaviour
    {
        public DistanceSensor[] DistanceSensors;
    }

    [Serializable]
    public struct DistanceSensor
    {
        public string[] Layers;
        public float Angle;
        public float MaxDistance;
        public Vector3 Origin;
        public Vector3 Extents;

        public int LayerMask;
        public Vector3 Direction;
    }
}
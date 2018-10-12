using System;
using UnityEngine;

namespace Components
{
    public class Sensors : MonoBehaviour
    {
        public float SensorCount
        {
            get {
                return this.TelemetrySensor.GetTelemetryCount()
                    + DistanceSensors.Length * 1; // TODO OTHER FACTORS
            }
        }
        public DistanceSensor[] DistanceSensors;
        public TelemetrySensor TelemetrySensor;
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

    [Serializable]
    public struct TelemetrySensor
    {
        public delegate int GetTelemetryCountFn();
        public delegate double[] GetTelemetryFn();

        public GetTelemetryFn GetTelemetry;
        public GetTelemetryCountFn GetTelemetryCount;
    }
}
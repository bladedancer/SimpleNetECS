using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Components
{
    public struct Metabolism : IComponentData
    {
        public float HealthDecayRate;
        public float MealtimeInterval; // How long can you go between meals without getting hungry
    }
}
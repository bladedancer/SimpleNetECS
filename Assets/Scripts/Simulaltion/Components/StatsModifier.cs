using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Components
{
    public struct StatsModifier : IComponentData
    {
        public float HealthDelta;
        public float AgeDelta;
        public float NutritionDelta;
        public float AggressionDelta;
    }
}
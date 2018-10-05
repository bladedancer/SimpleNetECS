using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Components
{
    public struct Stats : IComponentData
    {
        public float Health;
        public float Age;

        // What someone gets by eatting this
        public float Nutrition;

        // Who wins in a fight
        public float Aggression;
    }
}
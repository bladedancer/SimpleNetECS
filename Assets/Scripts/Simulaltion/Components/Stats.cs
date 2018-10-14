using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Components
{
    public struct Stats : IComponentData
    {
        public int Tag; // Hash
        public float Fitness;
        public float Health;
        public float Age;

        public float TotalMeals;    // Total ever eaten
        public long LastMealTime;

        public int Generation;

        // What someone gets by eatting this
        public float Nutrition;

        // Who wins in a fight
        public float Aggression;

        public float MatingAge;
        public float MatingCost;
        public float MatingCoolDown;

    }
}
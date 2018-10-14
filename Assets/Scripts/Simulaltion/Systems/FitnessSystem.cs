using Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Systems
{
    [BurstCompile]
    [UpdateBefore(typeof(UnityEngine.Experimental.PlayerLoop.FixedUpdate))]
    class FitnessSystem : ComponentSystem
    {
        private struct Filter
        {
            public ComponentDataArray<Stats> Stats;
            public ComponentDataArray<Metabolism> Metabolism;
            public EntityArray Entity;
            public readonly int Length;
        }

        private struct LeaderFilter
        {
            [ReadOnly] public ComponentDataArray<CurrentFittest> Fittest;
            [ReadOnly] public ComponentDataArray<Stats> Stats;
            [ReadOnly] public EntityArray Entities;
            readonly public int Length;
            
        }

        [Inject] Filter group;
        [Inject] LeaderFilter currentLeader;

        protected override void OnUpdate()
        {
            float leaderFitness = 0;
            Nullable<Entity> newFittest = null;

            if (currentLeader.Length > 0)
            {
                leaderFitness = currentLeader.Stats[0].Fitness;
            }

            for (int i = 0; i < group.Length; i++)
            {
                Stats stats = group.Stats[i];
                stats.Fitness = stats.Age + stats.TotalMeals;
                PostUpdateCommands.SetComponent<Stats>(group.Entity[i], stats);

                if (stats.Fitness > leaderFitness)
                {
                    newFittest = group.Entity[i];
                    leaderFitness = stats.Fitness;
                }
            }

            if (newFittest != null)
            {
                // Just incase there's more than one for some reason
                for (int i = 0; i < currentLeader.Length; i++)
                {
                    PostUpdateCommands.RemoveComponent<CurrentFittest>(currentLeader.Entities[i]);
                }
                PostUpdateCommands.AddComponent<CurrentFittest>(newFittest.Value, new CurrentFittest());
            }
        }
    }
}

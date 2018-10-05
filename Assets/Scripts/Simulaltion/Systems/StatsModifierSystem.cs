using Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Systems
{
    class StatsModifierSystem : ComponentSystem
    {
        private struct Filter
        {
            public ComponentDataArray<Stats> Stats;
            public ComponentDataArray<StatsModifier> StatsModifier;
            public EntityArray Entity;
            public readonly int Length;
        }

        [Inject] Filter group;

        protected override void OnUpdate()
        {
            for (int i = 0; i < group.Length; i++)
            {
                Stats stats = group.Stats[i];
                stats.Age += group.StatsModifier[i].AgeDelta;
                stats.Health += group.StatsModifier[i].HealthDelta;
                stats.Nutrition += group.StatsModifier[i].NutritionDelta;
                stats.Aggression += group.StatsModifier[i].AggressionDelta;
                group.Stats[i] = stats;
                // Delete the applied modifier
                PostUpdateCommands.RemoveComponent<StatsModifier>(group.Entity[i]);

                if (stats.Health <= 0 && !EntityManager.HasComponent<Destroy>(group.Entity[i]))
                {
                    PostUpdateCommands.AddComponent<Destroy>(group.Entity[i], new Destroy());
                }
            }
        }
    }
}

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
    [UpdateBefore(typeof(StatsModifierSystem))]
    class MetabolismSystem : ComponentSystem
    {
        private struct Filter
        {
            public ComponentDataArray<Stats> Stats;
            public ComponentDataArray<Metabolism> Metabolism;
            public EntityArray Entity;
            public readonly int Length;
        }

        [Inject] Filter group;

        protected override void OnUpdate()
        {
            // TODO penalize complexity
            for (int i = 0; i < group.Length; i++)
            {
                if (EntityManager.HasComponent<StatsModifier>(group.Entity[i]))
                {
                    StatsModifier comp = EntityManager.GetComponentData<StatsModifier>(group.Entity[i]);
                    comp.HealthDelta -= group.Metabolism[i].HealthDecayRate * Time.deltaTime;
                    comp.AgeDelta += Time.deltaTime;
                    PostUpdateCommands.RemoveComponent<StatsModifier>(group.Entity[i]);
                    PostUpdateCommands.AddComponent<StatsModifier>(group.Entity[i], comp);
                }
                else
                {
                    PostUpdateCommands.AddComponent<StatsModifier>(group.Entity[i], new StatsModifier
                    {
                        HealthDelta = -group.Metabolism[i].HealthDecayRate * Time.deltaTime,
                        AgeDelta = Time.deltaTime
                    });
                }
            }
        }
    }
}

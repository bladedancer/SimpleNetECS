using Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Systems
{
    [BurstCompile]
    [UpdateBefore(typeof(UnityEngine.Experimental.PlayerLoop.FixedUpdate))]
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
            for (int i = 0; i < group.Length; i++)
            {
                Stats stats = group.Stats[i];
                Metabolism meta = group.Metabolism[i];

                float hungerFactor = Mathf.Floor(1.0f + ((DateTime.Now.Ticks - stats.LastMealTime) / (1000 * TimeSpan.TicksPerMillisecond * meta.MealtimeInterval)));
                float healthPenalty = meta.HealthDecayRate * Time.fixedDeltaTime;

                stats.Age += Time.fixedDeltaTime;
                stats.Health -= healthPenalty * hungerFactor;

                PostUpdateCommands.SetComponent<Stats>(group.Entity[i], stats);

                if (stats.Health <= 0 && !EntityManager.HasComponent<Destroy>(group.Entity[i]))
                {
                    PostUpdateCommands.AddComponent<Destroy>(group.Entity[i], new Destroy());
                }
            }
        }
    }
}

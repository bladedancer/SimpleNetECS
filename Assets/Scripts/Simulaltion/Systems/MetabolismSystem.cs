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
            // TODO penalize complexity
            for (int i = 0; i < group.Length; i++)
            {
                Stats stats = group.Stats[i];
                Metabolism meta = group.Metabolism[i];

                stats.Age += Time.fixedDeltaTime;
                stats.TimeSinceLastMeal += Time.fixedDeltaTime;
                stats.Health -= (meta.HealthDecayRate * Time.fixedDeltaTime) * Mathf.Floor(1.0f + (stats.TimeSinceLastMeal/meta.MealtimeInterval));

                PostUpdateCommands.SetComponent<Stats>(group.Entity[i], stats);

                if (stats.Health <= 0 && !EntityManager.HasComponent<Destroy>(group.Entity[i]))
                {
                    PostUpdateCommands.AddComponent<Destroy>(group.Entity[i], new Destroy());
                }
            }
        }
    }
}

using Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Systems
{
    [UpdateBefore(typeof(DestroySystem))]
    [UpdateBefore(typeof(CollisionCleanupSystem))]
    class AttackSystem : ComponentSystem
    {
        private struct Group
        {
            public ComponentDataArray<CollisionComponent> Data;
            readonly public EntityArray Entities;
            readonly public int Length;
        }

        [Inject] private Group Collisions;

        protected override void OnUpdate()
        {
            for (int i = 0; i < Collisions.Length; ++i)
            {
                CollisionComponent data = Collisions.Data[i];

                if (EntityManager.Exists(data.source) && !EntityManager.HasComponent<Destroy>(data.source) && EntityManager.HasComponent<Stats>(data.source)
                    && EntityManager.Exists(data.target) && !EntityManager.HasComponent<Destroy>(data.target) && EntityManager.HasComponent<Stats>(data.target))
                { 
                    Stats sourceStats = EntityManager.GetComponentData<Stats>(data.source);
                    Stats targetStats = EntityManager.GetComponentData<Stats>(data.target);

                    if (sourceStats.Aggression > targetStats.Aggression)
                    {
                        // Eaten
                        sourceStats.Health += targetStats.Nutrition;
                        PostUpdateCommands.SetComponent<Stats>(data.source, sourceStats);
                        PostUpdateCommands.AddComponent<Destroy>(data.target, new Destroy());
                    }
                }
            }
        }
    }
}

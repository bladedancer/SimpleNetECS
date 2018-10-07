using Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Systems
{
    [UpdateAfter(typeof(DestroySystem))]
    class MatingSystem : ComponentSystem
    {
        private struct Group
        {
            public ComponentDataArray<CollisionComponent> Data;
            readonly public EntityArray Entities;
            readonly public int Length;
        }

        [Inject] private Group Collisions;

        /// <summary>
        /// Check if you can mate.
        /// - Same type.
        /// - Old enough.
        /// - Only with someone fitter.
        /// - Only if healthy enough.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        private bool canMate(Stats source, Stats target)
        {
            return 
                source.Tag == target.Tag
                && source.Fitness >= target.Fitness
                && source.Age >= source.MatingAge
                && source.Health >= source.MatingCost
                && target.Age >= target.MatingAge
                && target.Health >= target.MatingCost;
        }

        protected override void OnUpdate()
        {
            for (int i = 0; i < Collisions.Length; ++i)
            {
                CollisionComponent data = Collisions.Data[i];

                if (EntityManager.Exists(data.source) && !EntityManager.HasComponent<Destroy>(data.source) && EntityManager.HasComponent<Stats>(data.source)
                    && EntityManager.Exists(data.target) && !EntityManager.HasComponent<Destroy>(data.target) && EntityManager.HasComponent<Stats>(data.target)
                    && !EntityManager.HasComponent<Parent>(data.source) && !EntityManager.HasComponent<Parent>(data.target))
                { 
                    Stats sourceStats = EntityManager.GetComponentData<Stats>(data.source);
                    Stats targetStats = EntityManager.GetComponentData<Stats>(data.target);

                    if (canMate(sourceStats, targetStats))
                    {
                        // Create a child
                        PostUpdateCommands.AddComponent<Embryo>(data.source, new Embryo
                        {
                            // TODO NET
                        });

                        // Update Parents
                        sourceStats.Health -= sourceStats.MatingCost;
                        targetStats.Health -= targetStats.MatingCost;
                        PostUpdateCommands.SetComponent<Stats>(data.source, sourceStats);
                        PostUpdateCommands.SetComponent<Stats>(data.target, targetStats);
                        PostUpdateCommands.AddComponent<Parent>(data.source, new Parent { CoolDown = sourceStats.MatingCoolDown });
                        PostUpdateCommands.AddComponent<Parent>(data.target, new Parent { CoolDown = targetStats.MatingCoolDown });
                    }
                }
            }
        }
    }
}

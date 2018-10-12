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
    [UpdateBefore(typeof(DestroySystem))]
    [UpdateBefore(typeof(CollisionCleanupSystem))]
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
            /*
            Debug.Log("" + source.Tag + " == " + target.Tag);
            Debug.Log("" + source.Fitness + " >= " + target.Fitness);
            Debug.Log("" + source.Age + " >= " + source.MatingAge);
            Debug.Log("" + source.Health + " >= " + source.MatingCost);
            Debug.Log("" + target.Age + " >= " + target.MatingAge);
            Debug.Log("" + target.Health + " >= " + target.MatingCost);
            */
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

                    // Debug.Log("CAN MATE: " + canMate(sourceStats, targetStats));
                    if (canMate(sourceStats, targetStats))
                    {
                        Net sourceNet = EntityManager.GetComponentObject<Net>(data.source);
                        Net targetNet = EntityManager.GetComponentObject<Net>(data.target);

                        // Debug.Log("WITH CHILD: " + data.source.Index);
                        // Create a child
                        PostUpdateCommands.AddSharedComponent<Embryo>(data.source, new Embryo
                        {
                            netdata = mate(sourceNet, targetNet)
                        });

                        // Update Parent (source pays the price)
                        sourceStats.Health -= sourceStats.MatingCost;
                        PostUpdateCommands.SetComponent<Stats>(data.source, sourceStats);
                        PostUpdateCommands.AddComponent<Parent>(data.source, new Parent { CoolDown = sourceStats.MatingCoolDown });
                    }
                }
            }
        }

        private NetData mate(Net mother, Net father)
        {
            return Neural.Mutators.RandomMix(new NetData[] {
                mother.Data,
                father.Data,
            }, new Neural.Options() {
                { "clone", true }
            }).First();
        }
    }
}

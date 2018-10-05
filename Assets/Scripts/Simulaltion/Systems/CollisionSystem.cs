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
    class CollisionSystem : ComponentSystem
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
                try
                {
                    CollisionComponent data = Collisions.Data[i];

                    // TODO: Doesn't feel right here

                    if (EntityManager.Exists(data.source) && !EntityManager.HasComponent<Destroy>(data.source))
                    {
                        PostUpdateCommands.AddComponent<Destroy>(data.source, new Destroy());
                    }
                    if (EntityManager.Exists(data.other) && !EntityManager.HasComponent<Destroy>(data.other))
                    {
                        PostUpdateCommands.AddComponent<Destroy>(data.other, new Destroy());
                    }

                    PostUpdateCommands.DestroyEntity(Collisions.Entities[i]);
                }
                catch (Exception ex)
                {
                    Debug.Log("Collision exception: " + ex.Message);
                }
            }
        }
    }
}

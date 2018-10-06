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
    [UpdateAfter(typeof(DestroySystem))]
    class CollisionCleanupSystem : ComponentSystem
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
                PostUpdateCommands.DestroyEntity(Collisions.Entities[i]);
            }
        }
    }
}

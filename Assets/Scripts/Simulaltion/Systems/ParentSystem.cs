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
    [UpdateBefore(typeof(MatingSystem))]
    class ParentSystem : ComponentSystem
    {
        private struct Filter
        {
            public ComponentDataArray<Parent> Parent;
            public EntityArray Entity;
            public readonly int Length;
        }

        [Inject] Filter group;

        protected override void OnUpdate()
        {
            for (int i = 0; i < group.Length; i++)
            {
                Parent parent = group.Parent[i];
                parent.CoolDown -= Time.deltaTime; // Probably wrong

                if (parent.CoolDown > 0)
                {
                    PostUpdateCommands.SetComponent<Parent>(group.Entity[i], parent);
                } else { 
                    PostUpdateCommands.RemoveComponent<Parent>(group.Entity[i]);
                }
            }
        }
    }
}

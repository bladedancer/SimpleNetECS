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
    [UpdateBefore(typeof(SaveSystem))]
    class FitnessSystem : ComponentSystem
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
                stats.Fitness = stats.Age;
                PostUpdateCommands.SetComponent<Stats>(group.Entity[i], stats);
            }
        }
    }
}

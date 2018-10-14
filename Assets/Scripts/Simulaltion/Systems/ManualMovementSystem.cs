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
    [UpdateAfter(typeof(NetMovementSystem))]
    [UpdateAfter(typeof(PlayerInputSystem))]
    [UpdateBefore(typeof(MovementSystem))]
    class ManualMovementSystem : ComponentSystem
    {
        private struct Filter
        {
            public ComponentDataArray<PlayerInput> PlayerInput;
            public ComponentArray<MotionInput> MotionInput;
            public readonly int Length;
        }

        [Inject] Filter Group;

        protected override void OnUpdate()
        {
            for (int i = 0; i < Group.Length; ++i)
            {
                MotionInput mot = Group.MotionInput[i];
                mot.Horizontal = Group.PlayerInput[i].Horizontal;
                mot.Vertical = Group.PlayerInput[i].Vertical;
            }
        }
    }
}

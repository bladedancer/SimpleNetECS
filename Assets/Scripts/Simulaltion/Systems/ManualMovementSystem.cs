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
    [UpdateAfter(typeof(NetMovementSystem))]
    [UpdateAfter(typeof(PlayerInputSystem))]
    [UpdateBefore(typeof(MovementSystem))]
    class ManualMovementSystem : ComponentSystem
    {
        private struct Filter
        {
            public PlayerInput PlayerInput;
            public MotionInput MotionInput;
        }

        protected override void OnUpdate()
        {
            // Override the network outputs with manual ones
            foreach (Filter entity in GetEntities<Filter>())
            {
                entity.MotionInput.Horizontal = entity.PlayerInput.Horizontal;
                entity.MotionInput.Vertical = entity.PlayerInput.Vertical;
            }
        }
    }
}

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
    class PlayerInputSystem : ComponentSystem
    {
        private struct Filter
        {
            public PlayerInput PlayerInput;
        }

        protected override void OnUpdate()
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            foreach (Filter entity in GetEntities<Filter>())
            {
                entity.PlayerInput.Horizontal = horizontal;
                entity.PlayerInput.Vertical = vertical;
            }
        }
    }
}

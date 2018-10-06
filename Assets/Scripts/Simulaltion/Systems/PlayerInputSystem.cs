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
            public ComponentDataArray<PlayerInput> PlayerInput;
            public readonly int Length;
        }

        [Inject] Filter Group;

        protected override void OnUpdate()
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            for (int i = 0; i < Group.Length; ++i)
            {
                PlayerInput inp = Group.PlayerInput[i];
                inp.Horizontal = horizontal;
                inp.Vertical = vertical;
                Group.PlayerInput[i] = inp;
            }
        }
    }
}

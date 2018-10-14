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
    [UpdateBefore(typeof(UnityEngine.Experimental.PlayerLoop.FixedUpdate))]
    class MovementSystem : ComponentSystem
    {
        private struct Filter
        {
            public Transform Transform;
            public Rigidbody Rigidbody;
            public MotionInput MotionInput;
            public Speed Speed;
            public RotationSpeed RotationSpeed;
        }

        protected override void OnUpdate()
        {
            foreach (Filter entity in GetEntities<Filter>())
            {
                Quaternion rotation = entity.Rigidbody.rotation;
                float3 angles = rotation.eulerAngles;
                angles.y += (entity.RotationSpeed.Value * entity.MotionInput.Horizontal * Time.deltaTime);
                entity.Rigidbody.rotation = Quaternion.Euler(angles);
                entity.Rigidbody.velocity = entity.Transform.forward * entity.MotionInput.Vertical * entity.Speed.Value * Time.deltaTime;
            }
        }
    }
}

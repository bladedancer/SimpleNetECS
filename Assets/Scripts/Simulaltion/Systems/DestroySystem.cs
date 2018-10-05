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
    [UpdateAfter(typeof(CollisionSystem))]
    class DestroySystem : ComponentSystem
    {
        private struct Filter
        {
            public ComponentArray<Transform> Transform;
            public ComponentDataArray<Destroy> Destroy;
            readonly public int Length;
        }

        [Inject] private Filter Entities;

        protected override void OnUpdate()
        {
            GameObject[] toDestroy = new GameObject[Entities.Length];

            for (int i = 0; i < Entities.Length; ++i)
            {
                toDestroy[i] = Entities.Transform[i].gameObject;
            }

            foreach(GameObject obj in toDestroy)
            {
                GameObject.Destroy(obj);
            }
        }
    }
}

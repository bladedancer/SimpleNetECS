using Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Systems
{
    struct ChildInfo
    {
        public int tag;
        public Transform container;
        public float3 position;
        public int generation;
        public NetData netData;
    }

    [Unity.Burst.BurstCompile]
    [UpdateAfter(typeof(MatingSystem))]
    class BirthSystem : ComponentSystem
    {
        public static Dictionary<int, GameObject> Prefabs = new Dictionary<int, GameObject>();

        private struct Group
        {
            [ReadOnly] public SharedComponentDataArray<Embryo> Embryo;
            public ComponentDataArray<Stats> Stats;
            [ReadOnly] public ComponentArray<Transform> Transform;
            readonly public EntityArray Entities;
            readonly public int Length;
        }

        [Inject] private Group Data;

        protected override void OnUpdate()
        {
            List <ChildInfo> children = new List<ChildInfo>();

            for (int i = 0; i < Data.Length; ++i)
            {
                Embryo embryo = Data.Embryo[i];
                Transform transform = Data.Transform[i];
                Stats stats = Data.Stats[i];

                // Create a child
                foreach (NetData netdata in embryo.netdata)
                {
                    ChildInfo child = new ChildInfo();
                    child.tag = stats.Tag;
                    child.container = transform.parent;
                    child.position = transform.position + new Vector3(1, 0, 1);
                    child.generation = stats.Generation + 1;
                    child.netData = netdata;
                    children.Add(child);
                }
                PostUpdateCommands.SetComponent<Stats>(Data.Entities[i], stats);
                PostUpdateCommands.RemoveComponent<Embryo>(Data.Entities[i]);
            }

            // Spawn the children
            foreach(ChildInfo child in children)
            {
                // TODO MAKE GENERIC NET INITIALIZATION RATHER THAN HERBIVORE
                // Debug.Log("CREATING CHILD");
                GameObject obj = GameObject.Instantiate(Prefabs[child.tag], child.position, Quaternion.identity, child.container);
                obj.GetComponent<HerbivoreController>().InitalNet = child.netData;
                obj.name = Prefabs[child.tag].name + " Gen-" + child.generation + " (" + child.container.childCount + ")";
            }
        }
    }
}

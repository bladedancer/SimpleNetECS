using Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Systems
{
    [BurstCompile]
    [UpdateBefore(typeof(DestroySystem))]
    class SaveSystem : ComponentSystem
    {
        private struct Destroyed
        {
            public ComponentArray<Net> Net;
            public ComponentDataArray<Stats> Stats;
            public ComponentDataArray<Destroy> Destroy;
            readonly public int Length;
        }

        private struct Manager
        {
            [ReadOnly] public SharedComponentDataArray<Fittest> Fittest;
            [ReadOnly] public EntityArray Entities;
            readonly public int Length;
        }

        [Inject] private Destroyed Entities;
        [Inject] private Manager Saved;

        protected override void OnUpdate()
        {
            // Should only be one
            Fittest fittestData = Saved.Fittest[0];
            float fitnessToBeat = fittestData.fitness;
            int tag = 0;
            Nullable<NetData> netToSave = null;

            for (int i = 0; i < Entities.Length; ++i)
            {
                if (Entities.Stats[i].Fitness > fitnessToBeat)
                {
                    tag = Entities.Stats[i].Tag;
                    fitnessToBeat = Entities.Stats[i].Fitness;
                    netToSave = Entities.Net[i].Data;
                }
            }

            if (netToSave != null)
            {
                Debug.Log("FITTEST: " + fitnessToBeat);
                fittestData.fitness = fitnessToBeat;
                fittestData.net = netToSave.Value;
                fittestData.tag = tag;
                PostUpdateCommands.SetSharedComponent(Saved.Entities[0], fittestData);
            }
        }
    }
}

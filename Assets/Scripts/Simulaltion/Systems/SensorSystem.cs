using Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;

namespace Systems
{
    class SensorSystem : ComponentSystem {
        private struct Filter
        {
            public Transform Transform;
            public Sensors Sensors;
            public SensorData SensorData;
        }

        struct SensorBatchItem
        {
            public int BatchIndex;
            public int ResultIndex;
            public Transform Transform;
            public DistanceSensor Sensor;
            public SensorData SensorData;
        }

        protected override void OnUpdate()
        {
            ComponentGroupArray<Filter> entities = GetEntities<Filter>();
            if (entities.Length == 0)
            {
                return;
            }

            // How big is the list
            List<SensorBatchItem> sensorBatchItems = new List<SensorBatchItem>();
            int batchIndex = 0;
            foreach (Filter entity in entities)
            {
                for (int i = 0; i < entity.Sensors.DistanceSensors.Length; ++i)
                {
                    sensorBatchItems.Add(new SensorBatchItem()
                    {
                        BatchIndex = batchIndex++,
                        ResultIndex = i,
                        Transform = entity.Transform,
                        Sensor = entity.Sensors.DistanceSensors[i],
                        SensorData = entity.SensorData
                    });
                }
            }

            if (sensorBatchItems.Count == 0)
            {
                return;
            }

            // Batch'em
            NativeArray<RaycastHit> results = new NativeArray<RaycastHit>(sensorBatchItems.Count, Allocator.Temp);
            NativeArray<RaycastCommand> commands = new NativeArray<RaycastCommand>(sensorBatchItems.Count, Allocator.Temp);

            foreach (SensorBatchItem item in sensorBatchItems) {
                Vector3 dir = item.Transform.TransformDirection(new Vector3(item.Sensor.Direction.x, 0, item.Sensor.Direction.z)).normalized;
                Vector3 raySrc = item.Transform.position + (item.Transform.right * item.Sensor.Origin.x) + (item.Transform.forward * item.Sensor.Origin.z) + (item.Transform.up * item.Sensor.Origin.y);
                Debug.DrawRay(raySrc, dir * item.Sensor.MaxDistance);
                commands[item.BatchIndex] = new RaycastCommand(raySrc, dir, item.Sensor.MaxDistance);
            }

            // Schedule the batch of raycasts
            var handle = RaycastCommand.ScheduleBatch(commands, results, 64);

            // Wait for the batch processing job to complete
            handle.Complete();

            // Update the results
            for (int i = 0; i < results.Length; ++i)
            {
                SensorBatchItem item = sensorBatchItems[i];
                RaycastHit hit = results[i];

                if (hit.transform != null)
                {
                    item.SensorData.Data[item.ResultIndex] = 1 - (hit.distance / item.Sensor.MaxDistance);
                }
                else
                {
                    item.SensorData.Data[item.ResultIndex] = 0;
                }
            }
            // Dispose the buffers
            results.Dispose();
            commands.Dispose();
        }
    }
}

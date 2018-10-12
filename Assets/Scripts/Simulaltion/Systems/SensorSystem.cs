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
        /// <summary>
        /// What do we measure in distance sensors:
        /// relativeDistance, relativeAggression, relativeFitness
        /// </summary>
        public static int DATA_POINTS_PER_DISTANCE_SENSOR = 3;

        private struct Filter
        {
            [ReadOnly] public ComponentDataArray<Stats> Stats;
            [ReadOnly] public ComponentArray<Transform> Transform;
            [ReadOnly] public ComponentArray<Sensors> Sensors;
            public ComponentArray<SensorData> SensorData;
            readonly public EntityArray Entities;
            readonly public int Length;
        }

        [Inject] private Filter Group;

        /// <summary>
        /// The information need to batch the raycasts and process the results.
        /// </summary>
        struct CastBatchItem
        {
            public int BatchIndex;
            public int ResultIndex;
            public Stats Stats;
            public Transform Transform;
            public DistanceSensor Sensor;
            public SensorData SensorData;
        }

        /// <summary>
        /// Add the telemetry measurements to the sensor data.
        /// </summary>
        /// <param name="sensor">The telemetry sensor details.</param>
        /// <param name="sensorData">The target sensor data to write to.</param>
        /// <param name="offset">The offeset to write to.</param>
        /// <returns>The number of data points written.</returns>
        private int AddTelemetry(TelemetrySensor sensor, SensorData sensorData, int offset)
        {
            if (sensor.GetTelemetry != null)
            {
                double[] telemetry = sensor.GetTelemetry();
                telemetry.CopyTo(sensorData.Data, offset);
                return telemetry.Length;
            } else
            {
                return 0;
            }
        }

        private void AddDistanceSensors(List<CastBatchItem> castBatchItems)
        {
            if (castBatchItems.Count == 0)
            {
                return;
            }

            // Batch'em
            NativeArray<RaycastHit> results = new NativeArray<RaycastHit>(castBatchItems.Count, Allocator.TempJob);
            NativeArray<BoxcastCommand> commands = new NativeArray<BoxcastCommand>(castBatchItems.Count, Allocator.TempJob);

            foreach (CastBatchItem item in castBatchItems)
            {
                Vector3 dir = item.Transform.TransformDirection(new Vector3(item.Sensor.Direction.x, 0, item.Sensor.Direction.z)).normalized;
                Vector3 raySrc = item.Transform.position + (item.Transform.right * item.Sensor.Origin.x) + (item.Transform.forward * item.Sensor.Origin.z) + (item.Transform.up * item.Sensor.Origin.y);
                Debug.DrawRay(raySrc, dir * item.Sensor.MaxDistance);
                commands[item.BatchIndex] = new BoxcastCommand(raySrc, item.Sensor.Extents, Quaternion.identity, dir, item.Sensor.MaxDistance);
            }

            // Schedule the batch of raycasts
            var handle = BoxcastCommand.ScheduleBatch(commands, results, 64);

            // Wait for the batch processing job to complete
            handle.Complete();

            // Update the results
            for (int i = 0; i < results.Length; ++i)
            {
                CastBatchItem item = castBatchItems[i];
                RaycastHit hit = results[i];

                double relativeDistance = 0;
                double relativeAggression = 0;
                double relativeFitness = 0;

                if (hit.transform != null)
                {
                    // Distance
                    relativeDistance = 1 - (hit.distance / item.Sensor.MaxDistance);

                    // Relative Stats
                    GameObjectEntity goe = hit.collider.GetComponent<GameObjectEntity>();
                    if (goe != null)
                    {
                        Stats otherStats = EntityManager.GetComponentData<Stats>(goe.Entity);
                        
                        // Aggression
                        relativeAggression = item.Stats.Aggression != 0 
                            ? Math.Tanh((otherStats.Aggression/item.Stats.Aggression) - 1) 
                            : Math.Tanh(otherStats.Aggression);

                        // Fitness
                        if (item.Stats.Tag == otherStats.Tag)
                        {
                            relativeFitness = item.Stats.Fitness != 0
                                ? Math.Tanh((otherStats.Fitness / item.Stats.Fitness) - 1)
                                : Math.Tanh(otherStats.Fitness);
                        }
                        else
                        {
                            relativeFitness = -1;
                        }
                    }
                }

                item.SensorData.Data[item.ResultIndex + 0] = relativeDistance;
                item.SensorData.Data[item.ResultIndex + 1] = relativeAggression;
                item.SensorData.Data[item.ResultIndex + 2] = relativeFitness;
            }

            // Dispose the buffers
            results.Dispose();
            commands.Dispose();
            return;
        }

        protected override void OnUpdate()
        {
            List<CastBatchItem> castBatchItems = new List<CastBatchItem>();
            int batchIndex = 0;

            for (int i = 0; i < Group.Length; ++i)
            {
                // Add The telemetry
                int offset = AddTelemetry(Group.Sensors[i].TelemetrySensor, Group.SensorData[i], 0);

                // Gather information for the raycasting
                for (int j = 0; j < Group.Sensors[i].DistanceSensors.Length; ++j)
                {
                    castBatchItems.Add(new CastBatchItem()
                    {
                        BatchIndex = batchIndex++,
                        ResultIndex = offset + (j * DATA_POINTS_PER_DISTANCE_SENSOR),
                        Transform = Group.Transform[i],
                        Stats = Group.Stats[i],
                        Sensor = Group.Sensors[i].DistanceSensors[j],
                        SensorData = Group.SensorData[i]
                    });
                }
            }

            AddDistanceSensors(castBatchItems);
        }
    }
}

using Components;
using System;
using System.Collections;
using System.Collections.Generic;
using Systems;
using Unity.Entities;
using UnityEngine;

public class HerbivoreController : MonoBehaviour {
    #region Prefab Settings
    public float InitialHealth = 100;
    public float InitialAggression = 1;
    public float InitialNutrition = 25;
    public float InitialMealtimeInterval = 5;
    public float MatingAge = 20;
    public float MatingCost = 25;
    public float MatingCoolDown = 5;
    public int[] HiddenLayerSizes = new int[0];
    public DistanceSensor[] DistanceSensors;
    private GameObjectEntity gameObjectEntity;
    #endregion

    #region Mutable Initialization Settings
    // This is the net the child starts with
    public NetData InitalNet;
    #endregion

    private MotionInput motionInput;

    private void Awake()
    {
        gameObjectEntity = GetComponent<GameObjectEntity>();
        motionInput = GetComponent<MotionInput>();
    }

    private int getTelemetryCount()
    {
        // KEEP INSYNC WITH getTelemetry
        return 3;
    }

    private double[] getTelemetry()
    {
        // Probably not optimal
        Stats stats = gameObjectEntity.EntityManager.GetComponentData<Stats>(gameObjectEntity.Entity);

        return new double[]
        {
            motionInput.Horizontal,
            motionInput.Vertical,
            Math.Tanh(stats.Health / InitialHealth)
        };
    }

    void Start() {
        // Distance Sensors
        for (int i = 0; i < DistanceSensors.Length; ++i)
        {
            DistanceSensor sensor = DistanceSensors[i];
            DistanceSensors[i].LayerMask = LayerMask.GetMask(DistanceSensors[i].Layers);
            Quaternion rot = Quaternion.AngleAxis(DistanceSensors[i].Angle, Vector3.up);
            DistanceSensors[i].Direction = (rot * Vector3.forward).normalized;
        }

        // Telemetry Sensors
        TelemetrySensor teleSense = new TelemetrySensor()
        {
            GetTelemetry = this.getTelemetry,
            GetTelemetryCount = this.getTelemetryCount
        };

        // Sensor Component
        Sensors sensors = GetComponent<Sensors>();
        sensors.DistanceSensors = DistanceSensors;
        sensors.TelemetrySensor = teleSense;

        // Sensor Data Component
        SensorData sensorData = GetComponent<SensorData>();
        sensorData.Data = new double[
            sensors.TelemetrySensor.GetTelemetryCount() +
            (sensors.DistanceSensors.Length * SensorSystem.DATA_POINTS_PER_DISTANCE_SENSOR)
        ];

        for (int i = 0; i < sensorData.Data.Length; ++i)
        {
            sensorData.Data[i] = 0;
        }

        // Initialize the layers
        Net net = GetComponent<Net>();
        if (InitalNet.LayerSizes != null)
        {
            // Debug.Log("INITIALIZING WITH NET");
            net.Data = InitalNet;
        }
        else
        {
            net.Data = new NetData();
            net.Data.LayerSizes = new int[HiddenLayerSizes.Length + 2];
            net.Data.LayerSizes[0] = sensorData.Data.Length;
            HiddenLayerSizes.CopyTo(net.Data.LayerSizes, 1);
            net.Data.LayerSizes[HiddenLayerSizes.Length + 1] = 2; // MotionInputs
            net.Data.Weights = new double[netSize(net.Data)];
            Neural.Mutators.SelfMutate(new NetData[] { net.Data }, new Neural.Options()
            {
                { "clone", false },
                { "mutationProbability", 1 },
                { "mutationFactor", 1 },
                { "mutationRange", 10000 },
            });
        }

        // Mettabolism
        Metabolism metabolism = new Metabolism
        {
            HealthDecayRate = 1,
            MealtimeInterval = InitialMealtimeInterval
        };
        gameObjectEntity.EntityManager.AddComponentData<Metabolism>(gameObjectEntity.Entity, metabolism);

        // Stats
        Stats stats = new Stats
        {
            Tag = gameObject.tag.GetHashCode(),
            Health = InitialHealth,
            Age = 0,
            Generation = 0,
            Aggression = InitialAggression,
            Nutrition = InitialNutrition,
            MatingAge = MatingAge,
            MatingCost = MatingCost,
            MatingCoolDown = MatingCoolDown
        };
        gameObjectEntity.EntityManager.AddComponentData<Stats>(gameObjectEntity.Entity, stats);
    }

    private int netSize(NetData net)
    {
        int totalWeights = 0;
        for (int i = 1; i < net.LayerSizes.Length; ++i)
        {
            totalWeights += net.LayerSizes[i] * (net.LayerSizes[i - 1] + 1 /*bias node*/);
        }
        return totalWeights;
    }

}

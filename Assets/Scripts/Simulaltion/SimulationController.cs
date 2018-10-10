using Components;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class SimulationController : MonoBehaviour {
    public Nullable<Fittest> Fittest = null;
    private GameObjectEntity gameObjectEntity;

    private void Awake()
    {
        gameObjectEntity = GetComponent<GameObjectEntity>();
    }

    void Start () {
        // TODO LOAD/SAVE
        // Initialize fittest 
        Fittest fittest = new Fittest
        {
            fitness = 0,
            net = new NetData()
        };
        gameObjectEntity.EntityManager.AddSharedComponentData<Fittest>(gameObjectEntity.Entity, fittest);
    }

    private void Update()
    {
        Fittest = gameObjectEntity.EntityManager.GetSharedComponentData<Fittest>(gameObjectEntity.Entity);
    }
}

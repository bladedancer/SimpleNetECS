using Components;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class SimulationController : MonoBehaviour {
    public Nullable<Fittest> Fittest = null;
    public Nullable<Stats> CurrentFittest = null;
    public Net CurrentFittestNet = null;
    public Transform CurrentFittestTransform = null;
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

        StartCoroutine(updateStats());
    }

    private IEnumerator updateStats()
    {
        // Got to be a better way than this.....
        while (true) {
            Nullable<Entity> curFittest = gameObjectEntity.EntityManager.GetAllEntities().Where((e) => gameObjectEntity.EntityManager.HasComponent<CurrentFittest>(e)).FirstOrDefault();
            if (curFittest.HasValue && gameObjectEntity.EntityManager.Exists(curFittest.Value))
            {
                CurrentFittest = gameObjectEntity.EntityManager.GetComponentData<Stats>(curFittest.Value);
                CurrentFittestTransform = gameObjectEntity.EntityManager.GetComponentObject<Transform>(curFittest.Value);
                CurrentFittestNet = gameObjectEntity.EntityManager.GetComponentObject<Net>(curFittest.Value);
            }
            Fittest = gameObjectEntity.EntityManager.GetSharedComponentData<Fittest>(gameObjectEntity.Entity);
            yield return new WaitForSeconds(1);
        }
    }
}

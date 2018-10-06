using Components;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class CropController : MonoBehaviour {
    public float InitialNutrition = 5;
    private GameObjectEntity gameObjectEntity;

    // Use this for initialization
    void Start () {
        gameObjectEntity = GetComponent<GameObjectEntity>();

        // Stats
        Stats stats = new Stats
        {
            Age = 0,
            Aggression = 0,
            Nutrition = InitialNutrition
        };
        gameObjectEntity.EntityManager.AddComponentData<Stats>(gameObjectEntity.Entity, stats);
    }
}

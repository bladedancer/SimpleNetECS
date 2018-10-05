using Components;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

public class StatUIController : MonoBehaviour {

    public Slider health;
    private GameObjectEntity goe;

	void Start () {
        goe = GetComponent<GameObjectEntity>();		
	}
	
	// Update is called once per frame
	void Update () {
        Stats stats = goe.EntityManager.GetComponentData<Stats>(goe.Entity);
        health.value = stats.Health / 100;
	}
}

using Components;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatUIController : MonoBehaviour {

    public Slider health;
    public TMP_Text fitness;
    private GameObjectEntity goe;

	void Start () {
        goe = GetComponent<GameObjectEntity>();		
	}
	
	// Update is called once per frame
	void Update () {
        Stats stats = goe.EntityManager.GetComponentData<Stats>(goe.Entity);
        health.value = stats.Health / 100;
        fitness.text = Mathf.FloorToInt(stats.Fitness).ToString();
	}
}

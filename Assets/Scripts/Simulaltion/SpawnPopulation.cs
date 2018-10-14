using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Maintain a constant population.
public class SpawnPopulation : SpawnBase{
    public float population;
    public float cooldown;

    private float lastUpdate = float.MinValue;

	void Start () {
        base.OnStart();
        Fill();
    }
	
	// Update is called once per frame
	void Update () {
        if (Time.time <= lastUpdate + cooldown)
        {
            return;
        }
        lastUpdate = Time.time;
        Fill();
    }

    protected void Fill()
    {
        int count = (int) Mathf.Max(0, population - parent.childCount);
        if (count > 0)
        {
            Spawn(count);
        }
    }
}

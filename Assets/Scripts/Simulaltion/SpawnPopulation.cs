using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Maintain a constant population.
public class SpawnPopulation : SpawnBase{
    public float population;

	void Start () {
        Fill();
    }
	
	// Update is called once per frame
	void Update () {
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

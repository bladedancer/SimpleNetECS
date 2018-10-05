using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerOnKey : SpawnBase {

    public KeyCode SpawnKey;
    public int SpawnCount;

    public int SpawnTotal
    {
        get;
        private set;
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey(SpawnKey))
        {
            Spawn(SpawnCount);
        }	
	}
}

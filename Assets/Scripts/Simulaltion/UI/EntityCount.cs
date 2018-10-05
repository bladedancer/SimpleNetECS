using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EntityCount : MonoBehaviour {

    public Text entityCount;
    public SpawnerOnKey spawner;
	
	// Update is called once per frame
	void Update () {
        entityCount.text = spawner.SpawnTotal.ToString();
	}
}

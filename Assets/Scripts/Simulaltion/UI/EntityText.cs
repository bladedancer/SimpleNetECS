﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EntityText : MonoBehaviour {

    public Text entityText;
    public SimulationController simCtrl;
	
	// Update is called once per frame
	void Update () {
        entityText.text = (simCtrl.Fittest.HasValue ? simCtrl.Fittest.Value.fitness : 0).ToString();
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EntityText : MonoBehaviour {

    public Text entityText;
    public SimulationController simCtrl;
	
	// Update is called once per frame
	void Update () {
        string cur = Mathf.FloorToInt(simCtrl.CurrentFittest.HasValue ? simCtrl.CurrentFittest.Value.Fitness : 0).ToString();
        string fittest = Mathf.FloorToInt(simCtrl.Fittest.HasValue ? simCtrl.Fittest.Value.fitness : 0).ToString();
        entityText.text = cur + " (" + fittest + ")";
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPS : MonoBehaviour {
    public Text fpsText;
    
	// Update is called once per frame
	void Update () {
        fpsText.text = Mathf.FloorToInt(1 / Time.deltaTime).ToString();
	}
}

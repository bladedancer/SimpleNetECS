using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldController : MonoBehaviour {

    private Controllable possessed;

	void Start () {
		
	}

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Debug.Log(hit.transform.gameObject.name);
                Controllable toPossess = hit.transform.gameObject.GetComponent<Controllable>();
                if (toPossess)
                {
                    if (possessed)
                    {
                        possessed.ReleasePlayerControl();
                    }
                    toPossess.TakePlayerControl();
                    possessed = toPossess;
                }
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using Systems;
using UnityEngine;


public class WorldController : MonoBehaviour {
    public List<GameObject> Prefabs = new List<GameObject>();
    public Transform Container;
    private Controllable possessed;

    private void Awake()
    {
        foreach (GameObject prefab in Prefabs)
        {
            BirthSystem.Prefabs.Add(prefab.tag.GetHashCode(), prefab);
        }
    }

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

using Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Spawn Prefabs.
public class SpawnBase : MonoBehaviour
{
    public SimulationController controller;

    public Transform parent;
    public GameObject prefab;
    public GroundGenerator ground;

    protected void Spawn(int count)
    {
        float widthBound = (ground.width / 2) - 1;
        float lengthBound = (ground.length / 2) - 1;
        for (int i = 0; i < count; ++i)
        {
            Vector3 position = new Vector3(
                Random.Range(-widthBound, widthBound),
                prefab.transform.position.y,
                Random.Range(-lengthBound, lengthBound)
            );
            GameObject obj = Instantiate(prefab, position, Quaternion.identity, parent);
            obj.name = prefab.name + " Gen-0 (" + parent.childCount + ")";

            // TODO GENERIC
            if (controller != null && controller.Fittest.HasValue)
            {
                Fittest fittest = controller.Fittest.Value;
                if (fittest.tag == obj.tag.GetHashCode() && fittest.fitness > 0)
                {
                    obj.GetComponent<HerbivoreController>().InitalNet = fittest.net;
                }
            }
        }
    }
}

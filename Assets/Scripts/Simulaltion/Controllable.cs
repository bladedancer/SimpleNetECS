using Components;
using Unity.Entities;
using UnityEngine;

public class Controllable : MonoBehaviour {
    public Color controlledColor = new Color(255, 128, 128);
    public MaterialController materialController;
    private GameObjectEntity goe;

    public void Start()
    {
        goe = GetComponent<GameObjectEntity>();
    }

    public void TakePlayerControl()
    {
        if (!goe.EntityManager.HasComponent<PlayerInput>(goe.Entity))
        {
            goe.EntityManager.AddComponentData<PlayerInput>(goe.Entity, new PlayerInput());
            materialController.SetBaseColor(controlledColor);
        }
    }

    public void ReleasePlayerControl()
    {
        if (goe.EntityManager.HasComponent<PlayerInput>(goe.Entity))
        {
            goe.EntityManager.RemoveComponent<PlayerInput>(goe.Entity);
            materialController.ResetBaseColor();
        }
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            ReleasePlayerControl();
        }
    }
}

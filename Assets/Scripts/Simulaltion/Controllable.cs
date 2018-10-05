using Components;
using Unity.Entities;
using UnityEngine;

public class Controllable : MonoBehaviour {
    public Color controlledColor = new Color(255, 128, 128);
    public MaterialController materialController;

    public void TakePlayerControl()
    {
        if (GetComponent<PlayerInput>() == null)
        {
            gameObject.AddComponent<PlayerInput>();
            gameObject.SetActive(false);
            gameObject.SetActive(true);
            materialController.SetBaseColor(controlledColor);
        }
    }

    public void ReleasePlayerControl()
    {
        PlayerInput playerInput = GetComponent<PlayerInput>();
        if (playerInput != null)
        {
            DestroyImmediate(playerInput);
            gameObject.SetActive(false);
            gameObject.SetActive(true);
            materialController.ResetBaseColor();
        }
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Escape) && GetComponent<PlayerInput>())
        {
            ReleasePlayerControl();
        }
    }
}

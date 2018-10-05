using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialController : MonoBehaviour {

    MaterialPropertyBlock _propBlock;
    MeshRenderer _renderer;

    public float Seed = 1.5f;
    private Vector4 Offset;
    public Color BaseColor = new Color(240, 240, 240);
    public Color PatternColor = new Color(10, 10, 10);

	void Awake () {
        _propBlock = new MaterialPropertyBlock();
        _renderer = GetComponent<MeshRenderer>();
        _renderer.GetPropertyBlock(_propBlock);
        Offset = new Vector4(
            Random.Range(-10f, 10f),
            Random.Range(-10f, 10f)
        );
    }

    private void Start()
    {
        _propBlock.SetFloat("_Seed", Seed);
        _propBlock.SetVector("_Offset", Offset);
        _propBlock.SetColor("_BaseColor", BaseColor);
        _propBlock.SetColor("_PatternColor", PatternColor);
        _renderer.SetPropertyBlock(_propBlock);
    }

    public void SetBaseColor(Color color)
    {
        _propBlock.SetColor("_BaseColor", color);
        _renderer.SetPropertyBlock(_propBlock);
    }

    public void ResetBaseColor()
    {
        _propBlock.SetColor("_BaseColor", BaseColor);
        _renderer.SetPropertyBlock(_propBlock);
    }

}

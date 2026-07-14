using System;
using UnityEditor;
using UnityEngine;

public class AssemblyTray : MonoBehaviour
{
    public event Action<Collider> OnZoneTriggered;
    public Color color;
    public GameObject AssemblyItem;

    [SerializeField, HideInInspector]
    private Material instanceMaterial;

    Renderer rend;

    private void Start() {
        ApplyColor();
    }

    private void OnValidate() {
        ApplyColor();
    }

    private void OnTriggerEnter(Collider other)
    {
        OnZoneTriggered?.Invoke(other);
    }


    private void ApplyColor() {
        if (rend == null) {
            rend = GetComponent<Renderer>();
        }

        if (rend == null) return;

        if (instanceMaterial == null) {
            instanceMaterial = new Material(rend.sharedMaterial);
            rend.sharedMaterial = instanceMaterial;
        }

        instanceMaterial.color = color;
    }


    public GameObject GetAssemblyItem() {
        return Instantiate(AssemblyItem);
    }

}

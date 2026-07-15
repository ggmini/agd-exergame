using System;
using UnityEditor;
using UnityEngine;

public class AssemblyTray : MonoBehaviour
{
    public event Action<AssemblyTray> OnZoneTriggered;
    public event Action<AssemblyTray> OnZoneExited;
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
        OnZoneTriggered?.Invoke(this);
    }

    private void OnTriggerExit(Collider other)
    {
        OnZoneExited?.Invoke(this);
    }

    private void ApplyColor() {
        if (rend == null) {
            rend = GetComponent<Renderer>();
        }

        if (rend == null || rend.sharedMaterial == null) return;

        if (instanceMaterial == null) {
            instanceMaterial = new Material(rend.sharedMaterial);
            rend.sharedMaterial = instanceMaterial;
        }

        instanceMaterial.color = color;
    }


    public GameObject GetAssemblyItem() {
        GameObject tmp = Instantiate(AssemblyItem);
        //tmp.transform.position = transform.position;

        return tmp;
    }

}

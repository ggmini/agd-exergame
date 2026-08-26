using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandwichAssemblyStation : MonoBehaviour {
    public Action<SandwichAssemblyStation> OnStationCleared;

    public GameObject Table;
    public bool isActiveStation = false;

    protected List<GameObject> items;

    protected bool useMouse;

    protected IPlayerInput playerInput;

    protected virtual void Start() {
        enabled = isActiveStation;
    }

    protected void OnEnable() {
        playerInput = useMouse ? gameObject.AddComponent<MouseInput>() :  gameObject.AddComponent<WebSocketInput>();
    }

    protected void OnDisable() {
        Destroy(playerInput as UnityEngine.Object);
    }
    
    public void toggleIsActiveStation() {
        isActiveStation = !isActiveStation;
        enabled = isActiveStation;
    }

    protected IEnumerator cleanupItems() {
        yield return new WaitForSeconds(3);
        foreach (GameObject item in items)
            Destroy(item);
    }

    public void SetMouse() => useMouse = true;
}

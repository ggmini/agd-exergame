using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseStation : MonoBehaviour {
 
	protected Action<BaseStation> OnStationCleared;

	protected bool useMouse;
	protected IPlayerInput playerInput;

	protected bool isActiveStation = false;

	protected List<GameObject> spawnedItems = new List<GameObject>();

	protected void OnEnable() {
		playerInput = useMouse ? gameObject.AddComponent<MouseInput>() :  gameObject.AddComponent<WebSocketInput>();
		isActiveStation = true;
	}

	protected void OnDisable() {
		Destroy(playerInput as UnityEngine.Object);
	}

	protected IEnumerator cleanupItems() {
		yield return new WaitForSeconds(3);
		foreach (GameObject item in spawnedItems)
			Destroy(item);
	}

	//TODO: try to remove
	public void toggleIsActiveStation() {
		isActiveStation = !isActiveStation;
		enabled = isActiveStation;
	}

	public void SetMouse() => useMouse = true;

	public void AddStationClearedListener(Action<BaseStation> listener) => OnStationCleared += listener;
	
}

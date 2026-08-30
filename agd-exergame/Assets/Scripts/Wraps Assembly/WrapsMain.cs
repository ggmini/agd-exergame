using System;
using UnityEngine;

public class WrapsMain : MonoBehaviour {
	public BaseStation[] Stations;
	public CameraController Camera;

	private int CurrentStationIdx;

	[SerializeField] bool useMouse;

	void Awake() {
		foreach (BaseStation station in Stations)
			if (useMouse) station.SetMouse();
	}

	void Start() {
		foreach (BaseStation station in Stations) {
			station.AddStationClearedListener(HandleStationCleared);
		}
	}

	private void HandleStationCleared(BaseStation Station) {
		Stations[CurrentStationIdx].toggleIsActiveStation();
		CurrentStationIdx++;

		if (CurrentStationIdx >= Stations.Length) return;

		Camera.StartCoroutine(Camera.MoveCamera(Vector3.right, Camera.xDistanceIncrement));
		Stations[CurrentStationIdx].toggleIsActiveStation();
	}
}

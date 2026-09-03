using System;
using System.Collections;
using UnityEngine;

public class MainScene : MonoBehaviour
{
    public SandwichAssemblyStation[] Stations;
    public SandwichCamera Camera;

    private int CurrentStationIdx;

    [SerializeField] bool useMouse;

    void Awake() {
        foreach (SandwichAssemblyStation station in Stations)
            if (useMouse) station.SetMouse();
    }
    
    void Start()
    {
        foreach (SandwichAssemblyStation station in Stations) {
            station.OnStationCleared += HandleStationCleared;
        }
    }

    private void HandleStationCleared(SandwichAssemblyStation Station) {
        Stations[CurrentStationIdx].toggleIsActiveStation();
        CurrentStationIdx++;

        if (CurrentStationIdx >= Stations.Length) return;

        StartCoroutine(Camera.MoveCamera(Vector3.right, Camera.xDistanceIncrement));
        StartCoroutine(activateNextStation());
	}

    IEnumerator activateNextStation() {
        yield return new WaitForSeconds(2);
		Stations[CurrentStationIdx].toggleIsActiveStation();
	}
}

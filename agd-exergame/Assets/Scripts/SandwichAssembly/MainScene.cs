using System;
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

        Camera.StartCoroutine(Camera.MoveCamera(Vector3.right, Camera.xDistanceIncrement));
        Stations[CurrentStationIdx].toggleIsActiveStation();
    }
}

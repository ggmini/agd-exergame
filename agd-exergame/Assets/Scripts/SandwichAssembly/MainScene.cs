using System;
using UnityEngine;

public class MainScene : MonoBehaviour
{
    public SandwichAssemblyStation[] Stations;
    public SandwichCamera Camera;

    private int CurrentStationIdx;

    void Start()
    {
        foreach (SandwichAssemblyStation station in Stations) {
            station.OnStationCleared += HandleStationCleared;
        }
    }

    void Update() {
        if (WebSocketManager.Instance.Msg == null) return;

        Vector3 accel = new Vector3(
            WebSocketManager.Instance.Msg.accel_x,
            WebSocketManager.Instance.Msg.accel_y - 9.81f,
            WebSocketManager.Instance.Msg.accel_z);

        //Debug.Log(accel);
    }

    private void HandleStationCleared(SandwichAssemblyStation Station) {
        Stations[CurrentStationIdx].toggleIsActiveStation();
        CurrentStationIdx++;

        if (CurrentStationIdx >= Stations.Length) return;

        Camera.StartCoroutine(Camera.MoveCamera(Vector3.right, Camera.xDistanceIncrement));
        Stations[CurrentStationIdx].toggleIsActiveStation();

    }
}

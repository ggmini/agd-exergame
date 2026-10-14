using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class AssemblyLine : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public SandwichAssemblyStation[] Stations;
    public SandwichCamera Camera;

    private int CurrentStationIdx;

    private string deviceID;
    public string DeviceID {  get { return deviceID; } }

    [SerializeField] Image blackscreen;
    [SerializeField] bool useMouse;
    private bool lineDone;
    public bool FirstToFinish = true;
    private bool _activated = false;
    public bool activated {
        get { return _activated; }
    }

    public event Action<String> OnAssemblyLineFinished;


    void Awake() {
        foreach (SandwichAssemblyStation station in Stations)
            if (useMouse) station.SetMouse();
    }

    void Start() {
        foreach (SandwichAssemblyStation station in Stations) {
            station.AddStationClearedListener(HandleStationCleared);
        }
    }

    private void HandleStationCleared(BaseStation Station) {
        Stations[CurrentStationIdx].Camera.SetActive(false);
        Stations[CurrentStationIdx].toggleIsActiveStation();
        
        CurrentStationIdx++;
        if (CurrentStationIdx >= Stations.Length) {
            FinishAssemblyLine();
            lineDone = true;
            return;
        
        }
        LeanTween.alpha(blackscreen.rectTransform, 1f, 1f).setOnComplete(() =>
        {
            Stations[CurrentStationIdx].Camera.SetActive(true);
            LeanTween.alpha(blackscreen.rectTransform, 0f, 1f);
        });


        //StartCoroutine(Camera.MoveCamera(Vector3.right, Camera.xDistanceIncrement));
        StartCoroutine(activateNextStation());
    }

    IEnumerator activateNextStation() {
        yield return new WaitForSeconds(2);
        Stations[CurrentStationIdx].toggleIsActiveStation();
    }

    public void ActivateAssemblyLine(string id) {
        Stations[CurrentStationIdx].Camera.SetActive(true);
        deviceID = id;
        _activated = true;
        foreach (SandwichAssemblyStation station in Stations) {
            station.deviceID = deviceID;
        }
        //gameObject.SetActive(true);
    }

    public void DeactivateAssemblyLine() {
        gameObject.SetActive(false);
        deviceID = null;
        _activated = false;
        foreach (SandwichAssemblyStation station in Stations) {
            station.deviceID = null;
        }
    }

    private void FinishAssemblyLine()
    {
        //TODO: add picture or sth.
        OnAssemblyLineFinished?.Invoke(deviceID);
    }


    public void Reset()
    {
        if (!lineDone) Stations[CurrentStationIdx].toggleIsActiveStation();
        CurrentStationIdx = 0;
        Stations[CurrentStationIdx].toggleIsActiveStation();
        Camera.Reset();
        foreach (SandwichAssemblyStation station in Stations)
        {
            station.Reset();
        }
    }
}

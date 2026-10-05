using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class AssemblyLine : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public SandwichAssemblyStation[] Stations;
    public SandwichCamera Camera;

    private int CurrentStationIdx;

    private string deviceID;

    [SerializeField] PauseMenu pauseMenu;
    [SerializeField] PostGameMenu endScreen;

    [SerializeField] bool useMouse;
    bool gameDone;
    private bool _activated = false;
    public bool activated {
        get { return _activated; }
    }

    void Awake() {
        foreach (SandwichAssemblyStation station in Stations)
            if (useMouse) station.SetMouse();
    }

    void Start() {
        foreach (SandwichAssemblyStation station in Stations) {
            station.AddStationClearedListener(HandleStationCleared);
        }
    }

    private void FixedUpdate() {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
            TogglePause();
    }

    private void HandleStationCleared(BaseStation Station) {
        Stations[CurrentStationIdx].toggleIsActiveStation();
        CurrentStationIdx++;

        if (CurrentStationIdx >= Stations.Length) {
            OpenEndScreen();
            gameDone = true;
            return;
        }

        StartCoroutine(Camera.MoveCamera(Vector3.right, Camera.xDistanceIncrement));
        StartCoroutine(activateNextStation());
    }

    IEnumerator activateNextStation() {
        yield return new WaitForSeconds(2);
        Stations[CurrentStationIdx].toggleIsActiveStation();
    }

    public void ActivateAssemblyLine(string id) {
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


    public void TogglePause() {
        //Time.timeScale = Time.timeScale == 0 ? 1 : 0;
        //pauseMenu.gameObject.SetActive(!pauseMenu.gameObject.activeSelf);
    }

    void OpenEndScreen() {
        //endScreen.gameObject.SetActive(true);
    }

    public void Reset() {
        if (!gameDone) Stations[CurrentStationIdx].toggleIsActiveStation();
        CurrentStationIdx = 0;
        Stations[CurrentStationIdx].toggleIsActiveStation();
        Camera.Reset();
        foreach (SandwichAssemblyStation station in Stations) {
            station.Reset();
        }
        if (pauseMenu.gameObject.activeSelf) TogglePause();
        else if (endScreen.gameObject.activeSelf) endScreen.gameObject.SetActive(false);
    }
}

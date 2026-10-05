using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class MainScene : MonoBehaviour {
    public AssemblyLine[] AssemblyLines;
    [SerializeField] PlayerSignUp playerSignUp;
    [SerializeField] PauseMenu pauseMenu;
    [SerializeField] PostGameMenu endScreen;

    //[SerializeField] bool useMouse;
    private bool gameDone;
    private bool gameStarted = false;
    private int activeAssemblyLines = 0;
    private int finishedLines = 0;

    private List<string> finishedAssemblyLines = new List<string>();


    private void Start() {
        playerSignUp.OnDeviceAssigned += OnDeviceAssigned;
        playerSignUp.OnDeviceDeassigned += OnDeviceDeassigned;
        playerSignUp.GameStarted += StartGame;

        foreach (AssemblyLine line in AssemblyLines) {
            line.OnAssemblyLineFinished += OnAssemblyLineFinished;
        }
    }


    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Debug.Log("pressed escape");
            TogglePause();
        }
    }

    private void OnDeviceAssigned(int idx, string id) {
        AssemblyLine obj = AssemblyLines[idx];
        obj.ActivateAssemblyLine(id);
        activeAssemblyLines++;
    }


    private void OnDeviceDeassigned(int idx) {
        AssemblyLine obj = AssemblyLines[idx];
        obj.DeactivateAssemblyLine();
        activeAssemblyLines--;
    }


    private void StartGame() {
        foreach (AssemblyLine line in AssemblyLines) {
            if (line.activated) {
                line.gameObject.SetActive(true);
            }
            gameStarted = true;
        }
    }

    public void TogglePause()
    {
        Debug.Log("toggle pause called");
        Time.timeScale = Time.timeScale == 0 ? 1 : 0;
        pauseMenu.gameObject.SetActive(!pauseMenu.gameObject.activeSelf);
    }

    private void OnAssemblyLineFinished(string id)
    {
        if (finishedAssemblyLines.Contains(id))
        {
            Debug.Log("already finished assembly line tried to double dip");
        }
        finishedAssemblyLines.Add(id);
        finishedLines++;
        if (finishedLines >= activeAssemblyLines)
        {
            Debug.Log("all assembly lines finished");
            gameDone = true;
            OpenEndScreen();
        }
    }

    void OpenEndScreen()
    {
        endScreen.gameObject.SetActive(true);
    }

    public void Reset()
    {
        Debug.Log("called reset");
        Time.timeScale = 1;
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
        //gameStarted = false;
        //gameDone = false;
        //finishedLines = 0;
        //finishedAssemblyLines.Clear();

        //// tell playersignup to reset
        //playerSignUp.Reset();

        //// tell assemblylines to reset
        //foreach (AssemblyLine line in AssemblyLines) {
        //    line.Reset();
        //}

        //// if pause- or end screen is active, deactivate
        //if (pauseMenu.gameObject.activeSelf) TogglePause();
        //else if (endScreen.gameObject.activeSelf) endScreen.gameObject.SetActive(false);
    }



    //void Awake() {
    //	foreach (SandwichAssemblyStation station in Stations)
    //		if (useMouse) station.SetMouse();
    //}

    //void Start() {
    //	foreach (SandwichAssemblyStation station in Stations) {
    //		station.AddStationClearedListener(HandleStationCleared);
    //	}
    //}

    //private void FixedUpdate() {
    //	if (Keyboard.current.escapeKey.wasPressedThisFrame)
    //		TogglePause();
    //}

    //private void HandleStationCleared(BaseStation Station) {
    //	Stations[CurrentStationIdx].toggleIsActiveStation();
    //	CurrentStationIdx++;

    //	if (CurrentStationIdx >= Stations.Length) {
    //		OpenEndScreen();
    //		gameDone = true;
    //		return;
    //	}

    //	StartCoroutine(Camera.MoveCamera(Vector3.right, Camera.xDistanceIncrement));
    //	StartCoroutine(activateNextStation());
    //}

    //IEnumerator activateNextStation() {
    //	yield return new WaitForSeconds(2);
    //	Stations[CurrentStationIdx].toggleIsActiveStation();
    //}

    //public void TogglePause() {
    //	Time.timeScale = Time.timeScale == 0 ? 1 : 0;
    //	pauseMenu.gameObject.SetActive(!pauseMenu.gameObject.activeSelf);
    //}

    //void OpenEndScreen() {
    //	endScreen.gameObject.SetActive(true);
    //}

    //public void Reset() {
    //	if (!gameDone) Stations[CurrentStationIdx].toggleIsActiveStation();
    //	CurrentStationIdx = 0;
    //	Stations[CurrentStationIdx].toggleIsActiveStation();
    //	Camera.Reset();
    //	foreach (SandwichAssemblyStation station in Stations) {
    //		station.Reset();
    //	}
    //	if (pauseMenu.gameObject.activeSelf) TogglePause();
    //	else if(endScreen.gameObject.activeSelf) endScreen.gameObject.SetActive(false);
    //}
}

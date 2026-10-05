using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class MainScene : MonoBehaviour {
	public AssemblyLine[] AssemblyLines;
    //public SandwichCamera Camera;

    //private int CurrentStationIdx;

    [SerializeField] PlayerSignUp playerSignUp;
	[SerializeField] PauseMenu pauseMenu;
	[SerializeField] PostGameMenu endScreen;

	//[SerializeField] bool useMouse;
	bool gameDone;


    private void Start() {
        playerSignUp.OnDeviceAssigned += OnDeviceAssigned;
        playerSignUp.OnDeviceDeassigned += OnDeviceDeassigned;
        playerSignUp.GameStarted += StartGame;
    }


    private void OnDeviceAssigned(int idx, string id) {
        AssemblyLine obj = AssemblyLines[idx];
        obj.ActivateAssemblyLine(id);
    }


    private void OnDeviceDeassigned(int idx) {
        AssemblyLine obj = AssemblyLines[idx];
        obj.DeactivateAssemblyLine();
    }


    private void StartGame() {
        foreach (AssemblyLine line in AssemblyLines) {
            if (line.activated) {
                line.gameObject.SetActive(true);
            }
        }
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

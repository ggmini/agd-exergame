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
    private bool gamePaused = false;
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
        if (isPauseTogglePressed())
        {
            Debug.Log("pressed pause");
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
        if (gameStarted && activeAssemblyLines <= 0) {
            Reset();
        }
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
        gamePaused = !gamePaused;
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
    }

    // NOTE: this might not work bc of timescale
    private bool isPauseTogglePressed() {
        bool isTogglePressed = false;
        foreach (string id in WebSocketManager.Instance.Msg.Keys) {
            if (gamePaused) {
                if (WebSocketManager.Instance.Msg[id].start_pressed) isTogglePressed = true;
            }
            else {
                if (WebSocketManager.Instance.Msg[id].pause_pressed) isTogglePressed = true;
            }
        }

        return isTogglePressed;
    }
}

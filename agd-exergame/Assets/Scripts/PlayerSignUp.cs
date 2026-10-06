using System;
using System.Collections.Concurrent;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerSignUp : MonoBehaviour
{
    const int MAX_PLAYERS = 4;
    private int PlayerCount = 0;

    private float Timer = 0;

    private bool gameStarted = false;

    [SerializeField] private float ConfirmationDuration = 5f;

    private string[] ConnectedDevices = { "", "", "", "" };

    [SerializeField] GameObject[] Images = { };

    public event Action<int, String> OnDeviceAssigned;
    public event Action<int> OnDeviceDeassigned;
    public event Action GameStarted;
    private static readonly ConcurrentQueue<Action> queue = new();
    //public static void Enqueue(Action action) => queue.Enqueue(action);
    private void Enqueue(Action action) => queue.Enqueue(action);


    public void Reset()
    {
        gameStarted = false;
        gameObject.SetActive(true);
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        oldConnectionsCheck();
        WebSocketManager.Instance.OnSocketHandlerOpen += OnDeviceConnected;
        WebSocketManager.Instance.OnSocketHandlerClose += OnDeviceDisconnected;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        while (queue.TryDequeue(out var action)) {
            action();
        }

        if (isReadyPressed()) {
            Timer += Time.deltaTime;
            if (Timer >= ConfirmationDuration && !gameStarted) {
                Debug.Log("Timer exceeded");
                StartGame();
            }
        }
        else {
            if (Timer > 0) {
                Debug.Log("Timer reset");
                Timer = 0;
            }
        }
    }

    private void OnDeviceConnected(string deviceID)
    {
        if (PlayerCount >= MAX_PLAYERS)
        {
            Debug.Log("Tried connecting additional Device, but max Player Count is already reached");
            return;
        }

        // Register device under first free slot
        for (int i=0; i<ConnectedDevices.Length; i++)
        {
            if (ConnectedDevices[i].Equals(""))
            {
                ConnectedDevices[i] = deviceID;
                //OnDeviceAssigned?.Invoke(i, deviceID);
                Enqueue(() => OnDeviceAssigned?.Invoke(i, deviceID));
                break;
            }
        }

        PlayerCount++;
        UpdateCanvas();

    }

    private void OnDeviceDisconnected(string deviceID)
    {
        if (PlayerCount <= 0)
        {
            Debug.Log("Tried disconnecting Device, but there are no connected devices");
            return;
        }

        // Search and remove disconnected device
        for (int i = 0; i < ConnectedDevices.Length; i++)
        {
            if (ConnectedDevices[i].Equals(deviceID))
            {
                ConnectedDevices[i] = "";
                //OnDeviceDeassigned?.Invoke(i);
                Enqueue(() => OnDeviceDeassigned?.Invoke(i));
                break;
            }
        }

        PlayerCount--;
        UpdateCanvas();

    }

    private void UpdateCanvas()
    {
        for (int i = 0; i < Images.Length; i++) {
            int j = i;
            bool connected = !ConnectedDevices[i].Equals("");

            GameObject obj = Images[i];

            Enqueue(() => SetText(obj, j, connected));
            Enqueue(() => SetImage(obj, connected));

            //SetText(obj, i, connected);
            //SetImage(obj, connected);
        }
    }

    private void SetText(GameObject obj, int idx, bool connected) {
        //Debug.Log($"Called with idx {idx}");
        TextMeshProUGUI text = obj.GetComponentInChildren<TextMeshProUGUI>();
        if (connected) {
            text.text = "Player " + idx.ToString() + " connected!";
        } else {
            text.text = "Connect Device to Join";
        }
    }

    private void SetImage(GameObject obj, bool connected) {
        UnityEngine.UI.Image img = obj.GetComponentInChildren<UnityEngine.UI.Image>();
        //TODO: set to check image or sth.
    }

    private void StartGame() {
        if (gameStarted) return;

        Debug.Log("passed check");
        gameStarted = true;
        GetComponent<Canvas>().enabled = false;
        GameStarted?.Invoke();
    }


    // NOTE: unti,ely device disconnect might be a problem
    private bool isReadyPressed() {
        bool isReadyPressed = false;
        foreach (String id in WebSocketManager.Instance.Msg.Keys) {
            if (WebSocketManager.Instance.Msg[id].start_pressed) isReadyPressed = true;
        }

        return isReadyPressed;
    }


    private void oldConnectionsCheck()
    {
        if (WebSocketManager.Instance.Msg.Count <= 0) return;

        foreach (String id in WebSocketManager.Instance.Msg.Keys){
            OnDeviceConnected(id);
        }
    }


}

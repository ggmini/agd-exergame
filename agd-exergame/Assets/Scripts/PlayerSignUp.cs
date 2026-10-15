using System;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;



public class PlayerSignUp : MonoBehaviour
{
    const int MAX_PLAYERS = 4;
    private int PlayerCount = 0;

    private float timer = 0;

    private bool gameStarted = false;

    [SerializeField] private float ConfirmationDuration = 5f;

    private string[] ConnectedDevices = { "", "", "", "" };
    [SerializeField] Image blackscreen;
    [SerializeField] GameObject blackscreensInGame;
    [SerializeField] GameObject connectScreen;

    [SerializeField] PlayerConnectPanel[] PlayerConnectPanels = { };
    [SerializeField] Camera[] SplitScreenCameras = { };
    [SerializeField] TextMeshProUGUI ipText;

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
        ipText.text = "Code eingeben: " + WebSocketManager.Instance.Ip.ToString();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        while (queue.TryDequeue(out var action))
        {
            action();
        }

        if (isReadyPressed())
        {
            timer += Time.deltaTime;
            if (timer >= ConfirmationDuration && !gameStarted)
            {
                Debug.Log("Timer exceeded");
                StartGame();
            }
        }
        else
        {
            if (timer > 0)
            {
                Debug.Log("Timer reset");
                timer = 0;
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
        for (int i = 0; i < ConnectedDevices.Length; i++)
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
        if (CR.r != null)
            CR.r.playerCount = PlayerCount;

        for (int i = 0; i < PlayerConnectPanels.Length; i++)
        {
            //Debug.Log(PlayerConnectPanels[i]);
            int j = i;
            bool connected = !ConnectedDevices[i].Equals("");

            PlayerConnectPanel panel = PlayerConnectPanels[i];

            Enqueue(() => SetText(panel, j, connected));
            Enqueue(() => SetImage(panel, connected));

            //SetText(obj, i, connected);
            //SetImage(obj, connected);
        }
    }

    private void SetText(PlayerConnectPanel panel, int idx, bool connected)
    {
        //Debug.Log($"Called with idx {idx}");
        if (connected)
        {
            panel.connectionText.text = "Spieler " + (idx + 1).ToString() + " verbunden";
            panel.connectionText.color = new Color(0.4509804f, 0.427451f, 0.4196078f, 1);
        }
        else
        {
            panel.connectionText.text = "Spieler nicht verbunden";
            panel.connectionText.color = new Color(0.654902f, 0.6392157f, 0.627451f, 1);
        }
    }

    private void SetImage(PlayerConnectPanel panel, bool connected)
    {
        if (connected)
            panel.decorImage.color = new Color(0.4f, 0.91372f, 0.40392f, 1);
        else
            panel.decorImage.color = new Color(1f, 1f, 1f, 1);
        //TODO: set to check image or sth.
    }

    private void StartGame()
    {
        if (gameStarted) return;

        Debug.Log("passed check");
        gameStarted = true;
        //blackscreen animation
        LeanTween.alpha(blackscreen.rectTransform, 1f, 1f).setOnComplete(() =>
        {
            connectScreen.SetActive(false);
            if (CR.r.playerCount > 2)
            {
                blackscreensInGame.SetActive(true);
            }
            LeanTween.alpha(blackscreen.rectTransform, 0f, 2f);
        });

        SetCameraRects();

        GameStarted?.Invoke();
    }

    private void SetCameraRects()
    {
        //PlayerCount;
        if (PlayerCount == 1)
        {
            SplitScreenCameras[0].rect = new Rect(0f, 0f, 1f, 1f);
            SplitScreenCameras[1].gameObject.SetActive(false);
            SplitScreenCameras[2].gameObject.SetActive(false);
            SplitScreenCameras[3].gameObject.SetActive(false);
        }
        else if (PlayerCount == 2)
        {
            SplitScreenCameras[0].rect = new Rect(0f, 0.5f, 1f, 0.5f);
            SplitScreenCameras[1].rect = new Rect(0f, 0f, 1f, 0.5f);
            SplitScreenCameras[2].rect = new Rect(0f, 0f, 0f, 0f);
            SplitScreenCameras[3].rect = new Rect(0f, 0f, 0f, 0f);
        }
        else if (PlayerCount == 3)
        {

        }
    }


    // NOTE: unti,ely device disconnect might be a problem
    private bool isReadyPressed()
    {
        bool isReadyPressed = false;
        foreach (String id in WebSocketManager.Instance.Msg.Keys)
        {
            if (WebSocketManager.Instance.Msg[id].start_pressed) isReadyPressed = true;
        }

        return isReadyPressed;
    }


    private void oldConnectionsCheck()
    {
        if (WebSocketManager.Instance.Msg.Count <= 0) return;

        foreach (String id in WebSocketManager.Instance.Msg.Keys)
        {
            OnDeviceConnected(id);
        }
    }


}

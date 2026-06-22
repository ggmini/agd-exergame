using System;
using System.Net;
using UnityEngine;
using WebSocketSharp;
using WebSocketSharp.Server;
using SimpleJSON;

public class SafeServer : MonoBehaviour
{

    public GameObject cube;
    [SerializeField]
    string ip;
    [SerializeField]
    int port;

    float roll;

    WebSocketServer wssv;
        
    void Start()
    {
        if (ip == "-1") {
            string strHostName = Dns.GetHostName();
            IPHostEntry ipEntry = Dns.GetHostEntry(strHostName);
            IPAddress[] addr = ipEntry.AddressList;

            ip = addr[0].ToString();
        }

        Debug.Log("Starting Server");
        string fullAddress = "ws://" + ip + ":" + Convert.ToInt32(port);
        wssv = new WebSocketServer(fullAddress);
        wssv.AddWebSocketService<SafeGame>("/safe");
        WebSocketHub.OnMessageReceived += OnSafeGameMessage;

        wssv.Start();
        Console.ReadKey(true);
    }

    private void Update()
    {
        Vector3 rot = new Vector3(0f, 0f, roll * Mathf.Rad2Deg);
        cube.transform.eulerAngles = rot;
        //Debug.Log(cube.transform.eulerAngles);
    }

    void OnSafeGameMessage(string message)
    {
        Message msg = JsonUtility.FromJson<Message>(message);
        roll = msg.roll;
    }

    private void OnApplicationQuit()
    {
        wssv.Stop();
    }
}

[Serializable]
public class Message
{
    public float roll;
}


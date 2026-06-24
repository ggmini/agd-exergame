using System;
using System.Collections.Generic;
using System.Net;
using UnityEditor.Search;
using UnityEngine;
using WebSocketSharp.Server;

public class WebSocketManager : MonoBehaviour
{

    [SerializeField]
    string ip;
    [SerializeField]
    int port;

    private static WebSocketManager instance;

    private WebSocketMessage msg;
    public WebSocketMessage Msg { get { return msg; }}
    
    WebSocketServer wssv;

    public static WebSocketManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindAnyObjectByType<WebSocketManager>();

                if (instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(WebSocketManager).Name;
                    instance = obj.AddComponent<WebSocketManager>();
                }
            }
            return instance;
        }
    }


    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }


    void Start()
    {
        DontDestroyOnLoad(gameObject);
        
        if (ip == "-1")
        {
            string strHostName = Dns.GetHostName();
            IPHostEntry ipEntry = Dns.GetHostEntry(strHostName);
            List<IPAddress> addr = new();
            IPAddress[] entries = ipEntry.AddressList;

            foreach (IPAddress entry in ipEntry.AddressList)
            {
                if (entry.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    addr.Add(entry);
            }
            ip = addr[0].ToString();
        }

        Debug.Log("Starting Server");
        string fullAddress = "ws://" + ip + ":" + Convert.ToInt32(port);
        wssv = new WebSocketServer(fullAddress);
        wssv.AddWebSocketService<WebSocketMessageHandler>("/general");
        //WebSocketHub.OnMessageReceived += OnSafeGameMessage;

        wssv.Start();
        Console.ReadKey(true);
    }

    public void OnMessage(string message)
    {
        msg = JsonUtility.FromJson<WebSocketMessage>(message);
    }

    private void OnApplicationQuit()
    {
        wssv.Stop();
    }
}


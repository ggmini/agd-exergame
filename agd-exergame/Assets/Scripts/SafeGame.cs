using UnityEngine;
using WebSocketSharp.Server;
using WebSocketSharp;

public class SafeGame : WebSocketBehavior
{
    protected override void OnMessage(MessageEventArgs e)
    {
        //Debug.Log(e.Data);
        WebSocketHub.OnMessageReceived?.Invoke(e.Data);
    }

    protected override void OnOpen()
    {
        base.OnOpen();
        Debug.Log("Device connected");
    }
}
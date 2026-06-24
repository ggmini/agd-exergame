using UnityEngine;
using WebSocketSharp.Server;
using WebSocketSharp;

public class WebSocketMessageHandler : WebSocketBehavior {
    protected override void OnMessage(MessageEventArgs e) {
        //Debug.Log(e.Data);
        WebSocketManager.Instance.OnMessage(e.Data);
    }

    protected override void OnOpen() {
        base.OnOpen();
        Debug.Log("Device connected");
    }
}
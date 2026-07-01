using System;
using NUnit.Framework.Internal;
using UnityEngine;
using WebSocketSharp.Server;
using WebSocketSharp;

public class WebSocketMessageHandler : WebSocketBehavior {
    protected override void OnMessage(MessageEventArgs e) {
        if (e.Data == "Test packet")
            return;
        WebSocketManager.Instance.OnMessage(e.Data);
    }

    protected override void OnOpen() {
        base.OnOpen();
        Debug.Log("Device connected");
    }
    
    protected override void OnClose(CloseEventArgs e) {
        Debug.Log(e.Reason);
    }

    protected override void OnError(ErrorEventArgs e) {
        Debug.Log(e.Exception);
    }
}
using System;
using NUnit.Framework.Internal;
using UnityEngine;
using WebSocketSharp.Server;
using WebSocketSharp;

public class WebSocketMessageHandler : WebSocketBehavior {
    protected override void OnMessage(MessageEventArgs e) {
        if (e.Data == "Test packet")
            return;
        WebSocketManager.Instance.OnMessage(ID, e.Data);
    }

    protected override void OnOpen() {
        base.OnOpen();
        Debug.Log($"Device connected: {ID}");
        WebSocketManager.Instance.OnOpen(ID);
    }

    protected override void OnClose(CloseEventArgs e) {
        Debug.Log(e.Reason);
        WebSocketManager.Instance.OnClose(ID);
    }

    protected override void OnError(ErrorEventArgs e) {
        Debug.Log(e.Exception);
    }
}
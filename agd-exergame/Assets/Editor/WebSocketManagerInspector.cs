using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WebSocketManager))]
public class WebSocketManagerInspector : Editor
{
    public override void OnInspectorGUI() 
    {
        base.OnInspectorGUI();
        WebSocketManager webSocketManager = (WebSocketManager)target;

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Restart Server"))
        {
            webSocketManager.Restart();
        }

        GUILayout.EndHorizontal();

    }
}

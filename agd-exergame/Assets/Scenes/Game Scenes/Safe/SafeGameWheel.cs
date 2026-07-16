using UnityEngine;

public class SafeGameWheel : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update() {
        if (WebSocketManager.Instance.Msg == null) return;

        Vector3 accel = new Vector3(
            WebSocketManager.Instance.Msg.accel_x,
            WebSocketManager.Instance.Msg.accel_y,
            WebSocketManager.Instance.Msg.accel_z);
        
        Debug.Log(accel);
    }
}

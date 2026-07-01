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

        Vector3 rot = new Vector3(0.0f, 0.0f, WebSocketManager.Instance.Msg.roll * Mathf.Rad2Deg);
        transform.eulerAngles = rot;
    }
}

using UnityEngine;

public class BallRoll : MonoBehaviour {

    void Update() {
        var msg = WebSocketManager.Instance.Msg;
        if (msg == null)
            return;
        float roll = msg.roll;
        float pitch = msg.pitch;
        transform.eulerAngles = new Vector3(pitch, roll, 0);
    }

}

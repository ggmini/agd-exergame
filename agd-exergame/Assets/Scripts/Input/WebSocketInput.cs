using UnityEngine;

public class WebSocketInput : MonoBehaviour, PlayerInput {
	#region Accelerometer
	//TODO: dynamic gravity compensation based on device orientation (requires gyro)
	public Vector3 GetAccel() {
		if (WebSocketManager.Instance.Msg == null) return Vector3.zero;
		return new(WebSocketManager.Instance.Msg.accel_x, WebSocketManager.Instance.Msg.accel_y + 9.81f, WebSocketManager.Instance.Msg.accel_z);
	}

	public float GetAccelX() {
		if (WebSocketManager.Instance.Msg == null) return 0;
		return WebSocketManager.Instance.Msg.accel_x;
	}

	public float GetAccelY() {
		if (WebSocketManager.Instance.Msg == null) return 0;
		return WebSocketManager.Instance.Msg.accel_y + 9.81f;
	}
	#endregion Accelerometer

	public bool GetButtonPressed() {
		if (WebSocketManager.Instance.Msg == null) return false;
		return WebSocketManager.Instance.Msg.button_pressed;
	}
}

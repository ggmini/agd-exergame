using UnityEngine;

public class WebSocketInput : MonoBehaviour, IPlayerInput {
	
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

	public float GetAccelZ() {
		if (WebSocketManager.Instance.Msg == null) return 0;
		return WebSocketManager.Instance.Msg.accel_z;
	}
	#endregion Accelerometer

	#region Gyroscope
	public Vector3 GetGyro() {
		if (WebSocketManager.Instance.Msg == null) return Vector3.zero;
		return new Vector3(WebSocketManager.Instance.Msg.pitch, WebSocketManager.Instance.Msg.yaw, WebSocketManager.Instance.Msg.roll);
	}

	public float GetPitch() {
		if (WebSocketManager.Instance.Msg == null) return 0;
		return WebSocketManager.Instance.Msg.pitch;
	}

	public float GetYaw() {
		if (WebSocketManager.Instance.Msg == null) return 0;
		return WebSocketManager.Instance.Msg.yaw;
	}

	public float GetRoll() {
		if (WebSocketManager.Instance.Msg == null) return 0;
		return WebSocketManager.Instance.Msg.roll;
	}
	#endregion Gyroscope

	public bool GetButtonPressed() {
		if (WebSocketManager.Instance.Msg == null) return false;
		return WebSocketManager.Instance.Msg.button_pressed;
	}
}

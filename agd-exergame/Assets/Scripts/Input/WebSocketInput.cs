using System;
using UnityEngine;

public class WebSocketInput : MonoBehaviour, IPlayerInput {
	
	#region Accelerometer
	//TODO: dynamic gravity compensation based on device orientation (requires gyro)
	public Vector3 GetAccel(String deviceID) {
		if (WebSocketManager.Instance.Msg == null) return Vector3.zero;
		return new(WebSocketManager.Instance.Msg[deviceID].accel_x, WebSocketManager.Instance.Msg[deviceID].accel_y + 9.81f, WebSocketManager.Instance.Msg[deviceID].accel_z);
	}

	public float GetAccelX(String deviceID) {
		if (WebSocketManager.Instance.Msg == null) return 0;
		return WebSocketManager.Instance.Msg[deviceID].accel_x;
	}

	public float GetAccelY(String deviceID) {
		if (WebSocketManager.Instance.Msg == null) return 0;
		return WebSocketManager.Instance.Msg[deviceID].accel_y + 9.81f;
	}

	public float GetAccelZ(String deviceID) {
		if (WebSocketManager.Instance.Msg == null) return 0;
		return WebSocketManager.Instance.Msg[deviceID].accel_z;
	}
	#endregion Accelerometer

	#region Gyroscope
	public Vector3 GetGyro(String deviceID) {
		if (WebSocketManager.Instance.Msg == null) return Vector3.zero;
		return new Vector3(WebSocketManager.Instance.Msg[deviceID].pitch, WebSocketManager.Instance.Msg[deviceID].yaw, WebSocketManager.Instance.Msg[deviceID].roll);
	}

	public float GetPitch(String deviceID) {
		if (WebSocketManager.Instance.Msg == null) return 0;
		return WebSocketManager.Instance.Msg[deviceID].pitch;
	}

	public float GetYaw(String deviceID) {
		if (WebSocketManager.Instance.Msg == null) return 0;
		return WebSocketManager.Instance.Msg[deviceID].yaw;
	}

	public float GetRoll(String deviceID) {
		if (WebSocketManager.Instance.Msg == null) return 0;
		return WebSocketManager.Instance.Msg[deviceID].roll;
	}
	#endregion Gyroscope

	public bool GetButtonPressed(String deviceID) {
		if (WebSocketManager.Instance.Msg == null) return false;
		return WebSocketManager.Instance.Msg[deviceID].button_pressed;
	}

    private bool DoesMessageExist(String deviceID)
    {
        return (
            WebSocketManager.Instance.Msg != null &&
            WebSocketManager.Instance.Msg.ContainsKey(deviceID) &&
            WebSocketManager.Instance.Msg[deviceID] != null);
    }
}

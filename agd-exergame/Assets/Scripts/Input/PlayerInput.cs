using System;
using UnityEngine;

public interface IPlayerInput
{
	#region Accelerometer
	public Vector3 GetAccel(String deviceID);
	public float GetAccelX(String deviceID);
	public float GetAccelY(String deviceID);
	#endregion Accelerometer

	#region Gyroscope
	public Vector3 GetGyro(String deviceID);
	public float GetPitch(String deviceID);
	public float GetYaw(String deviceID);
	public float GetRoll(String deviceID);
	#endregion Gyroscope


	public bool GetButtonPressed(String deviceID);
}

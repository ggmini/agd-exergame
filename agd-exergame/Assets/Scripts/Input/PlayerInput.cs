using UnityEngine;

public interface IPlayerInput
{
	#region Accelerometer
	public Vector3 GetAccel();
	public float GetAccelX();
	public float GetAccelY();
	#endregion Accelerometer

	#region Gyroscope
	public Vector3 GetGyro();
	public float GetPitch();
	public float GetYaw();
	public float GetRoll();
	#endregion Gyroscope


	public bool GetButtonPressed();
}

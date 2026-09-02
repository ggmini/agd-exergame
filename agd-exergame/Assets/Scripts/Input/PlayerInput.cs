using UnityEngine;

public interface IPlayerInput
{
	#region Accelerometer
	public Vector3 GetAccel();
	public float GetAccelX();
	public float GetAccelY();
	public float GetAccelZ();
	#endregion Accelerometer

	public bool GetButtonPressed();
}

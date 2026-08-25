using UnityEngine;

public interface PlayerInput
{
	#region Accelerometer
	public Vector3 GetAccel();
	public float GetAccelX();
	public float GetAccelY();
	#endregion Accelerometer

	public bool GetButtonPressed();
}

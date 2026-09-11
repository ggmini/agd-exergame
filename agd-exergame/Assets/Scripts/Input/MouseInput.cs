using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseInput : MonoBehaviour, IPlayerInput
{
    Vector2 accumDelta =  Vector2.zero;
    Vector3 gyro = Vector3.zero;


	void Update() {
        accumDelta += Mouse.current.delta.ReadValue();

        if (Keyboard.current.aKey.isPressed) gyro.x += 1f;
		if (Keyboard.current.dKey.isPressed) gyro.x -= 1f;
        if (Keyboard.current.wKey.isPressed) gyro.y += 1f;
        if (Keyboard.current.sKey.isPressed) gyro.y -= 1f;
        if (Keyboard.current.qKey.isPressed) gyro.z += 1f;
        if (Keyboard.current.eKey.isPressed) gyro.z -= 1f;
	}
    
    #region Accelorometer

    public Vector3 GetAccel(String deviceID) {
        var ret = accumDelta;
        accumDelta = Vector2.zero;
        return ret;
    }

    public float GetAccelX(String deviceID) {
        var ret = accumDelta.x;
        accumDelta = Vector2.zero;
        return ret;
    }

    public float GetAccelY(String deviceID) {
        var ret = accumDelta.y;
        accumDelta = Vector2.zero;
        return ret;
    }
	#endregion Accelorometer

	#region Gyroscope
	//TODO: implement mouse gyro emulation
	public Vector3 GetGyro(String deviceID) {
        return gyro;
	}
    public float GetPitch(String deviceID) {
        return gyro.x;
	}
    public float GetYaw(String deviceID) {
		return gyro.y;
	}
    public float GetRoll(String deviceID) {
		return gyro.z;
	}
	#endregion

	public bool GetButtonPressed(String deviceID) {
        return Mouse.current.leftButton.wasPressedThisFrame;
    }
}

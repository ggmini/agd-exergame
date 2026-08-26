using UnityEngine;
using UnityEngine.InputSystem;

public class MouseInput : MonoBehaviour, IPlayerInput
{
    Vector2 accumDelta =  Vector2.zero;

    void Update() {
        accumDelta += Mouse.current.delta.ReadValue();
    }
    
    #region Accelorometer

    public Vector3 GetAccel() {
        var ret = accumDelta;
        accumDelta = Vector2.zero;
        return ret;
    }

    public float GetAccelX() {
        var ret = accumDelta.x;
        accumDelta = Vector2.zero;
        return ret;
    }

    public float GetAccelY() {
        var ret = accumDelta.y;
        accumDelta = Vector2.zero;
        return ret;
    }
    #endregion Accelorometer

    public bool GetButtonPressed() {
        return Mouse.current.leftButton.wasPressedThisFrame;
    }
}

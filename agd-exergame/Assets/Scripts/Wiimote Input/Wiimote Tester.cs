using UnityEngine;
using WiimoteApi;

public class WiimoteTester : MonoBehaviour {

    [SerializeField]
    WiimoteInput input;

    void Update() {
        if (!input.IsActive) return;
        transform.localRotation = input.Rotation;
        input.UpdateGestureDetection(input.Acceleration, input.Rotation);
    }

}

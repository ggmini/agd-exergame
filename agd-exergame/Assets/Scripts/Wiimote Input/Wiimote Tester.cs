using UnityEngine;
using WiimoteApi;

public class WiimoteTester : MonoBehaviour {

    [SerializeField]
    WiimoteInput input;

    [SerializeField]
    bool readAccel;

    Rigidbody rb;

    void Start() {
        rb = GetComponent<Rigidbody>();
    }

    void Update() {
        if (!input.IsActive) return;
        transform.localRotation = input.Rotation;
        if (readAccel) {
            Vector3 vel = rb.linearVelocity;
            vel.x = input.GetAccelX();
			rb.linearVelocity = vel;
		}
    }

}

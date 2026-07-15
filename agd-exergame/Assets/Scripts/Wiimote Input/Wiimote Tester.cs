using UnityEngine;
using WiimoteApi;

public class WiimoteTester : MonoBehaviour {

    [SerializeField]
    WiimoteInput input;

    [SerializeField]
    bool readAccel;

    [SerializeField]
	[Range(0f, 5f)]
	float accelSensitivity = 1f;

    [SerializeField]
    [Range(0f, 5f)]
    float maxSpeed = 1f;

	[SerializeField]
    [Range(0f, 1f)]
    float threshold = 0.2f;

	Rigidbody rb;

    void Start() {
        rb = GetComponent<Rigidbody>();
    }

    void Update() {
        if (!input.IsActive) return;
        transform.localRotation = input.Rotation;
        if (readAccel) {
            Vector3 vel = rb.linearVelocity;
            float x = input.GetAccelX();
            if (x > threshold || x < -threshold) {
                vel.x += x * accelSensitivity * accelSensitivity;
                vel.x = Mathf.Clamp(vel.x, -maxSpeed, maxSpeed);
                Debug.Log(vel.x);
                rb.linearVelocity = vel;
            } else
                rb.linearVelocity = Vector3.zero;
        }
    }

}

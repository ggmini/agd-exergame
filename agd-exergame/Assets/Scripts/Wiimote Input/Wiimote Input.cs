using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Windows;
using WiimoteApi;

public class WiimoteInput : MonoBehaviour {

	public enum CubeDirection { Top, Bottom, Left, Right, Forward, Back }

	Quaternion initialRotation;

	Wiimote wiimote = null;
	Vector3 wmpOffset = Vector3.zero;

	Vector3 correction = Vector3.zero;
	Vector3 accelCorrection = Vector3.zero;

	Quaternion rotation;
	Vector3 acceleration = Vector3.zero;
	public Quaternion Rotation { get => rotation; }
	public Vector3 Acceleration { get => acceleration; }

	bool isActive = false;
	public bool IsActive { get => isActive; }

	[SerializeField]
	bool isPressedA = false;
	public bool IsPressedA { get => isPressedA; }
	[SerializeField]
	bool isPressedB = false;
	public bool IsPressedB { get => isPressedB; }

	void Start() {
		initialRotation = transform.localRotation;
	}

	void Update() {
		if (!isActive) {
			return;
		}

		int ret;

		do {
			ret = wiimote.ReadWiimoteData();

			if (ret > 0 && wiimote.current_ext == ExtensionController.MOTIONPLUS) {
				var offset = new Vector3(wiimote.MotionPlus.PitchSpeed, -wiimote.MotionPlus.YawSpeed, wiimote.MotionPlus.RollSpeed) / 95f; //Divide by 95Hz (average updates per second from wiimote)
				offset -= correction;
				wmpOffset += offset;

				rotate(offset);

			} //else Debug.Log("Software development is just being gaslit as your job");
		} while (ret > 0);

		isPressedA = wiimote.Button.a;
		isPressedB = wiimote.Button.b;

		//Other IR stuff

		if (wiimote.current_ext != ExtensionController.MOTIONPLUS)
			rotation = initialRotation;

		//IR Stuff?
		acceleration = GetAccelVector() - accelCorrection;
	}

	void activateAccelorometer() {
		wiimote.SendDataReportMode(InputDataType.REPORT_BUTTONS_ACCEL_EXT16);
		CalibrateAccelerometer();
	}

	public void CalibrateAccelerometer() {
		accelCorrection = GetAccelVector();
		Debug.Log(accelCorrection);
	}

	Vector3 GetAccelVector() {
		float accel_x;
		float accel_y;
		float accel_z;

		float[] accel = wiimote.Accel.GetCalibratedAccelData();
		accel_x = accel[0];
		accel_y = -accel[2];
		accel_z = -accel[1];

		return new Vector3(accel_x, accel_y, accel_z).normalized;
	}

	void rotate(Vector3 offset) {
		Quaternion rotationQ = Quaternion.Euler(offset);
		rotation *= rotationQ;
	}

	public void CalibrateGyro() {
		correction = new Vector3(wiimote.MotionPlus.PitchSpeed, -wiimote.MotionPlus.YawSpeed, wiimote.MotionPlus.RollSpeed) / 95;
		rotation = initialRotation;
	}

	private void OnApplicationQuit() {
		if (wiimote != null) {
			WiimoteManager.Cleanup(wiimote);
			wiimote = null;
		}
	}

	void OnGUI() {
		GUI.Box(new Rect(0, 0, 320, Screen.height), "");

		GUILayout.BeginVertical(GUILayout.Width(300));
		GUILayout.Label("Wiimote Found: " + WiimoteManager.HasWiimote());
		if (GUILayout.Button("Find Wiimote")) {
			WiimoteManager.FindWiimotes();
			if (WiimoteManager.HasWiimote()) {
				wiimote = WiimoteManager.Wiimotes[0];
				wiimote.SendPlayerLED(true, false, false, false);
				isActive = true;
			}
		}
		if (wiimote != null) {
			GUILayout.Label("WMP Attached: " + wiimote.wmp_attached);
			if (GUILayout.Button("Request Identify WMP"))
				wiimote.RequestIdentifyWiiMotionPlus();
			if ((wiimote.wmp_attached || wiimote.Type == WiimoteType.WIIMOTEPLUS || wiimote.Type == WiimoteType.PROCONTROLLER) && GUILayout.Button("Activate WMP")) {
				wiimote.ActivateWiiMotionPlus();
				wiimote.SetupIRCamera(IRDataType.BASIC);
			}
			if ((wiimote.current_ext == ExtensionController.MOTIONPLUS || wiimote.current_ext == ExtensionController.MOTIONPLUS_CLASSIC ||
					wiimote.current_ext == ExtensionController.MOTIONPLUS_NUNCHUCK) && GUILayout.Button("Deactivate WMP"))
				wiimote.DeactivateWiiMotionPlus();
			if ((wiimote.current_ext == ExtensionController.MOTIONPLUS || wiimote.current_ext == ExtensionController.MOTIONPLUS_CLASSIC ||
					wiimote.current_ext == ExtensionController.MOTIONPLUS_NUNCHUCK) && GUILayout.Button("Calibrate WMP"))
				CalibrateGyro();
		}

		if (GUILayout.Button("B/A/Ext16", GUILayout.Width(300 / 4)))
			activateAccelorometer();

		GUILayout.Label("Calibrate Accelerometer");
		GUILayout.BeginHorizontal();
		for (int x = 0; x < 3; x++) {
			AccelCalibrationStep step = (AccelCalibrationStep)x;
			if (GUILayout.Button(step.ToString(), GUILayout.Width(100))) {
				wiimote.Accel.CalibrateAccel(step);
				gravityEstimate = GetAccelVector();
			}
		}
		GUILayout.EndHorizontal();

		if (GUILayout.Button("Zero Accel"))
			CalibrateAccelerometer();


		GUILayout.EndVertical();
	}

	[Header("Filter Tuning")]
	[Tooltip("How fast the filter adapts to a new orientation. Lower = slower adaptation but cleaner swings.")]
	public float filterStrength = 5f;

	[Tooltip("The minimum force (in Gs) to trigger a gesture.")]
	public float gestureThreshold = 1.5f;
	public float gestureCooldown = 0.3f;

	[Header("Events")]
	public UnityEvent OnSwingForward;
	public UnityEvent OnSwingLeft;
	public UnityEvent OnSwingRight;
	public UnityEvent OnSwingUp;

	private Vector3 gravityEstimate = Vector3.zero;
	private float cooldownTimer = 0f;

	/// <summary>
	/// Call this in your Update loop. No complex rotation calculations required!
	/// </summary>
	public void UpdateGestureDetection(Vector3 rawAccel, Quaternion currentRotation) {
		float dt = Time.deltaTime;

		if (cooldownTimer > 0) {
			cooldownTimer -= dt;
		}

		// 1. High-Pass Filter: Slowly slide our gravity estimate toward the current reading.
		// This acts as a low-pass filter (capturing only slow/static gravity).
		gravityEstimate = Vector3.Lerp(gravityEstimate, rawAccel, dt * filterStrength);

		// 2. Subtract the low-pass gravity estimate from the raw reading.
		// This isolates only the sudden, high-frequency physical movements!
		Vector3 cleanLocalMotion = rawAccel - gravityEstimate;

		// 3. Convert that clean motion vector to World Space so directions match your room
		Vector3 worldMotion = currentRotation * cleanLocalMotion;

		// 4. If we aren't cooling down, check if the motion exceeds our threshold
		if (cooldownTimer <= 0 && worldMotion.magnitude > gestureThreshold) {
			AnalyzeWorldGesture(worldMotion, worldMotion.magnitude);
			cooldownTimer = gestureCooldown;
		}
	}

	private void AnalyzeWorldGesture(Vector3 worldMotion, float force) {
		Vector3 direction = worldMotion.normalized;

		float absX = Mathf.Abs(direction.x);
		float absY = Mathf.Abs(direction.y);
		float absZ = Mathf.Abs(direction.z);

		if (absZ > absX && absZ > absY) {
			if (direction.z > 0) {
				Debug.Log($"Forward Punch! Force: {force:F2}G");
				OnSwingForward.Invoke();
			}
		} else if (absX > absY && absX > absZ) {
			if (direction.x > 0) {
				Debug.Log($"Right Swipe! Force: {force:F2}G");
				OnSwingRight.Invoke();
			} else {
				Debug.Log($"Left Swipe! Force: {force:F2}G");
				OnSwingLeft.Invoke();
			}
		} else if (absY > absX && absY > absZ) {
			if (direction.y > 0) {
				Debug.Log($"Upward Swing! Force: {force:F2}G");
				OnSwingUp.Invoke();
			}
		}
	}

}
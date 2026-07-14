using System.Text;
using UnityEngine;
using WiimoteApi;

public class Wiimote_Input : MonoBehaviour {

	Quaternion initialRotation;

	[SerializeField]
	Wiimote wiimote;
	Vector3 wmpOffset = Vector3.zero;

	Quaternion rotation;
	Vector3 acceleration = Vector3.zero;
	public Quaternion Rotation { get => rotation; }
	public Vector3 Acceleration { get => acceleration; }

	bool isActive = false;
	public bool IsActive { get => isActive; }

	void Start() {
		initialRotation = transform.localRotation;
	}

	void Update() {
		if (!isActive) {
			tryGetWiimote();
			return;
		}

		isActive = true;

		int ret;

		do {
			ret = wiimote.ReadWiimoteData();

			if (ret > 0 && wiimote.current_ext == ExtensionController.MOTIONPLUS) {
				var offset = new Vector3(0 /*-wiimote.MotionPlus.YawSpeed*/, wiimote.MotionPlus.PitchSpeed, wiimote.MotionPlus.RollSpeed) / 95f; //Divide by 95Hz (average updates per second from wiimote)
				wmpOffset += offset;

				rotate(offset);
				Debug.Log(offset);
				Debug.Log(wiimote.MotionPlus.YawSlow);

			} //else Debug.Log("Software development is just being gaslit as your job");
		} while (ret > 0);

		if (wiimote.Button.a)
			Debug.Log("A Pressed");
		/*
		model.a.enabled = wiimote.Button.a;
		model.b.enabled = wiimote.Button.b;
		model.one.enabled = wiimote.Button.one;
		model.two.enabled = wiimote.Button.two;
		model.d_up.enabled = wiimote.Button.d_up;
		model.d_down.enabled = wiimote.Button.d_down;
		model.d_left.enabled = wiimote.Button.d_left;
		model.d_right.enabled = wiimote.Button.d_right;
		model.plus.enabled = wiimote.Button.plus;
		model.minus.enabled = wiimote.Button.minus;
		model.home.enabled = wiimote.Button.home;
		*/

		if (wiimote.current_ext != ExtensionController.MOTIONPLUS)
			rotation = initialRotation;

		//IR Stuff?

		acceleration = GetAccelVector();
	}

	void tryGetWiimote() {
		if (WiimoteManager.HasWiimote()) {
			wiimote = WiimoteManager.Wiimotes[0];
			isActive = true;
		}
	}

	public void CalibrateAccelerometer() {
		for (int x = 0; x < 3; x++) {
			AccelCalibrationStep step = (AccelCalibrationStep)x;
			if (GUILayout.Button(step.ToString(), GUILayout.Width(100)))
				wiimote.Accel.CalibrateAccel(step);
		}

		StringBuilder str = new StringBuilder();
		for (int x = 0; x < 3; x++) {
			for (int y = 0; y < 3; y++) {
				str.Append(wiimote.Accel.accel_calib[y, x]).Append(" ");
			}
			str.Append("\n");
		}
		Debug.Log(str.ToString());
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

	void rotate(Vector3 eulers, Space relativeTo = Space.Self) {
		Quaternion quaternion = Quaternion.Euler(eulers.x, eulers.y, eulers.z);
		if (relativeTo == Space.Self) {
			transform.localRotation *= quaternion;
		} else {
			transform.rotation *= Quaternion.Inverse(transform.rotation) * quaternion * transform.rotation;
		}
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
		if (GUILayout.Button("Find Wiimote"))
			WiimoteManager.FindWiimotes();
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
		}
		GUILayout.EndVertical();
	}
}

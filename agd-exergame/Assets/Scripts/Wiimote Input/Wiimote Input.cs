using System;
using System.Text;
using UnityEngine;
using WiimoteApi;

public class WiimoteInput : MonoBehaviour {

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
				var offset = new Vector3(-wiimote.MotionPlus.YawSpeed, wiimote.MotionPlus.PitchSpeed, wiimote.MotionPlus.RollSpeed) / 95f; //Divide by 95Hz (average updates per second from wiimote)
				offset -= correction;
				wmpOffset += offset;

				rotate(offset);

			} //else Debug.Log("Software development is just being gaslit as your job");
		} while (ret > 0);

		isPressedA = wiimote.Button.a;
		isPressedB = wiimote.Button.b;

		/*
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
        acceleration = GetAccelVector() - accelCorrection;
        Debug.Log(acceleration);
    }

	void activateAccelorometer() {
        wiimote.SendDataReportMode(InputDataType.REPORT_BUTTONS_ACCEL_EXT16);
        CalibrateAccelerometer();
    }

	public void CalibrateAccelerometer() {
		accelCorrection = GetAccelVector();
		Debug.Log(accelCorrection);
	}

	public float GetAccelX() {
		float accel = wiimote.Accel.GetCalibratedAccelData()[0];
		return accel;
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
		correction = new Vector3(-wiimote.MotionPlus.YawSpeed, wiimote.MotionPlus.PitchSpeed, wiimote.MotionPlus.RollSpeed) / 95;
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
            if (GUILayout.Button(step.ToString(), GUILayout.Width(100)))
                wiimote.Accel.CalibrateAccel(step);
        }
        GUILayout.EndHorizontal();

        if (GUILayout.Button("Zero Accel"))
			CalibrateAccelerometer();


        GUILayout.EndVertical();
	}
}

using System.Text;
using UnityEngine;
using WiimoteApi;

public class Wiimote_Input : MonoBehaviour
{

	Quaternion initialRotation;

	Wiimote wiimote;
	Vector3 wmpOffset = Vector3.zero;

	void Start()
	{
		initialRotation = transform.localRotation;
	}

	void Update()
	{
		if (!WiimoteManager.HasWiimote()) return;
		
		wiimote = WiimoteManager.Wiimotes[0];

		int ret;

		do {
			ret = wiimote.ReadWiimoteData();

			if (ret > 0 && wiimote.current_ext == ExtensionController.MOTIONPLUS) {
				var offset = new Vector3(wiimote.MotionPlus.PitchSpeed, -wiimote.MotionPlus.YawSpeed, -wiimote.MotionPlus.RollSpeed) / 2; //Divide by 95Hz (average updates per seconf from wiimote)
				wmpOffset += offset;

				transform.Rotate(offset, Space.Self);
			}
		} while (ret > 0);

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
			transform.localRotation = initialRotation;

		//IR Stuff?


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

	private void OnApplicationQuit() {
		if (wiimote != null) {
			WiimoteManager.Cleanup(wiimote);
			wiimote = null;
		}
	}


}

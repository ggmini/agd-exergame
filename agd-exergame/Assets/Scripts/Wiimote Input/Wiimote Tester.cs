using UnityEngine;
using WiimoteApi;

public class WiimoteTester : MonoBehaviour {

	[SerializeField]
	WiimoteInput input;

	//void Start() {
	//	input = new Wiimote_Input();
	//}

	void Update() {
		if (!input.IsActive) return;
		transform.localRotation = input.Rotation;

	}

}

using UnityEngine;
using WiimoteApi;

public class WiimoteTester : MonoBehaviour {

	[SerializeField]
	Wiimote_Input input;

	//void Start() {
	//	input = new Wiimote_Input();
	//}

	void Update() {
		if (!input.IsActive) return;
		transform.localRotation = input.Rotation;

	}

}

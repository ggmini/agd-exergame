using System.Collections;
using Unity.VectorGraphics;
using UnityEngine;

public class PattyStation : SandwichAssemblyStation {

	[SerializeField]
	float minimumAccel = 0.5f;
	[SerializeField]
	float impulseTarget = 1f;

	float required = 3;
	float completed = 0;

	bool scanInputs;
	int frame = 0;
	float curMax = 0;

	[SerializeField]
	GameObject pattyPrefab;
	[SerializeField]
	GameObject flipper;
	[SerializeField]
	Vector3 pattySpawnLocation;

	[SerializeField]
	float flipUpAnimDuration = 30f;
	[SerializeField]
	float flipDownAnimDuration = 60f;
	[SerializeField]
	float flipAnimHeight = 1f;
	[SerializeField]
	float flipRotateAmount = 15f;
	[SerializeField]
	float failUpAnimDuration = 30f;
	[SerializeField]
	float failAnimHeight = 0.5f;
	[SerializeField]
	float failRotateAmount = 10f;

	new void OnEnable() {
		base.OnEnable();
		SpawnPatty();
		scanInputs = true;
	}

	void FixedUpdate() {
		if (!scanInputs) 
			return;

		var accel = playerInput.GetAccelY();
		Debug.Log($"Accel: {accel}");
		if (accel > minimumAccel) {
			if (accel > curMax)
				curMax = accel;
		}
		if (frame >= 15) {
			if (curMax > minimumAccel && curMax < impulseTarget) // Not enough power
				StartCoroutine(FailFlip());
			else if (curMax >= impulseTarget) // Accel will be over the impulse target, so we can flip the patty
				StartCoroutine(FlipPatty());
			// Reset the frame counter and current max
			frame = 0;
			curMax = 0;
		} else frame++;
	}

	void MoveAndRotFlipper(Vector3 position, Quaternion rotation) {
		flipper.GetComponent<Rigidbody>().MovePosition(position);
		flipper.GetComponent<Rigidbody>().MoveRotation(rotation);
	}

	IEnumerator FlipPatty() {
		Debug.Log("Flipping patty");
		scanInputs = false;

		var originPos = flipper.transform.position;
		var targetPos = new Vector3(originPos.x, originPos.y + flipAnimHeight, originPos.z);
		var originRot = flipper.transform.rotation;
		var targetRot = Quaternion.Euler(0, 0, flipRotateAmount) * originRot;


		for (int frame = 0; frame < flipUpAnimDuration; frame++) {
			float t = (float) frame / flipUpAnimDuration;
			MoveAndRotFlipper(
				Vector3.Lerp(originPos, targetPos, t),
				Quaternion.Slerp(originRot, targetRot, t));
			yield return new WaitForFixedUpdate();
		}
		yield return new WaitForSeconds(0.5f);
		for (int frame = 0; frame < flipDownAnimDuration; frame++) {
			float t = (float)frame / flipDownAnimDuration;
			MoveAndRotFlipper(
				Vector3.Lerp(targetPos, originPos, t),
				Quaternion.Slerp(targetRot, originRot, t));
			yield return new WaitForFixedUpdate();
		}
		completed++;
		SpawnPatty();
		scanInputs = true;
	}

	IEnumerator FailFlip() {
		Debug.Log("Failed flip");
		scanInputs = false;

		var originPos = flipper.transform.position;
		var targetPos= new Vector3(originPos.x, originPos.y + failAnimHeight, originPos.z);
		var originRot = flipper.transform.rotation;
		var targetRot = Quaternion.Euler(0, 0, failRotateAmount) * originRot;

		for (int frame = 0; frame < failUpAnimDuration; frame++) {
			float t = (float)frame / failUpAnimDuration;
			MoveAndRotFlipper(
				Vector3.Lerp(originPos, targetPos, t),
				Quaternion.Slerp(originRot, targetRot, t));
			yield return new WaitForFixedUpdate();
		}
		yield return new WaitForSeconds(0.5f);
		for (int frame = 0; frame < flipDownAnimDuration; frame++) {
			float t = (float)frame / flipDownAnimDuration;
			MoveAndRotFlipper(
				Vector3.Lerp(targetPos, originPos, t),
				Quaternion.Slerp(targetRot, originRot, t));
			yield return new WaitForFixedUpdate();
		}
		scanInputs = true;


	}

	void SpawnPatty() {
		if (completed >= required) {
			StartCoroutine(cleanupItems());
			OnStationCleared?.Invoke(this);
			return;
		}

		Instantiate(pattyPrefab, transform.position + pattySpawnLocation, Quaternion.identity);
	}

}

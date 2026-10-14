using System.Collections;
using UnityEngine;

public class PattyStation : SandwichAssemblyStation {

	[SerializeField]
	float minimumAccel = 0.6f;
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

	Vector3 startPosition;
	Rigidbody flipperRB;

	[SerializeField]
	float flipUpAnimDuration;
	[SerializeField]
	float flipDownAnimDuration = 30f;
	[SerializeField]
	float flipAnimHeight = .2f;
	[SerializeField]
	float flipRotateAmount = 20f;
	[SerializeField]
	float failUpAnimDuration = 15f;
	[SerializeField]
	float failAnimHeight = 0.05f;
	[SerializeField]
	float failRotateAmount = 0f;

	new void OnEnable() {
		base.OnEnable();
		startPosition = flipper.transform.position;
		SpawnPatty();
		scanInputs = true;
		flipperRB = flipper.GetComponent<Rigidbody>();
	}

	void FixedUpdate() {
		if (!scanInputs) 
			return;

		var accel = playerInput.GetAccelY(deviceID);
		//Debug.Log($"Accel: {accel}");
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

		var targetPos = new Vector3(startPosition.x + 0.1f, startPosition.y + flipAnimHeight, startPosition.z);
		var originRot = flipper.transform.rotation;
		var targetRot = Quaternion.Euler(0, 0, -flipRotateAmount) * originRot;


		for (int frame = 0; frame < flipUpAnimDuration; frame++) {
			float t = (float) frame / flipUpAnimDuration;
			MoveAndRotFlipper(
				Vector3.Lerp(startPosition, targetPos, t),
				Quaternion.Lerp(originRot, targetRot, t));
			yield return new WaitForFixedUpdate();
		}
		yield return new WaitForSeconds(0.5f);
		for (int frame = 0; frame < flipDownAnimDuration; frame++) {
			float t = (float)frame / flipDownAnimDuration;
			MoveAndRotFlipper(
				Vector3.Lerp(targetPos, startPosition, t),
				Quaternion.Lerp(targetRot, originRot, t));
			yield return new WaitForFixedUpdate();
		}
		completed++;
		flipperRB.linearVelocity = Vector3.zero;
		flipperRB.angularVelocity = Vector3.zero;
		flipperRB.MovePosition(startPosition);
		flipperRB.MoveRotation(originRot);
		SpawnPatty();
		scanInputs = true;
	}

	IEnumerator FailFlip() {
		Debug.Log("Failed flip");
		scanInputs = false;

		var targetPos = new Vector3(startPosition.x, startPosition.y + failAnimHeight, startPosition.z);
		var originRot = flipper.transform.rotation;
		var targetRot = Quaternion.Euler(0, 0, -failRotateAmount) * originRot;

		for (int frame = 0; frame < failUpAnimDuration; frame++) {
			float t = (float)frame / failUpAnimDuration;
			MoveAndRotFlipper(
				Vector3.Lerp(startPosition, targetPos, t),
				Quaternion.Slerp(originRot, targetRot, t));
			yield return new WaitForFixedUpdate();
		}
		yield return new WaitForSeconds(0.5f);
		for (int frame = 0; frame < flipDownAnimDuration; frame++) {
			float t = (float)frame / flipDownAnimDuration;
			MoveAndRotFlipper(
				Vector3.Lerp(targetPos, startPosition, t),
				Quaternion.Slerp(targetRot, originRot, t));
			yield return new WaitForFixedUpdate();
		}
		scanInputs = true;
		flipperRB.linearVelocity = Vector3.zero;
		flipperRB.angularVelocity = Vector3.zero;
		flipperRB.MovePosition(startPosition);
		flipperRB.MoveRotation(originRot);
	}

	void SpawnPatty() {
		if (completed >= required) {
			Debug.Log("station should be cleared");
			StartCoroutine(cleanupItems());
			OnStationCleared?.Invoke(this);
			return;
		}

		Instantiate(pattyPrefab, transform.position + pattySpawnLocation, Quaternion.identity);
	}


	private void OnDrawGizmos()
	{
		Gizmos.color = Color.orangeRed;
		Gizmos.DrawCube(transform.position + pattySpawnLocation,Vector3.one/10);
	}




}

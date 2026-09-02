using UnityEngine;

public class RollStation : BaseStation
{

	float roll;
	float rollPrev;
	float rollProgress;

	[SerializeField]
	int requiredRolls = 3;

	void FixedUpdate() {
		rollPrev = roll;

		float rollQuat = playerInput.GetRoll();

		// Convert the roll quaternion to degrees
		if (useMouse) roll = rollQuat;
		else roll = rollQuat * Mathf.Rad2Deg;
		float rollDiff = Mathf.Abs(roll - rollPrev);

		float rollDiffNormalized = rollDiff / 360f;

		rollProgress += rollDiffNormalized;
		Mathf.Clamp(rollProgress, 0f, requiredRolls);

		AnimateRoll();

		if (rollProgress >= 1f) {
			OnStationCleared?.Invoke(this);
		}
	}

	void AnimateRoll() {
		transform.localRotation = Quaternion.Euler(0f, 0f, roll);
		// TODO: Animate the roll based on rollProgress
	}

}

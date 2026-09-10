using UnityEngine;
using System.Collections.Generic;

public class CuttingStation : SandwichAssemblyStation {
    public Rigidbody Knife;
    public float Speed = 10f;
    public float YOffsetCeiling = 2f;

    private bool IsKnifePrimed = true;

    [SerializeField]
    float accelerationModifier = 1f;

    [SerializeField]
	List<GameObject> decoys;
    [SerializeField]
    List<SliceableAsset> sliceableAssets;
    int activeAssetIndex = 0;

	void FixedUpdate() {
        Vector3 accelerationDir;
        if (useMouse) {
            Vector2 mouseDelta = playerInput.GetAccel();
            if (mouseDelta.magnitude == 0) return;
            accelerationDir = new(0, mouseDelta.y, 0);
        } else {
            var accel = playerInput.GetAccel();

            if (Mathf.Abs(accel.y) < 0.5f) return;
            accelerationDir = new(0, accel.y * accelerationModifier, 0);
        }
        float moveDistance = (accelerationDir * Speed * Time.fixedDeltaTime).magnitude;

        Vector3 targetPos = Knife.transform.position + accelerationDir.normalized * moveDistance;

        if (Physics.Raycast(Knife.transform.position, accelerationDir.normalized, out RaycastHit hit, moveDistance)) {
            float safeDistance = Mathf.Clamp(hit.distance - 0.05f, 0, moveDistance);
            float targetY = Knife.transform.position.y + safeDistance;
            targetPos.y = targetY;
        }

        if (accelerationDir.y > 0) {
            targetPos.y = Mathf.Min(targetPos.y, transform.position.y + YOffsetCeiling);
        }

        Knife.MovePosition(targetPos);
    }


    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.GetComponent<Rigidbody>() != Knife || !IsKnifePrimed) return;

        NextTomato();
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.GetComponent<Rigidbody>() == Knife) {
            IsKnifePrimed = true;
        }
    }

    private void NextTomato() {
        if (sliceableAssets[activeAssetIndex].NextStep()) {
            if (activeAssetIndex == sliceableAssets.Count - 1) {
                //StartCoroutine(cleanupItems());
                OnStationCleared?.Invoke(this);
                return;
			} else {
                sliceableAssets[activeAssetIndex].gameObject.SetActive(false);
                sliceableAssets[++activeAssetIndex].gameObject.SetActive(true);
                decoys[activeAssetIndex - 1].SetActive(false);
			}
		}

        IsKnifePrimed = false;
    }
}

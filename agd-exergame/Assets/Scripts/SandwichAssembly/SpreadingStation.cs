using UnityEngine;
using UnityEngine.InputSystem;

public class SpreadingStation : SandwichAssemblyStation {
    public Rigidbody ButterKnife;
    public AnimationCurve pathCurve;
    public GameObject[] ButterLayers;
    public float Speed = 5f;

    private Vector3 startPos;

    float MaxWidth = 1.0f;
    float MaxDepth = 0.3f;
    float curveX = 0f;
    private int Layers = 0;

    private bool BoxEntered = false;

    private float lastSavedX;

    protected override void Start() {
        base.Start();
        startPos = ButterKnife.position;
    }

    void FixedUpdate() {
        curveX += playerInput.GetAccelZ() * Speed * Time.fixedDeltaTime;
        curveX = Mathf.Clamp(curveX, 0f, 1f);

        float z = pathCurve.Evaluate(curveX);
        Vector3 targetPos = startPos + new Vector3(curveX * MaxWidth, 0f, z * MaxDepth);

        ButterKnife.MovePosition(targetPos);
    }


    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.GetComponent<Rigidbody>() != ButterKnife) return;

        BoxEntered = true;
        lastSavedX = ButterKnife.position.x;
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.GetComponent<Rigidbody>() != ButterKnife) return;
        if (BoxEntered && Mathf.Abs(ButterKnife.position.x - lastSavedX) > 0.2f) {
            NextLayer();
            BoxEntered = false;
        }
    }

    private void NextLayer() {
        if (Layers >= 5) {
            OnStationCleared?.Invoke(this);
            return;
        }

        ButterLayers[Layers].SetActive(true);
        Layers++;
        BoxEntered = false;
    }

}

using UnityEngine;
using UnityEngine.InputSystem;

public class SpreadingStation : SandwichAssemblyStation
{
    public Rigidbody ButterKnife;
    public AnimationCurve pathCurve;
    public GameObject[] ButterLayers;
    public float Speed = 5f;

    private Vector3 startPos;

    float MaxWidth = 1.0f;
    float MaxDepth = 0.3f;
    float curveX = 0f;
    private int Layers = 0;
    
    private Vector2 accumDelta;
    private bool BoxEntered = false;



    protected override void Start() {
        base.Start();
        startPos = ButterKnife.position;
    }

    private void Update()
    {
        accumDelta += Mouse.current.delta.ReadValue();
    }


    void FixedUpdate()
    {
        //Vector3 acceleration = Vector3.zero; // replace with WiiMote Input
        Vector2 mouseDelta = accumDelta;
        accumDelta = Vector2.zero;
        curveX += mouseDelta.x * Speed * Time.fixedDeltaTime;
        curveX = Mathf.Clamp(curveX, 0f, 1f);

        float z = pathCurve.Evaluate(curveX);
        Vector3 targetPos = startPos + new Vector3(curveX * MaxWidth, 0f, z * MaxDepth);

        ButterKnife.MovePosition(targetPos);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Rigidbody>() != ButterKnife) return;

        BoxEntered = true;

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<Rigidbody>() != ButterKnife) return;
        if (BoxEntered)
        {
            NextLayer();
            BoxEntered = false;
        }
    }

    private void NextLayer()
    {
        if (Layers >= 3)
        {
            OnStationCleared?.Invoke(this);
            return;
        }

        ButterLayers[Layers].SetActive(true);
        Layers++;
        BoxEntered = false;
    }

}

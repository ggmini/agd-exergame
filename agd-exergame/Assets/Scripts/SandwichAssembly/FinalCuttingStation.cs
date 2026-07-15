using UnityEngine;
using UnityEngine.InputSystem;

public class FinalCuttingStation : SandwichAssemblyStation
{
    public Rigidbody Knife;
    public float Speed = 10f;

    private Vector2 accumDelta;
    private Vector3 bottomRight;
    private Vector3 topLeft;
    private Vector3 diagonal;

    private float t = 0;
    private float t2 = 0;
    private float runningCounter = 0;
    private int CurrentExtreme = 0;
    private int DepthCounter = 0;

    protected override void Start()
    {
        base.Start();
        topLeft = Knife.transform.position + new Vector3(-0.2f, 0, 0.2f);
        bottomRight = Knife.transform.position + new Vector3(0.2f, 0, -0.2f);
        diagonal = topLeft - bottomRight;
    }


    private void Update()
    {
        accumDelta += Mouse.current.delta.ReadValue();
    }

    void FixedUpdate() {
        Vector2 mouseDelta = accumDelta;
        accumDelta = Vector2.zero;
        t += mouseDelta.x * Speed * Time.fixedDeltaTime;
        t = Mathf.Clamp(t, 0, 1);
        runningCounter += (Mathf.Abs(t - t2));
        t2 = t;
        if ((t == 0 || t == 1) && (int)t != CurrentExtreme)
        {
            CurrentExtreme = (int)t;
            DepthCounter++;
            if (DepthCounter >= 7)
            {
                SplitSandwich();
                return;
            }
            else if (t == 0)
            {
                SeparateLayer(0);
            }
        }

        Vector3 targetPos = GetNextPoint(t);

        Knife.MovePosition(targetPos);
    }

    Vector3 GetNextPoint(float t)
    {
        Debug.Log(DepthCounter + t);
        return bottomRight + diagonal * t + new Vector3(0, -0.03f, 0) * runningCounter;
    }


    private void SeparateLayer(int Layer)
    {

    }

    private void SplitSandwich()
    {


        OnStationCleared?.Invoke(this);
    }


}

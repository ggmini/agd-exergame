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

    protected override void Start()
    {
        base.Start();
        topLeft = Table.transform.position + new Vector3(-0.5f, 0.3f, 0.5f);
        bottomRight = Table.transform.position + new Vector3(0.5f, 0.3f, -0.5f);
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

        Vector3 targetPos = GetNextPoint(t);

        Knife.MovePosition(targetPos);
    }

    Vector3 GetNextPoint(float t)
    {
        return bottomRight + diagonal * t;
    }

}

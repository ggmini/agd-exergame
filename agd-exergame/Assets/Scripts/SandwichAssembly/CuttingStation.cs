using UnityEngine;
using UnityEngine.InputSystem;

public class CuttingStation : SandwichAssemblyStation
{
    public Rigidbody Knife;
    public float Speed = 10f;
    public float YOffsetCeiling = 2f;

    private Vector2 accumDelta;

    private void Update()
    {
        accumDelta += Mouse.current.delta.ReadValue();
    }

    void FixedUpdate() {
        //Vector3 acceleration = Vector3.zero; // replace with WiiMote Input
        Vector2 mouseDelta = accumDelta;
        accumDelta = Vector2.zero;
        if (mouseDelta.magnitude == 0) return;
        
        Vector3 accelerationDir = new Vector3(0, mouseDelta.y, 0);
        float moveDistance = (accelerationDir * Speed * Time.fixedDeltaTime).magnitude;
        
        Vector3 targetPos = Knife.transform.position + accelerationDir.normalized * moveDistance;

        if (Physics.Raycast(Knife.transform.position, accelerationDir.normalized, out RaycastHit hit, moveDistance))
        {
            float safeDistance = Mathf.Clamp(hit.distance - 0.05f, 0, moveDistance);
            float targetY = Knife.transform.position.y + safeDistance;
            targetPos.y = targetY;
        }

        if (accelerationDir.y > 0)
        {
            targetPos.y = Mathf.Min(targetPos.y, transform.position.y + YOffsetCeiling);
        }


        Knife.MovePosition(targetPos);
    }
}

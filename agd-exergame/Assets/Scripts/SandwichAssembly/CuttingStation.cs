using UnityEngine;
using UnityEngine.InputSystem;

public class CuttingStation : SandwichAssemblyStation
{
    public Rigidbody Knife;
    public float Speed = 10f;
    public float YOffsetCeiling = 2f;

    void FixedUpdate() {
        //Vector3 acceleration = Vector3.zero; // replace with WiiMote Input
        Vector2 mouseDelta = Mouse.current.delta.ReadValue().normalized;
        Vector3 accelerationDir = new Vector3(0, mouseDelta.y, 0);
        float moveDistance = (accelerationDir * Speed * Time.fixedDeltaTime).magnitude;
        Vector3 targetPos = Knife.transform.position + accelerationDir * moveDistance;

        if (Physics.Raycast(Knife.transform.position, accelerationDir, out RaycastHit hit, moveDistance)) {
            float safeDistance = Mathf.Clamp(hit.distance - 0.05f, 0, moveDistance);
            float targetY = Knife.transform.position.y + safeDistance;
            targetPos.y = targetY;
        }

        targetPos.y = Mathf.Min(targetPos.y, 0.1f);

        Knife.MovePosition(targetPos);
    }
}

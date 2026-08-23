using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CuttingStation : SandwichAssemblyStation
{
    public Rigidbody Knife;
    public float Speed = 10f;
    public float YOffsetCeiling = 2f;
    public GameObject[] tomatoSlices;
    public GameObject Tomato;

    private Vector2 accumDelta;
    private int TomatoCounter = 0;
    private bool IsKnifePrimed = true;

    [SerializeField]
    bool useKM;

    private void Update()
    {
        accumDelta += Mouse.current.delta.ReadValue();
    }

    void FixedUpdate() {
        Vector3 accelerationDir;
        if (useKM) {
            Vector2 mouseDelta = accumDelta;
            accumDelta = Vector2.zero;
            if (mouseDelta.magnitude == 0) return;
            accelerationDir = new Vector3(0, mouseDelta.y, 0);
        } else {

            if (WebSocketManager.Instance.Msg == null) return;

            Vector3 accel = new Vector3(
                WebSocketManager.Instance.Msg.accel_x,
                WebSocketManager.Instance.Msg.accel_y + 9.81f,
                WebSocketManager.Instance.Msg.accel_z);

            if (Mathf.Abs(accel.y) < 0.5f) return;
            accelerationDir = new Vector3(0, accel.y, 0);
        }
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


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Rigidbody>() != Knife || !IsKnifePrimed) return;

        NextTomato();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<Rigidbody>() == Knife)
        {
            IsKnifePrimed = true;
        }
    }

    private void NextTomato()
    {
        if (TomatoCounter >= tomatoSlices.Length)
        {
            OnStationCleared?.Invoke(this);
            return;
        }

        tomatoSlices[TomatoCounter].SetActive(true);
        TomatoCounter++;
        IsKnifePrimed = false;
        
        Renderer rend  = Tomato.GetComponent<Renderer>();

        float newRatio = rend.material.GetFloat("_CutRatio") -0.1f;
        rend.material.SetFloat("_CutRatio", newRatio);   
    }
}

using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class AssemblyStation : SandwichAssemblyStation
{
    public AssemblyTray[] Trays;
    public Rigidbody Pointer;
    public float Speed = 10f;

    [SerializeField] private AssemblyTray CurrentHoveredTray;
    [SerializeField] private GameObject HeldItem;
    private Vector3 HeldItemStartingPos;

    private int NextLayer = 0;
    private float LayerHeight = 0.05f;

    private Vector2 accumDelta;
    private float t;

    void OnEnable()
    {
        foreach (var tray in Trays)
        {
            tray.OnZoneTriggered += HandleZoneTriggered;
            tray.OnZoneExited += HandleZoneExited;
        }
    }

    void OnDisable()
    {
        foreach (var tray in Trays)
        {
            tray.OnZoneTriggered -= HandleZoneTriggered;
            tray.OnZoneExited += HandleZoneExited;
        }
    }

    private void Update()
    {
        accumDelta += Mouse.current.delta.ReadValue();

        if (CurrentHoveredTray == null || HeldItem != null) return;

        //if (Keyboard.current.altKey.wasPressedThisFrame)
        if (WebSocketManager.Instance.Msg == null) return;
        
        if (WebSocketManager.Instance.Msg.button_pressed)
        {
            HeldItem = CurrentHoveredTray.GetAssemblyItem();
            //HeldItem.transform.position = CurrentHoveredTray.;
            HeldItem.transform.position = CurrentHoveredTray.transform.position + new Vector3(0, 0.1f, 0);
            HeldItemStartingPos = HeldItem.transform.position;
            Pointer.GetComponent<MeshRenderer>().enabled = false;
            t = 0f;
        }

    }

    void FixedUpdate()
    {
        if (WebSocketManager.Instance.Msg == null) return;

        Vector3 accel = new Vector3(
            WebSocketManager.Instance.Msg.accel_x,
            WebSocketManager.Instance.Msg.accel_y + 9.81f,
            WebSocketManager.Instance.Msg.accel_z);

        if (HeldItem == null)
        {
            //Vector2 mouseDelta = accumDelta;
            //accumDelta = Vector2.zero;

            //Vector3 accelerationDir = new Vector3(mouseDelta.x, 0, 0);
            Vector3 accelerationDir = new Vector3(accel.x, 0, 0);
            Vector3 targetPos = Pointer.position + transform.TransformDirection(accelerationDir * Speed * Time.fixedDeltaTime);

            targetPos.x = Mathf.Clamp(targetPos.x, transform.position.x - 1, transform.position.x + 1);

            Pointer.MovePosition(targetPos);
        }
        else
        {
            //Vector2 mouseDelta = accumDelta;
            //accumDelta = Vector2.zero;
            //t += mouseDelta.y * Speed * Time.fixedDeltaTime;
            t += accel.y * Speed * Time.fixedDeltaTime;
            t = Mathf.Clamp(t, 0, 1);

            Vector3 targetPos = GetNextTargetPosition();
            Vector3 middlePos = HeldItemStartingPos + (targetPos - HeldItemStartingPos) / 2.0f + new Vector3(0, 1f, 0);

            Vector3 nextPos = QuadraticBezier(HeldItemStartingPos, middlePos, targetPos, t);
            HeldItem.transform.position = nextPos;

            if (t >= 1) LayerGoalReached();
        }
    }

    private void LayerGoalReached()
    {
        NextLayer++;
        HeldItem = null;
        HeldItemStartingPos = Vector3.zero;
        Pointer.GetComponent<MeshRenderer>().enabled = true;

        if (NextLayer >= 5)
        {
            OnStationCleared?.Invoke(this);
        }
    }

    Vector3 GetNextTargetPosition()
    {
        Bounds b = Table.GetComponent<Renderer>().bounds;
        Vector3 target = new(b.center.x, b.max.y, b.center.z);
        target.y += (0.01f + NextLayer * LayerHeight);

        return target;
    }


    Vector3 QuadraticBezier(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        float u = 1f - t;
        return u * u * p0 + 2f * u * t * p1 + t * t * p2;
    }



    private void HandleZoneTriggered(AssemblyTray tray)
    {
        CurrentHoveredTray = tray;
    }

    private void HandleZoneExited(AssemblyTray tray)
    {
        if (tray == CurrentHoveredTray)
        {
            CurrentHoveredTray = null;
        }
    }

}

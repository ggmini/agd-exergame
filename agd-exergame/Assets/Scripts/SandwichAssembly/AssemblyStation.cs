using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class AssemblyStation : SandwichAssemblyStation {
    public AssemblyTray[] Trays;
    public Rigidbody Pointer;
    public float Speed = 10f;

    [SerializeField] private AssemblyTray CurrentHoveredTray;
    [SerializeField] private GameObject HeldItem;
    private Vector3 HeldItemStartingPos;

    GameObject[]
        items = new GameObject[5]; //Store for cleanup (possibly a better solution, link to a parent and destroy it instead)

    private int NextLayer = 0;
    private float LayerHeight = 0.05f;
    System.Random rnd = new System.Random();
    
    private float t;

    [SerializeField] bool useKM;

    IPlayerInput playerInput;

    void OnEnable() {
        playerInput = gameObject.AddComponent<WebSocketInput>(); //TODO: globalish type thing?
        foreach (var tray in Trays) {
            tray.OnZoneTriggered += HandleZoneTriggered;
            tray.OnZoneExited += HandleZoneExited;
        }
        SelectNextItem();
    }

    void OnDisable() {
        StartCoroutine(CleanUpOffScreen());
        foreach (var tray in Trays) {
            tray.OnZoneTriggered -= HandleZoneTriggered;
            tray.OnZoneExited -= HandleZoneExited;
        }
    }

    private void Update() {
        if (CurrentHoveredTray == null || HeldItem != null) return;

        if (playerInput.GetButtonPressed())
            PickUpItem();
    }

    void FixedUpdate() {
        if (HeldItem == null) return;
        
        t += playerInput.GetAccelY() * Speed * Time.fixedDeltaTime;
        t = Mathf.Clamp(t, 0, 1);

        var targetPos = GetNextTargetPosition();
        var middlePos = HeldItemStartingPos + (targetPos - HeldItemStartingPos) / 2.0f + new Vector3(0, 1f, 0);

        var nextPos = QuadraticBezier(HeldItemStartingPos, middlePos, targetPos, t);
        HeldItem.transform.position = nextPos;

        if (t >= 1) LayerGoalReached();
    }

    void PickUpItem() {
        HeldItem = CurrentHoveredTray.GetAssemblyItem();
        items[NextLayer] = HeldItem;
        //HeldItem.transform.position = CurrentHoveredTray.;
        HeldItem.transform.position = CurrentHoveredTray.transform.position + new Vector3(0, 0.1f, 0);
        HeldItemStartingPos = HeldItem.transform.position;
        Pointer.GetComponent<MeshRenderer>().enabled = false;
        t = 0f;
    }

    void SelectNextItem() {
        if (NextLayer == 0 // First layer is always bread
            || NextLayer == 5 && rnd.NextDouble() < 0.4) {
            // High chance to cover sandwich with bread
            CurrentHoveredTray = Trays[0];
            return;
        }
        // Default: Select a random item
        int trayIndex = rnd.Next(1, Trays.Length);
        CurrentHoveredTray = Trays[trayIndex];
    }

    IEnumerator CleanUpOffScreen() {
        yield return new WaitForSeconds(3f);
        foreach (var item in items) {
            if (item != null) {
                Destroy(item);
            }
        }
    }

    private void LayerGoalReached() {
        NextLayer++;
        HeldItem = null;
        HeldItemStartingPos = Vector3.zero;
        SelectNextItem();
        Pointer.GetComponent<MeshRenderer>().enabled = true;
        Pointer.position = CurrentHoveredTray.transform.position + new Vector3(0, 0.1f, 0);

        if (NextLayer >= 5) {
            OnStationCleared?.Invoke(this);
        }
    }

    Vector3 GetNextTargetPosition() {
        Bounds b = Table.GetComponent<Renderer>().bounds;
        Vector3 target = new(b.center.x, b.max.y, b.center.z);
        target.y += (0.01f + NextLayer * LayerHeight);

        return target;
    }


    Vector3 QuadraticBezier(Vector3 p0, Vector3 p1, Vector3 p2, float t) {
        float u = 1f - t;
        return u * u * p0 + 2f * u * t * p1 + t * t * p2;
    }



    private void HandleZoneTriggered(AssemblyTray tray) {
        CurrentHoveredTray = tray;
    }

    private void HandleZoneExited(AssemblyTray tray) {
        if (tray == CurrentHoveredTray) {
            CurrentHoveredTray = null;
        }
    }

}

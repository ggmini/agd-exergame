using UnityEngine;

public class AssemblyStation : SandwichAssemblyStation {
    public AssemblyTray[] Trays;
    public Rigidbody Pointer;
    public float Speed = 10f;

    [SerializeField] private AssemblyTray CurrentHoveredTray;
    [SerializeField] private GameObject HeldItem;
    Vector3 HeldItemStartingPos;

    private int NextLayer = 0;
    private float LayerHeight = 0.05f;
    System.Random rnd = new System.Random();

    private float t = 0.5f;

    new void OnEnable() {
        base.OnEnable();
        foreach (var tray in Trays) {
            tray.OnZoneTriggered += HandleZoneTriggered;
            tray.OnZoneExited += HandleZoneExited;
        }
        HeldItemStartingPos = Trays[0].transform.position;
        SelectNextItem();
    }

    new void OnDisable() {
        base.OnDisable();
        // StartCoroutine(cleanupItems());
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
        // Movement
        if (useMouse) {
            Vector2 mouseDelta = playerInput.GetAccel();
            t += mouseDelta.y * Speed * Time.fixedDeltaTime;
        }
        else
            t -= playerInput.GetAccelZ() * Speed * Time.fixedDeltaTime;

        t = Mathf.Clamp(t, 0, 1);

        var targetPos = GetNextTargetPosition();
        var middlePos = HeldItemStartingPos + (targetPos - HeldItemStartingPos) / 2.0f + new Vector3(0, 1f, 0);

        var nextPos = QuadraticBezier(HeldItemStartingPos, middlePos, targetPos, t);
        if (HeldItem != null)
            HeldItem.transform.position = nextPos;
        else Pointer.transform.position = nextPos;
        
        // Grab/Dop
        if (t >= 1 && HeldItem != null)
            LayerGoalReached();
        else if (t == 0f && HeldItem == null && CurrentHoveredTray != null)
            PickUpItem();            
    }

    void PickUpItem() {
        HeldItem = CurrentHoveredTray.GetAssemblyItem();
        //items.Add(HeldItem); adding this to the var in base script means its not instantiated as a child even if i enforce it
        
        HeldItem.transform.position = CurrentHoveredTray.transform.position + new Vector3(0, 0.1f, 0);
        HeldItemStartingPos = HeldItem.transform.position;
        Pointer.GetComponent<MeshRenderer>().enabled = false;
    }

    void SelectNextItem() {
        if (NextLayer == 0 // First layer is always bread
            || NextLayer == 5 && rnd.NextDouble() < 0.4) { // High chance to cover sandwich with bread
            CurrentHoveredTray = Trays[0];
            return;
        } // Default: Select a random item
        var trayIndex = rnd.Next(1, Trays.Length);
        CurrentHoveredTray = Trays[trayIndex];
        HeldItemStartingPos = Trays[trayIndex].transform.position;
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

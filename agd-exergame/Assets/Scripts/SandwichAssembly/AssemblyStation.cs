using UnityEngine;
using UnityEngine.InputSystem;

public class AssemblyStation : SandwichAssemblyStation
{
    public AssemblyTray[] Trays;

    void OnEnable()
    {
        foreach (var tray in Trays)
            tray.OnZoneTriggered += HandleZoneTriggered;
    }

    void OnDisable()
    {
        foreach (var tray in Trays)
            tray.OnZoneTriggered -= HandleZoneTriggered;
    }


    void FixedUpdate() {

    }


    private void HandleZoneTriggered(Collider other)
    {

    }

}

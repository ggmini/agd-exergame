using System;
using UnityEngine;

public class SandwichAssemblyStation : MonoBehaviour
{
    public static Action<SandwichAssemblyStation> OnStationCleared;

    public GameObject Table;
    public bool isActiveStation = false;


    protected virtual void Start()
    {
        enabled = isActiveStation;
    }

    void Update()
    {
        
    }

    public void toggleIsActiveStation(){
        isActiveStation = !isActiveStation;
        enabled = isActiveStation;
    }
}

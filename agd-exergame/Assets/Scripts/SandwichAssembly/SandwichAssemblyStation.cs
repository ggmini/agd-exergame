using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandwichAssemblyStation : BaseStation {

    public GameObject Table;

    protected List<GameObject> items;

    protected virtual void Start() {
        enabled = isActiveStation;
    }
}

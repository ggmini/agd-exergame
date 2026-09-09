using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandwichAssemblyStation : BaseStation {

    protected List<GameObject> items;

    protected virtual void Start() {
        enabled = isActiveStation;
    }
}

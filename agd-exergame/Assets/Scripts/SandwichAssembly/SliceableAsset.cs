using System.Collections.Generic;
using UnityEngine;

public class SliceableAsset : MonoBehaviour {
    
    Material material;

    [SerializeField]
	List<GameObject> slices = new List<GameObject>();
    int step = 0;

    [SerializeField]
	float maxCutRatio;
    [SerializeField]
    float minCutRatio;

    float minMaxCutRatioDiff => maxCutRatio - minCutRatio;

	void Start() {
        material = GetComponent<Renderer>().material;
	}

	public bool NextStep() {
        slices[step].SetActive(true);
        step++;
        material.SetFloat("_CutRatio",  minCutRatio + minMaxCutRatioDiff * ((float)step / slices.Count));
		if (step >= slices.Count)
			return true;
        return false;
    }

}

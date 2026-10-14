using UnityEngine;

public class Goal : MonoBehaviour {
    BallRollGameManager gm;

    void Start() {
        gm = FindAnyObjectByType<BallRollGameManager>();
    }

    //TODO
    /*void OnTriggerEnter(Collider other) {
        if (other.gameObject == null)
            return;
        if (other.gameObject.tag == "ball")
            gm.BallHitTarget(other.gameObject);
    }*/

}

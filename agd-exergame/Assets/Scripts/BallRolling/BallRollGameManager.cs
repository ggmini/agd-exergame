using UnityEngine;

public class BallRollGameManager : MonoBehaviour {

    int points;

    public void BallHitTarget(GameObject ball) {
        points++;
        Destroy(ball);
        //TODO: New shape new ball?
        SpawnNewBall();
    }

    public void SpawnNewBall() {

    }

}

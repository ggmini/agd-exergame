using UnityEngine;

public class BallRollGameManager : MonoBehaviour {

	int points;
	int highscore;

	float timer;

	[SerializeField]
	GameObject ballPrefab;

	GameObject curBall;

	void Start() {
		curBall = Instantiate(ballPrefab, new Vector3(0, 10, 0), Quaternion.identity);
	}

	void FixedUpdate() {
		if (curBall == null)
			ResetGame();
		timer -= Time.fixedDeltaTime;
		if (timer < 0)
		{
			timer = 1;
			points++;
		}
	}

	public void ResetGame() {
		CheckHighScore();
		new WaitForSeconds(3f);
		points = 0;
		curBall = Instantiate(ballPrefab, new Vector3(0, 10, 0), Quaternion.identity);
	}

	void CheckHighScore() {
		if (points > highscore)
			highscore = points;
	}

	void OnGUI() {
		GUI.Label(new Rect(10, 10, 300, 20), "Points: " + points.ToString());
		GUI.Label(new Rect(10, 30, 300, 20), "Highscore: " + highscore.ToString());
	}

}

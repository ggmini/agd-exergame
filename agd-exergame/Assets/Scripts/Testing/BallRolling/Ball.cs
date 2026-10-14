using UnityEngine;

public class Ball : MonoBehaviour{

	void FixedUpdate() {
		if (transform.position.y < -10) 
			Destroy();
	}

	public void Destroy(){
		Destroy(gameObject);
	}

}
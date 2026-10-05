using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
	[SerializeField]
	MainScene main; // this is an ugly circular reference

	public void Reset()
	{
		main.Reset();
	}

	public void Resume()
	{
		main.TogglePause();
	}

	public void ReturnToMenu()
	{
		SceneManager.LoadScene(0);
	}

}

using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class PostGameMenu : MonoBehaviour {

	[SerializeField] MainScene main; // this is an ugly circular reference
	public TextMeshProUGUI winningText;

	public void RestartGame()
	{
		main.Reset();
	}

	public void ExitToMainMenu()
	{
		SceneManager.LoadScene(0);
	}
}

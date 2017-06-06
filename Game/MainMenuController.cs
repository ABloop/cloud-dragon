using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Controls the MainMenu scene
/// </summary>
public class MainMenuController : MonoBehaviour
{
	// UI
	public GameObject mmCanvas;
	public GameObject opCanvas;

	public void PlayGame ()
	{
		SceneManager.LoadScene ("PilotSelect", LoadSceneMode.Single);
	}

	public void GameOptions ()
	{
		mmCanvas.SetActive (false);
		opCanvas.SetActive (true);
	}

	public void QuitGame ()
	{
		Application.Quit ();
	}
}
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Controls the LevelComplete Scene
/// </summary>
public class LevelCompleteController : MonoBehaviour
{
	// PLAYER MANAGER
	public GameObject pm;
	public PlayerManager playerManager;

	// UI
	public Sprite[] pilotPortraits;
	public Image pilotPortrait;
	public Text pilotText;
	public Text scoreText;

	void Start ()
	{
		pm = GameObject.Find ("PlayerManager");
		playerManager = pm.GetComponent<PlayerManager> ();

		if (playerManager.pilot == "Icarai") {
			pilotPortrait.sprite = pilotPortraits [0];
		} else if (playerManager.pilot == "Chorg") {
			pilotPortrait.sprite = pilotPortraits [1];
		}

		pilotText.text = (playerManager.pilot);
		scoreText.text = ("Score: " + playerManager.score);
	}

	public void MainMenu ()
	{
		SceneManager.LoadScene ("MainMenu", LoadSceneMode.Single);
	}
}
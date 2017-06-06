/*
Background Fade Out adapted from Griffo (2012).
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Manages the game while a level is playing, procedurally generating enemies, hazards or power ups
/// </summary>
public class GameManager : MonoBehaviour
{
	// GAME MANAGER
	public static GameManager instance = null;

	// GAME
	[HideInInspector] public bool gamePlaying = true;
	[HideInInspector] public bool gamePaused = false;

	private string nextWaveType;
	private string currentWaveType;
	private float waveTimer = 0.0f;
	private int waveCount = 0;

	// LEVEL START AND FINISH
	public bool levelStarting = true;
	public bool levelFinished = false;
	public bool levelEnding = false;
	private float waitTime = 1.0f;
	public Image fadeOut;
	private float fader = 0.0f;

	// UI
	public GameObject restartButton;
	public GameObject quitButton;

	// ENEMIES
	[HideInInspector] public WaveManager waveManager;
	[HideInInspector] public ArrayList waveEnemies;
	[HideInInspector] public int enemiesInPlay = 0;
	[HideInInspector] public int enemiesInTotal = 0;
	[HideInInspector] public int difficulty = 1;

	// HAZARDS
	private HazardManager hazardManager;
	[HideInInspector] public ArrayList hazards;
	[HideInInspector] public int hazardsInPlay = 0;
	[HideInInspector] public int hazardsInTotal = 0;
	private int wavesSinceHazards;

	// POWER UPS
	[HideInInspector] public PowerUpManager powerUpManager;
	[HideInInspector] public ArrayList powerUps;
	[HideInInspector] public int powerUpsInPlay = 0;
	[HideInInspector] public int powerUpsInTotal = 0;
	private int wavesSincePowerUps;

	// BOSS
	[HideInInspector] public bool bossInPlay = false;
	[HideInInspector] public bool bossDefeated = false;

	void Awake ()
	{
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy (gameObject);
	
		waveManager = GetComponent <WaveManager> ();
		hazardManager = GetComponent <HazardManager> ();
		powerUpManager = GetComponent <PowerUpManager> ();

		waveEnemies = new ArrayList ();
		hazards = new ArrayList ();
		powerUps = new ArrayList ();
	}

	void Update ()
	{
		if (levelStarting) {
			waitTime -= Time.deltaTime;
		} else {

			if (gamePlaying && !bossInPlay) {
				
				if (waveTimer <= 0.0f) {
					ChooseNextWaveType ();
					waveTimer = 10.0f;
				} else {

					if (currentWaveType == "Enemies" || currentWaveType == "Enemies and Hazards") {
						
						if (enemiesInPlay == 0) {
							waveEnemies.Clear ();
							ChooseNextWaveType ();
							waveTimer = 10.0f;
						}
					} else if (currentWaveType == "Hazards and Power Ups") {

						if (powerUpsInPlay == 0) {
							powerUps.Clear ();
							ChooseNextWaveType ();
							waveTimer = 10.0f;
						}
					}
				}

				if (!PlayerController.instance.ickySpecialActive) {
					waveTimer -= Time.deltaTime;
				}

			} else if (gamePlaying && bossInPlay) {
				
				if (bossDefeated) {
					levelFinished = true;
					PlayerController.instance.LevelFinished ();
				}
			} else {
				restartButton.SetActive (true);
				quitButton.SetActive (true);
			}
		}

		if (waitTime <= 0.0f && levelStarting) {
			levelStarting = false;
			PlayerController.instance.LevelStarted ();
		}

		if (levelEnding == true) {
			fadeOut.gameObject.SetActive (true);

			if (fadeOut.color.a >= 1.0f) {
				SceneManager.LoadScene ("LevelFinished", LoadSceneMode.Single);
			} else {
				fader += Time.fixedDeltaTime;
				fadeOut.color = new Color (1.0f, 1.0f, 1.0f, fader);
			}
		}
	}

	void CreateWave ()
	{
		waveManager.WaveSetup (waveEnemies);
		enemiesInPlay = enemiesInPlay + waveManager.enemiesInPlay;
		enemiesInTotal = enemiesInPlay;

		wavesSinceHazards--;
		wavesSincePowerUps--;
	}

	void CreateHazards ()
	{
		hazardManager.HazardsSetup (hazards);
		hazardsInPlay = hazardsInPlay + hazardManager.hazardsInPlay;
		hazardsInTotal = hazardsInPlay;

		SetHazardWait ();
	}

	void CreatePowerUps ()
	{
		powerUpManager.PowerUpsSetup (powerUps);
		powerUpsInPlay = powerUpsInPlay + powerUpManager.powerUpsInPlay;
		powerUpsInTotal = powerUpsInPlay;

		SetPowerUpsWait ();
	}

	void ChooseNextWaveType ()
	{
		int nextWave = 0;

		if (wavesSinceHazards <= 0) {
			nextWave = Random.Range (1, 3);
		} else if (wavesSincePowerUps <= 0) {
			nextWave = Random.Range (1, 4);
		} else {
			nextWave = 1;
		}

		if (difficulty == 6) {
			nextWave = 1;
		}

		switch (nextWave) {
		case 1:
			currentWaveType = "Enemies";
			CreateWave ();
			break;
		case 2:
			currentWaveType = "Enemies and Hazards";
			CreateWave ();
			CreateHazards ();
			break;
		case 3:
			currentWaveType = "Hazards and Power Ups";
			CreateHazards ();
			CreatePowerUps ();
			break;
		}

		// GET TO BOSS FASTER
		//waveCount = waveCount + 20;

		waveCount++;
		UpdateDifficulty ();
	}

	void UpdateDifficulty ()
	{
		if (waveCount > 4 && waveCount < 8) {
			difficulty = 2;
		} else if (waveCount > 8 && waveCount < 12) {
			difficulty = 3;
		} else if (waveCount > 12 && waveCount < 16) {
			difficulty = 4;
		} else if (waveCount > 16 && waveCount < 20) {
			difficulty = 5;
		} else if (waveCount > 20) {
			difficulty = 6;
		}
	}

	void SetHazardWait ()
	{
		wavesSinceHazards = Random.Range (1, 5);
	}

	void SetPowerUpsWait ()
	{
		wavesSincePowerUps = Random.Range (3, 6);
	}

	public void PauseGame ()
	{
		if (gamePaused) {
			Time.timeScale = 1.0f;
			gamePaused = false;
			PlayerController.instance.GamePaused ();

			restartButton.SetActive (false);
			quitButton.SetActive (false);
		} else {
			Time.timeScale = 0.0f;
			gamePaused = true;
			PlayerController.instance.GamePaused ();

			restartButton.SetActive (true);
			quitButton.SetActive (true);
		}
	}

	public void RestartGame ()
	{
		if (gamePaused) {
			Time.timeScale = 1.0f;
			gamePaused = false;
		}

		var pm = GameObject.Find ("PlayerManager");
		PlayerManager playerManager = pm.GetComponent <PlayerManager> ();

		if (playerManager.pilot == "Icarai") {
			SceneManager.LoadScene ("Icarai", LoadSceneMode.Single);
		} else if (playerManager.pilot == "Chorg") {
			SceneManager.LoadScene ("Chorg", LoadSceneMode.Single);
		}
	}

	public void QuitGame ()
	{
		if (gamePaused) {
			Time.timeScale = 1.0f;
			gamePaused = false;
		}

		SceneManager.LoadScene ("MainMenu", LoadSceneMode.Single);
	}
}
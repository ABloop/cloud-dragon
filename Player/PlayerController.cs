using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Controls both Players and allows other scripts to access them
/// </summary>
public class PlayerController : MonoBehaviour
{
	// PLAYER CONTROLLER
	[HideInInspector] public static PlayerController instance = null;

	// PLAYER MANAGER
	[HideInInspector] public GameObject pm;
	[HideInInspector] public PlayerManager playerManager;

	// PILOT
	[HideInInspector] private string playerChar;
	[HideInInspector] public IcaraiController icky;
	[HideInInspector] public ChorgController chorgie;

	// AR
	public GameObject gameBackground;
	public GameObject arCamera;

	public GameObject imageTarget1;
	public GameObject imageTarget2;
	public GameObject imageTarget3;
	public GameObject imageTarget4;
	public GameObject imageTarget5;

	// ICARAI ONLY
	[HideInInspector] public bool ickySpecialActive = false;

	void Start ()
	{
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);
		}

		pm = GameObject.Find ("PlayerManager");
		playerManager = pm.GetComponent<PlayerManager> ();

		playerChar = playerManager.pilot;

		if (playerManager.arCameraActive) {
			gameBackground.SetActive (false);
			arCamera.SetActive (true);

			imageTarget1.SetActive (true);
			imageTarget2.SetActive (true);
			imageTarget3.SetActive (true);
			imageTarget4.SetActive (true);
			imageTarget5.SetActive (true);
		}
	}

	public float DamageToTake ()
	{
		float damage = 0.0f;

		if (playerChar == "Icarai") {
			damage = icky.damage;
		} else if (playerChar == "Chorg") {
			damage = chorgie.damage;
		}

		return damage;
	}

	public void ApplyDebuff ()
	{
		if (playerChar == "Icarai") {
			icky.slowed = true;
		} else if (playerChar == "Chorg") {
			chorgie.slowed = true;
		}
	}

	public void ClearChorgSpecial ()
	{
		chorgie.ClearSpecial ();
	}

	public void UpdateScore (int points)
	{
		if (playerChar == "Icarai") {
			icky.UpdateScore (points);
		} else if (playerChar == "Chorg") {
			chorgie.UpdateScore (points);
		}
	}

	public void BossApproaching ()
	{
		if (playerChar == "Icarai") {
			icky.bossWarning = true;
		} else if (playerChar == "Chorg") {
			chorgie.bossWarning = true;
		}
	}

	public void LevelStarted ()
	{

		if (playerChar == "Icarai") {
			icky.LevelStarted ();
		} else if (playerChar == "Chorg") {
			chorgie.LevelStarted ();
		}
	}

	public void LevelFinished ()
	{
		if (playerChar == "Icarai") {
			icky.LevelFinished ();
		} else if (playerChar == "Chorg") {
			chorgie.LevelFinished ();
		}
	}

	public void GamePaused ()
	{
		if (playerChar == "Icarai") {
			icky.GamePaused ();
		} else if (playerChar == "Chorg") {
			chorgie.GamePaused ();
		}
	}
}
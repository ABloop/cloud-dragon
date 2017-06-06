using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls the Options Canvas on the Main Menu
/// </summary>
public class OptionsController : MonoBehaviour
{
	// PLAYER MANAGER
	private GameObject pm;
	private PlayerManager playerManager;

	// UI
	public GameObject mmCanvas;
	public GameObject opCanvas;

	public Dropdown controlsDD;
	public Toggle arToggle;
	public Toggle htpToggle;

	void Start ()
	{
		pm = GameObject.Find ("PlayerManager");
		playerManager = pm.GetComponent<PlayerManager> ();
		controlsDD.captionText.text = playerManager.controlScheme;

		if (playerManager.controlScheme == "Touch") {
			controlsDD.value = 0;
		} else if (playerManager.controlScheme == "Tilt") {
			controlsDD.value = 1;
		}
	
		arToggle.isOn = playerManager.arCameraActive;
		htpToggle.isOn = playerManager.howToPlay;
	}

	public void ControlsDropdownChanged ()
	{
		playerManager.controlScheme = controlsDD.captionText.text;
	}

	public void ARCheckboxChanged ()
	{
		if (arToggle.isOn) {
			playerManager.arCameraActive = true;
		} else {
			playerManager.arCameraActive = false;
		}
	}

	public void HTPCheckboxChanged ()
	{
		if (htpToggle.isOn) {
			playerManager.howToPlay = true;
		} else {
			playerManager.howToPlay = false;
		}
	}

	public void Back ()
	{
		opCanvas.SetActive (false);
		mmCanvas.SetActive (true);
	}
}
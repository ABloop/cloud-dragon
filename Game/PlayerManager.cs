using System.Collections;
using UnityEngine;

/// <summary>
/// Manages the Player Settings and Options throughout the runtime of the game
/// </summary>
public class PlayerManager : MonoBehaviour
{
	// PILOT SETTINGS AND SCORES
	public string pilot;
	public int score;

	// CONTROLS
	public string controlScheme = "Touch";
	public bool arCameraActive = false;
	public bool howToPlay = true;

	void Start ()
	{
		DontDestroyOnLoad (gameObject);
	}
}
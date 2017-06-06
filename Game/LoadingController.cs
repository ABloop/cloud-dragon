using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Controls the Loading Scene
/// </summary>
public class LoadingController : MonoBehaviour {

	// TIMER
	private float loadingTimer = 5.0f;

	void Update () {
		loadingTimer -= Time.deltaTime;

		if (loadingTimer <= 0.0f) {
			SceneManager.LoadScene ("MainMenu", LoadSceneMode.Single);
		}
	}
}
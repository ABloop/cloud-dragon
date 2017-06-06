using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Controls the PilotSelect scene
/// </summary>
public class PilotSelectController : MonoBehaviour
{
	// PLAYER MANAGER
	private GameObject pm;
	private PlayerManager playerManager;

	// PILOT SELECTION
	private string pilotToView;

	// UI
	public GameObject pilotSelection;
	public GameObject pilotSelected;
	public GameObject controls;

	public Sprite[] pilotPortraits;
	public Image pilotPortrait;
	public Text pilotName;
	public Text weaponType;
	public Text speed;
	public Sprite[] specialSprites;
	public Image specialImage;
	public Text specialDesc;

	public Text howToPlayText;
	public Image touchControls;
	public Image tiltControls;
	public Button okButton;
	public Text loadingText;

	// BACKGROUND
	public SpriteRenderer background;
	private float fader = 1.0f;
	private bool levelStarting = false;

	// LOADING
	private bool controlsCanvasLoaded = false;
	private bool notViewingControls = false;
	private float loadingTimer = 5.0f;

	void Start ()
	{
		pm = GameObject.Find ("PlayerManager");
		playerManager = pm.GetComponent <PlayerManager> ();
	}

	void Update ()
	{
		if (levelStarting) {

			if (background.color.a <= 0.0f && !controlsCanvasLoaded) {
				controls.SetActive (true);
				loadControls ();
			} else if (background.color.a > 0.0f) {
				fader -= Time.fixedDeltaTime;
				background.color = new Color (1.0f, 1.0f, 1.0f, fader);
			}

			if (notViewingControls) {
				loadingTimer -= Time.fixedDeltaTime;

				if (loadingTimer <= 0.0f) {
					BeginLevel ();
				}
			}
		}
	}

	public void Icarai ()
	{
		pilotToView = "Icarai";
		pilotPortrait.sprite = pilotPortraits [0];
		pilotName.text = "ICARAI";
		weaponType.text = "Weapons: Twin Lasers";
		speed.text = "Speed: Medium";
		specialImage.sprite = specialSprites [0];
		specialDesc.text = "TIME STOPPER" + "\r\n" + "Freeze all enemies on the screen for a duration";

		pilotSelection.SetActive (false);
		pilotSelected.SetActive (true);
	}

	public void Chorg ()
	{
		pilotToView = "Chorg";

		pilotPortrait.sprite = pilotPortraits [1];
		pilotName.text = "CHORG";
		weaponType.text = "Weapons: Dual Gatling Guns";
		speed.text = "Speed: Slow";
		specialImage.sprite = specialSprites [1];
		specialDesc.text = "BUSTER SHOT" + "\r\n" + "An expanding cannon ball for smashing enemies";
		pilotSelection.SetActive (false);
		pilotSelected.SetActive (true);
	}

	public void Launch ()
	{
		if (pilotToView == "Icarai") {
			playerManager.pilot = "Icarai";
		} else if (pilotToView == "Chorg") {
			playerManager.pilot = "Chorg";
		}

		pilotSelected.SetActive (false);
		levelStarting = true;
	}

	void loadControls ()
	{
		if (playerManager.howToPlay) {

			if (playerManager.controlScheme == "Touch") {
				touchControls.gameObject.SetActive (true);
			} else if (playerManager.controlScheme == "Tilt") {
				tiltControls.gameObject.SetActive (true);
			}

			howToPlayText.gameObject.SetActive (true);
			okButton.gameObject.SetActive (true);
		} else {
			loadingText.gameObject.SetActive (true);
			notViewingControls = true;
		}

		controlsCanvasLoaded = true;
	}

	public void BeginLevel ()
	{
		if (pilotToView == "Icarai") {
			SceneManager.LoadScene ("Icarai", LoadSceneMode.Single);
		} else if (pilotToView == "Chorg") {
			SceneManager.LoadScene ("Chorg", LoadSceneMode.Single);
		}
	}

	public void Back ()
	{
		SceneManager.LoadScene ("MainMenu", LoadSceneMode.Single);
	}

	public void Return ()
	{
		pilotSelection.SetActive (true);
		pilotSelected.SetActive (false);
	}
}
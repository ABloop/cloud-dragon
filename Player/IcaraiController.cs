/*
Accelerometer Controls inspired by user Dan (2015) from StackOverflow, and
from Unity Scripting API (2017).

Editor Controls adapted from Geig (2014).
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Controls Icarai, including attacks, special and movement
/// </summary>
public class IcaraiController : MonoBehaviour
{
	// PLAYER CONTROLLER
	[HideInInspector] public PlayerController pc;

	// STATS
	private int hitPoints;
	private int ammo;
	private float speed;
	private int speedValue;
	private float ammoRecharge;
	[HideInInspector] public float damage;
	private int score;

	// POWER UP AND STATUS TIMERS
	private bool speedIncreased = false;
	private bool damageIncreased = false;
	private bool statusTextActive = false;
	private float damageIncreaseTimer = 30.0f;
	private float speedIncreaseTimer = 30.0f;
	private float statusTextTimer = 2.0f;

	[HideInInspector] public bool slowed = false;
	private float debuffTimer = 5.0f;

	[HideInInspector] public bool bossWarning = false;
	private float warningTimer = 2.0f;

	// UI ELEMENTS
	public Text healthText;
	public Text ammoText;
	public Text speedText;
	public Text scoreText;
	public Text startText;
	public Text statusText;
	public Button specialButton;

	// SPECIAL
	private int specialCharge;
	private bool specialReady;
	private bool specialActive = false;
	private float specialTimer = 10.0f;

	// COMPONENTS AND GAMEOBJECTS
	private Rigidbody2D rb2d;
	private Animator anim;
	private BoxCollider2D bc2d;

	public Quaternion origin;
	public GameObject startPoint;
	private bool destinationReached = false;

	// BLASTER GAMEOBJECTS AND POSITIONS
	public GameObject laser;
	public GameObject laserPU;
	public GameObject laserHY;

	public Transform leftCannon;
	public Transform rightCannon;

	private bool hasFired = false;
	private float blasterRecharge = 0.15f;

	//TILT CONTROLS
	private float accelerometerUpdateInterval = 1.0f / 60.0f;
	private float accelerometerInputSpeed = 0.5f;
	private float shakeLimit = 1.0f;

	private float accelerometerFilter;
	private Vector3 shakeValue = Vector3.zero;
	private Vector3 acceleration;
	private Vector3 accelerationChange;

	void Start ()
	{
		rb2d = GetComponent <Rigidbody2D> ();
		anim = GetComponent <Animator> ();
		bc2d = GetComponent <BoxCollider2D> ();
		origin = transform.rotation;

		bc2d.enabled = false;

		hitPoints = 100;
		ammo = 100;
		speed = 10.0f;
		speedValue = 100;
		ammoRecharge = 10.0f;
		damage = 1.5f;
		score = 0;

		pc = PlayerController.instance;
		pc.icky = this;

		if (pc.playerManager.controlScheme == "Tilt") {
			accelerometerFilter = accelerometerUpdateInterval / accelerometerInputSpeed;
		}
	}

	void Update ()
	{
		// MOVE TO STARTING POS WHEN GAME IS STARTING
		if (GameManager.instance.levelStarting == true) {
			LevelStarting ();
			// MOVE TO FINISHING POS WHEN GAME IS ENDING
		} else if (GameManager.instance.levelFinished == true) {
			LevelFinishing ();
			// OTHERWISE WHILE GAME IS PLAYING AND PLAYER NOT DEAD, DO THIS
		} else {

			if (!GameManager.instance.gamePaused) {
				
				if (hitPoints > 0) {
					#if UNITY_EDITOR
					// EDITOR CONTROLS
					EditorControls ();
					#endif

					if (pc.playerManager.controlScheme == "Touch") {
						// TOUCH CONTROLS
						TouchControls ();
					} else if (pc.playerManager.controlScheme == "Tilt") {
						// ACCELEROMETER CONTROLS
						TiltControls ();
					}

					AmmoRecharge ();
					SpecialActiveTimer ();
					SpeedBuffTimer ();
					DamageBuffTimer ();
					SlowDebuffTimer ();
					StatusTextTimer ();
					BossWarningTimer ();
				} else {
					GameOver ();
				}
			}
		}
	}

	void EditorControls ()
	{
		float h = Input.GetAxis ("Horizontal");
		float v = Input.GetAxis ("Vertical");

		rb2d.AddForce (gameObject.transform.right * (speed * 4) * h);
		rb2d.AddForce (gameObject.transform.up * (speed * 4) * v);

		if (Input.GetKeyDown ("a")) {
			transform.Rotate (0, -15, 0);
		} else if (Input.GetKeyDown ("d")) {
			transform.Rotate (0, 15, 0);
		} else if (Input.anyKey == false) {
			transform.rotation = Quaternion.identity;
		}
	}

	void TouchControls ()
	{
		if (Input.touchCount > 0 && Input.GetTouch (0).phase != TouchPhase.Ended) {
			// CHECK TO SEE IF TOUCH IS WITHIN THE TOUCH AREA
			bool isTouchableArea = false;
			Ray ray = Camera.main.ScreenPointToRay (Input.GetTouch (0).position);
			RaycastHit hit;

			if (Physics.Raycast (ray, out hit, Mathf.Infinity)) {
				if (hit.collider != null && hit.collider.tag == "Touch Area") {
					isTouchableArea = true;
				} else {
					isTouchableArea = false;
				}
			}

			// IF YES, BEGIN MOVING
			if (isTouchableArea) {
				Vector2 touchPos = Camera.main.ScreenToWorldPoint (Input.GetTouch (0).position);

				if (touchPos.x > transform.position.x) {
					transform.Translate (Vector3.right * speed * Time.deltaTime);
				} else if (touchPos.x < transform.position.x) {
					transform.Translate (Vector3.left * speed * Time.deltaTime);
				} 

				if (touchPos.y > transform.position.y) {
					transform.Translate (Vector3.up * (speed / 2.0f) * Time.deltaTime);
				} else if (touchPos.y < transform.position.y) {
					transform.Translate (Vector3.down * (speed / 2.0f) * Time.deltaTime);
				}

			}
		}
	}

	void TiltControls ()
	{
		// SIMPLE ACCELEROMETER CONTROLS (MOVEMENT ONLY)
		//		Vector2 dir = Vector2.zero;

		//		dir.x = Input.acceleration.x;
		//		dir.y = Input.acceleration.y;
		//
		//		if (dir.sqrMagnitude > 1)
		//			dir.Normalize ();
		//
		//		dir *= Time.deltaTime;
		//		transform.Translate (dir * speed * 2.0f);

		// MAIN ACCELEROMETER CONTROLS
		Vector2 dir = Vector2.zero;
											
		dir.x = Input.acceleration.x;
		dir.y = Input.acceleration.y;
											
		if (dir.sqrMagnitude > 1)
			dir.Normalize ();
											
		dir *= Time.deltaTime;

		// MOVE IN THAT DIRECTION
		if (dir.y > 0.008 || dir.y < 0.003) {
			transform.Translate (new Vector2 (dir.x, dir.y / 2) * speed * 2.0f);
		} else {
			transform.Translate (new Vector2 (dir.x, 0) * speed * 2.0f);
		}
	
		// CHECK FOR THRESHOLD VALUES BEFORE FIRING
		if (dir.y <= 0.008 && dir.y > 0.003 && !hasFired) {
			Blasters ();
			hasFired = true;
			blasterRecharge = 0.15f;
		} else if (hasFired) {
			BlasterRecharge ();
		}

		// IF TILT EXCEEDS LIMIT, DEVICE IS SHAKING
		acceleration = Input.acceleration;
		shakeValue = Vector3.Lerp (shakeValue, acceleration, accelerometerFilter);
		accelerationChange = acceleration - shakeValue;

		if (accelerationChange.sqrMagnitude >= shakeLimit) {
			Special ();
		}
	}

	public void Blasters ()
	{
		if (!GameManager.instance.gamePaused) {
			
			if (ammo > 0 || specialActive) {
				GameObject lLaser;
				GameObject rLaser;

				if (specialActive) {
					lLaser = (GameObject)Instantiate (laserHY, 
						leftCannon.position, leftCannon.rotation);
					rLaser = (GameObject)Instantiate (laserHY, 
						rightCannon.position, rightCannon.rotation);
				} else if (damageIncreased) {
					lLaser = (GameObject)Instantiate (laserPU, 
						leftCannon.position, leftCannon.rotation);
					rLaser = (GameObject)Instantiate (laserPU, 
						rightCannon.position, rightCannon.rotation);
				} else {
					lLaser = (GameObject)Instantiate (laser, 
						leftCannon.position, leftCannon.rotation);
					rLaser = (GameObject)Instantiate (laser, 
						rightCannon.position, rightCannon.rotation);
				}

				lLaser.SetActive (true);
				rLaser.SetActive (true);

				if (specialActive) {
					lLaser.GetComponent <Rigidbody2D> ().AddForce (gameObject.transform.up * 2000);
					rLaser.GetComponent <Rigidbody2D> ().AddForce (gameObject.transform.up * 2000);
				} else {
					lLaser.GetComponent <Rigidbody2D> ().AddForce (gameObject.transform.up * 1000);
					rLaser.GetComponent <Rigidbody2D> ().AddForce (gameObject.transform.up * 1000);
				}

				Destroy (lLaser, 2.0f);
				Destroy (rLaser, 2.0f);

				if (!specialActive) {
					ammo = (ammo - 2);
				}

				if (ammo < 0) {
					ammo = 0;
				}

				UpdateAmmo ();
			}
		}
	}

	public void Special ()
	{
		if (!GameManager.instance.gamePaused) {

			if (specialReady && !specialActive) {
				specialActive = true;
				specialReady = false;
				specialCharge = 0;
				anim.SetTrigger ("specialStart");

				ArrayList en = new ArrayList ();

				try {
					en = GameManager.instance.waveEnemies;
				} catch (Exception e) {
					Debug.Log (e);
				}

				for (int i = 0; i < en.Capacity; i++) {
					
					try {
						GameObject enemy = (GameObject)en [i];

						if (enemy != null && enemy.tag != "Boss") {
							enemy.GetComponent<Rigidbody2D> ().isKinematic = true;

							if (enemy.GetComponent<ChomperController> () != null) {
								enemy.GetComponent<Animator> ().enabled = false;
							}

							if (enemy.GetComponent<PowerFistController> () != null) {
								enemy.GetComponent<PowerFistController> ().isFrozen = true;
								enemy.GetComponent<Animator> ().enabled = false;
							}

							if (enemy.GetComponentInChildren <SolarisController> () != null) {
								enemy.GetComponentInChildren<SolarisController> ().isFrozen = true;
								enemy.GetComponentInChildren<SolarisController> ().enabled = false;
								enemy.GetComponentInChildren<Animator> ().enabled = false;
							}

							if (enemy.GetComponent<MartianController> () != null) {
								enemy.GetComponent<MartianController> ().isFrozen = true;
								enemy.GetComponent<Animator> ().enabled = false;
							}

							if (enemy.GetComponent<QuazzarianController> () != null) {
								enemy.GetComponent<QuazzarianController> ().isFrozen = true;
								enemy.GetComponent<Animator> ().enabled = false;
							}
						}
					} catch (Exception e) {
						e.GetType ();
					}
				}

				try {
					en = GameManager.instance.hazards;
				} catch (Exception e) {
					Debug.Log (e);
				}

				for (int i = 0; i < en.Capacity; i++) {
					
					try {
						GameObject hazard = (GameObject)en [i];

						if (hazard != null) {
							
							if (hazard.GetComponent<AsteroidController> () != null) {
								hazard.GetComponent<AsteroidController> ().isFrozen = true;
								hazard.GetComponent<Rigidbody2D> ().isKinematic = true;
							}

							if (hazard.GetComponent<ZapGateController> () != null) {
								hazard.GetComponent<ZapGateController> ().isFrozen = true;
								hazard.GetComponent<Animator> ().enabled = false;
							} 
						}
					} catch (Exception e) {
						e.GetType ();
					}
				}

				try {
					en = GameManager.instance.powerUps;
				} catch (Exception e) {
					Debug.Log (e);
				}

				for (int i = 0; i < en.Capacity; i++) {
					
					try {
						GameObject powerUp = (GameObject)en [i];

						if (powerUp != null) {
							
							if (powerUp.GetComponent<PowerUpController> () != null) {
								powerUp.GetComponent<PowerUpController> ().isFrozen = true;
								powerUp.GetComponent<Rigidbody2D> ().isKinematic = true;
							}
						}
					} catch (Exception e) {
						e.GetType ();
					}
				}

				PlayerController.instance.ickySpecialActive = true;
			}
		}
	}

	public void ClearSpecial ()
	{
		ArrayList en = new ArrayList ();

		try {
			en = GameManager.instance.waveEnemies;
		} catch (Exception e) {
			Debug.Log (e);
		}

		for (int i = 0; i < en.Capacity; i++) {

			try {
				GameObject enemy = (GameObject)en [i];

				if (enemy != null && enemy.tag != "Boss") {
					enemy.GetComponent<Rigidbody2D> ().isKinematic = false;

					if (enemy.GetComponent<ChomperController> () != null) {
						enemy.GetComponent<Animator> ().enabled = true;
					}

					if (enemy.GetComponent<PowerFistController> () != null) {
						enemy.GetComponent<PowerFistController> ().isFrozen = false;
						enemy.GetComponent<Animator> ().enabled = true;
					} 

					if (enemy.GetComponentInChildren <SolarisController> () != null) {
						enemy.GetComponentInChildren<SolarisController> ().isFrozen = false;
						enemy.GetComponentInChildren<SolarisController> ().enabled = true;
						enemy.GetComponentInChildren<Animator> ().enabled = true;
					}

					if (enemy.GetComponent<MartianController> () != null) {
						enemy.GetComponent<MartianController> ().isFrozen = false;
						enemy.GetComponent<Animator> ().enabled = true;
					}

					if (enemy.GetComponent<QuazzarianController> () != null) {
						enemy.GetComponent<QuazzarianController> ().isFrozen = false;
						enemy.GetComponent<Animator> ().enabled = true;
					}
				}
			} catch (Exception e) {
				e.GetType ();
			}
		}

		try {
			en = GameManager.instance.hazards;
		} catch (Exception e) {
			Debug.Log (e);
		}

		for (int i = 0; i < en.Capacity; i++) {

			try {
				GameObject hazard = (GameObject)en [i];

				if (hazard != null) {
					
					if (hazard.GetComponent<AsteroidController> () != null) {
						hazard.GetComponent<AsteroidController> ().isFrozen = false;
						hazard.GetComponent<Rigidbody2D> ().isKinematic = false;
					}

					if (hazard.GetComponent<ZapGateController> () != null) {
						hazard.GetComponent<ZapGateController> ().isFrozen = false;
						hazard.GetComponent<Animator> ().enabled = true;
					} 
				}
			} catch (Exception e) {
				e.GetType ();
			}
		}

		try {
			en = GameManager.instance.powerUps;
		} catch (Exception e) {
			Debug.Log (e);
		}

		for (int i = 0; i < en.Capacity; i++) {
			
			try {
				GameObject powerUp = (GameObject)en [i];

				if (powerUp != null) {
					
					if (powerUp.GetComponent<PowerUpController> () != null) {
						powerUp.GetComponent<PowerUpController> ().isFrozen = false;
						powerUp.GetComponent<Rigidbody2D> ().isKinematic = false;
					}
				}
			} catch (Exception e) {
				e.GetType ();
			}
		}

		specialTimer = 10.0f;
		specialActive = false;
		anim.SetTrigger ("specialEnd");
		PlayerController.instance.ickySpecialActive = false;
		specialButton.animator.Rebind ();
	}

	void OnTriggerEnter2D (Collider2D other)
	{
		if (other.tag == "Enemy Attack") {
			hitPoints -= 5;
			UpdateScore (-5);

			if (hitPoints <= 0) {
				anim.SetTrigger ("boom");
				rb2d.isKinematic = true;
				bc2d.enabled = false;
				Destroy (gameObject, 2.0f);
			}
		}

		if (other.tag == "Enemy Debuff") {
			hitPoints -= 5;
			UpdateScore (-5);
			debuffTimer = 5.0f;
			speed = speed - 5.0f;

			if (speed < 5.0f) {
				speed = 5.0f;
			}

			speedValue = speedValue - 50;

			if (speedValue < 50) {
				speedValue = 50;
			}

			UpdateSpeed ();
			DisplayStatus ("-50 Speed");

			if (hitPoints <= 0) {
				anim.SetTrigger ("boom");
				rb2d.isKinematic = true;
				bc2d.enabled = false;
				Destroy (gameObject, 2.0f);
			}
		}

		if (other.tag == "Enemy Shield") {
			hitPoints -= 20;
			UpdateScore (-20);

			if (hitPoints <= 0) {
				anim.SetTrigger ("boom");
				rb2d.isKinematic = true;
				bc2d.enabled = false;
				Destroy (gameObject, 2.0f);
			}
		}

		if (other.tag == "Enemy") {
			hitPoints = hitPoints - 25;
			UpdateScore (-25);

			if (hitPoints <= 0) {
				anim.SetTrigger ("boom");
				rb2d.isKinematic = true;
				bc2d.enabled = false;
				Destroy (gameObject, 2.0f);
			}
		}

		if (other.tag == "Hazard") {
			hitPoints = hitPoints - 20;
			UpdateScore (-20);

			if (hitPoints <= 0) {
				anim.SetTrigger ("boom");
				rb2d.isKinematic = true;
				bc2d.enabled = false;
				Destroy (gameObject, 2.0f);
			}
		}

		if (other.tag == "Boss") {
			hitPoints = hitPoints - 50;
			UpdateScore (-50);

			if (hitPoints <= 0) {
				anim.SetTrigger ("boom");
				rb2d.isKinematic = true;
				bc2d.enabled = false;
				Destroy (gameObject, 2.0f);
			}
		}

		if (other.tag == "Power Up") {

			if (other.GetComponent<PowerUpController> ().buffApplied != true) {
				
				if (other.name == "HealthPowerUp(Clone)") {
					hitPoints = hitPoints + 25;

					if (hitPoints > 100) {
						hitPoints = 100;
					}

					DisplayStatus ("+25 HP");
				} else if (other.name == "AmmoPowerUp(Clone)") {
					ammo = ammo + 50;

					if (ammo > 100) {
						ammo = 100;
					}

					UpdateAmmo ();

					DisplayStatus ("+50 Ammo");
				} else if (other.name == "SpeedPowerUp(Clone)") {
					speed = speed + 5.0f;
					speedValue = speedValue + 50;

					speedIncreased = true;
					speedIncreaseTimer = 30.0f;

					UpdateSpeed ();
					DisplayStatus ("+50 Speed");
				} else if (other.name == "DamagePowerUp(Clone)") {
					damage = damage + 1.5f;
					damageIncreaseTimer = 30.0f;
					damageIncreased = true;

					DisplayStatus ("+1 Damage");
				} else if (other.name == "SpecialPowerUp(Clone)") {
					specialCharge = specialCharge + 50;
					UpdateScore (0);

					DisplayStatus ("+50 Special");
				}
			}
		}

		UpdateHitPoints ();
	}

	// TILT CONTROLS ONLY
	void BlasterRecharge ()
	{
		if (hasFired) {
			blasterRecharge -= Time.deltaTime;

			if (blasterRecharge <= 0.0f) {
				hasFired = false;
			}
		}
	}

	void AmmoRecharge ()
	{
		ammoRecharge -= Time.deltaTime;

		if (ammoRecharge <= 0.0f) {
			ammo = (ammo + 25);

			if (ammo > 100) {
				ammo = 100;
			}

			UpdateAmmo ();
			ammoRecharge = 10.0f;
		}
	}

	void SpecialActiveTimer ()
	{
		if (specialActive == true) {
			specialTimer -= Time.deltaTime;

			if (specialTimer <= 0.0f) {
				ClearSpecial ();
			}
		}
	}

	void SpeedBuffTimer ()
	{
		if (speedIncreased == true) {
			speedIncreaseTimer -= Time.deltaTime;

			if (speedIncreaseTimer <= 0.0f) {
				speedIncreased = false;
				speed = 10.0f;
				speedValue = 100;

				UpdateSpeed ();
			}
		}
	}

	void DamageBuffTimer ()
	{
		if (damageIncreased == true) {
			damageIncreaseTimer -= Time.deltaTime;

			if (damageIncreaseTimer <= 0.0f) {
				damageIncreased = false;
				damage = 1.5f;
			}
		}
	}

	void SlowDebuffTimer ()
	{
		if (slowed == true) {
			debuffTimer -= Time.deltaTime;

			if (debuffTimer <= 0.0f) {
				slowed = false;
				speed = 10.0f;
				speedValue = 100;

				UpdateSpeed ();
			}
		}
	}

	void StatusTextTimer ()
	{
		if (statusTextActive == true) {
			statusTextTimer -= Time.deltaTime;

			if (statusTextTimer <= 0.0f) {
				statusText.enabled = false;
				statusTextActive = false;
			}
		}
	}

	void BossWarningTimer ()
	{
		if (bossWarning == true) {
			startText.enabled = true;
			startText.text = "WARNING!";
			warningTimer -= Time.deltaTime;

			if (warningTimer <= 0.0f) {
				startText.enabled = false;
				bossWarning = false;
			}
		}
	}

	void LevelStarting ()
	{
		transform.Translate (Vector2.up * speed * Time.deltaTime);
	}

	void LevelFinishing ()
	{
		// UNTIL DESTINATION REACHED, KEEP MOVING TO END POS
		if (transform.position.x == startPoint.transform.position.x && transform.position.y == startPoint.transform.position.y) {
			destinationReached = true;
		}

		// WHEN DESTINATION REACHED, LEVEL ENDS
		if (!destinationReached) {
			transform.position = Vector2.MoveTowards (transform.position, startPoint.transform.position, ((speed / 2) * Time.deltaTime));
		} else {
			transform.position = Vector2.MoveTowards (transform.position, new Vector3 (0, 20, 0), ((speed / 2) * Time.deltaTime));
			GameManager.instance.levelEnding = true;
			pc.playerManager.score = score;
		}
	}

	public void LevelStarted ()
	{
		if (!GameManager.instance.bossDefeated) {
			bc2d.enabled = true;
			startText.enabled = false;
		}
	}

	public void LevelFinished ()
	{
		if (GameManager.instance.levelFinished == true) {
			bc2d.enabled = false;
			startText.enabled = true;
			startText.text = "YOU WIN!";
		}
	}

	public void GamePaused ()
	{
		if (GameManager.instance.gamePaused) {
			startText.enabled = true;
			startText.text = "PAUSED";
		} else {
			startText.enabled = false;
		}
	}

	void GameOver ()
	{
		startText.enabled = true;
		startText.text = "GAME OVER";
		statusText.enabled = false;
		GameManager.instance.gamePlaying = false;
	}

	void UpdateHitPoints ()
	{
		if (hitPoints < 0) {
			hitPoints = 0;
		}

		healthText.text = "HP: " + hitPoints;
	}

	void UpdateAmmo ()
	{
		ammoText.text = "Ammo: " + ammo;
	}

	void UpdateSpeed ()
	{
		speedText.text = "Speed: " + speedValue;
	}

	void DisplayStatus (string status)
	{
		statusText.enabled = true;
		statusText.text = status;
		statusTextTimer = 2.0f;
		statusTextActive = true;
	}

	public void UpdateScore (int points)
	{
		if (specialActive == true) {
			points = points * 2;
		}

		score = score + points;

		if (score < 0) {
			score = 0;
		}

		scoreText.text = "Score: " + score;

		if (points > 0 && !specialActive) {
			// FAST SPECIAL CHARGING
			//specialCharge = (specialCharge + 50);

			specialCharge = (specialCharge + points);
		}


		if (specialCharge >= 50 && specialCharge < 100) {
			specialButton.animator.SetTrigger ("state1");
		} else if (specialCharge >= 100 && specialCharge < 150) {
			specialButton.animator.SetTrigger ("state2");
		} else if (specialCharge >= 150 && specialCharge < 200) {
			specialButton.animator.SetTrigger ("state3");
		} else if (specialCharge >= 200) {
			specialButton.animator.SetTrigger ("state4");
			specialReady = true;
		}
	}
}
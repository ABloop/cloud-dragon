using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Controls Chorg, including attacks, special, and movement
/// </summary>
public class ChorgController : MonoBehaviour
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

	// COMPONENTS AND GAMEOBJECTS
	private Rigidbody2D rb2d;
	private Animator anim;
	public BoxCollider2D bc2dw;
	public BoxCollider2D bc2dx;
	public BoxCollider2D bc2dy;
	public BoxCollider2D bc2dz;

	public Quaternion origin;
	public GameObject startPoint;
	private bool destinationReached = false;

	// BLASTER GAMEOBJECTS
	public GameObject bullet;
	public GameObject bulletPU;
	public GameObject bulletHY;
	public GameObject busterShot;

	public Transform leftCannon1;
	public Transform leftCannon2;
	public Transform rightCannon1;
	public Transform rightCannon2;
	public Transform centreCannon;

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
		origin = transform.rotation;

		DeactivateColliders ();

		hitPoints = 125;
		ammo = 200;
		speed = 7.5f;
		speedValue = 100;
		ammoRecharge = 20.0f;
		damage = 0.75f;
		score = 0;

		pc = PlayerController.instance;
		pc.chorgie = this;

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
		// SIMPLE ACCELEROMETER CONTROLS (JUST FOR MOVEMENT)
//		Vector2 dir = Vector2.zero;
//
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

		if (dir.y > 0.01 || dir.y < 0.005) {
			transform.Translate (new Vector2 (dir.x, dir.y / 2) * speed * 2.0f);
		} else {
			transform.Translate (new Vector2 (dir.x, 0) * speed * 2.0f);
		}

		// BLASTERS
		if (dir.y <= 0.01 && dir.y > 0.005 && !hasFired) {
			Blasters ();
			hasFired = true;
			blasterRecharge = 0.15f;
		} else if (hasFired) {
			BlasterRecharge ();
		}

		// SPECIAL
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
				GameObject lBullet1, lBullet2, rBullet1, rBullet2;

				if (specialActive) {
					lBullet1 = (GameObject)Instantiate (bulletHY, 
						leftCannon1.position, leftCannon1.rotation);
					lBullet2 = (GameObject)Instantiate (bulletHY, 
						leftCannon2.position, leftCannon2.rotation);
					rBullet1 = (GameObject)Instantiate (bulletHY, 
						rightCannon1.position, rightCannon1.rotation);
					rBullet2 = (GameObject)Instantiate (bulletHY, 
						rightCannon2.position, rightCannon2.rotation);

				} else if (damageIncreased) {
					lBullet1 = (GameObject)Instantiate (bulletPU, 
						leftCannon1.position, leftCannon1.rotation);
					lBullet2 = (GameObject)Instantiate (bulletPU, 
						leftCannon2.position, leftCannon2.rotation);
					rBullet1 = (GameObject)Instantiate (bulletPU, 
						rightCannon1.position, rightCannon1.rotation);
					rBullet2 = (GameObject)Instantiate (bulletPU, 
						rightCannon2.position, rightCannon2.rotation);

				} else {
					lBullet1 = (GameObject)Instantiate (bullet, 
						leftCannon1.position, leftCannon1.rotation);
					lBullet2 = (GameObject)Instantiate (bullet, 
						leftCannon2.position, leftCannon2.rotation);
					rBullet1 = (GameObject)Instantiate (bullet, 
						rightCannon1.position, rightCannon1.rotation);
					rBullet2 = (GameObject)Instantiate (bullet, 
						rightCannon2.position, rightCannon2.rotation);
				}

				lBullet1.SetActive (true);
				lBullet2.SetActive (true);
				rBullet1.SetActive (true);
				rBullet2.SetActive (true);

				if (specialActive) {
					lBullet1.GetComponent <Rigidbody2D> ().AddForce (gameObject.transform.up * 2500);
					lBullet2.GetComponent <Rigidbody2D> ().AddForce (gameObject.transform.up * 2500);
					rBullet1.GetComponent <Rigidbody2D> ().AddForce (gameObject.transform.up * 2500);
					rBullet2.GetComponent <Rigidbody2D> ().AddForce (gameObject.transform.up * 2500);
				} else {
					lBullet1.GetComponent <Rigidbody2D> ().AddForce (gameObject.transform.up * 1250);
					lBullet2.GetComponent <Rigidbody2D> ().AddForce (gameObject.transform.up * 1250);
					rBullet1.GetComponent <Rigidbody2D> ().AddForce (gameObject.transform.up * 1250);
					rBullet2.GetComponent <Rigidbody2D> ().AddForce (gameObject.transform.up * 1250);
				}

				Destroy (lBullet1, 2.0f);
				Destroy (lBullet2, 2.0f);
				Destroy (rBullet1, 2.0f);
				Destroy (rBullet2, 2.0f);

				if (!specialActive) {
					ammo = (ammo - 4);
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
				GameObject buster;

				buster = (GameObject)Instantiate (busterShot, 
					centreCannon.position, centreCannon.rotation);
				buster.GetComponent <Rigidbody2D> ().AddForce (gameObject.transform.up * 400);
			}
		}
	}

	public void ClearSpecial ()
	{
		specialActive = false;
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
				DeactivateColliders ();
				Destroy (gameObject, 2.0f);
			}
		}

		if (other.tag == "Enemy Debuff") {
			hitPoints -= 5;
			UpdateScore (-5);

			debuffTimer = 5.0f;
			speed = speed - 3.75f;

			if (speed < 3.75f) {
				speed = 3.75f;
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
				DeactivateColliders ();
				Destroy (gameObject, 2.0f);
			}
		}

		if (other.tag == "Enemy Shield") {
			hitPoints -= 20;
			UpdateScore (-20);

			if (hitPoints <= 0) {
				anim.SetTrigger ("boom");
				rb2d.isKinematic = true;
				DeactivateColliders ();
				Destroy (gameObject, 2.0f);
			}
		}

		if (other.tag == "Enemy") {
			hitPoints = hitPoints - 25;
			UpdateScore (-25);

			if (hitPoints <= 0) {
				anim.SetTrigger ("boom");
				rb2d.isKinematic = true;
				DeactivateColliders ();
				Destroy (gameObject, 2.0f);
			}
		}

		if (other.tag == "Hazard") {
			hitPoints = hitPoints - 20;
			UpdateScore (-20);

			if (hitPoints <= 0) {
				anim.SetTrigger ("boom");
				rb2d.isKinematic = true;
				DeactivateColliders ();
				Destroy (gameObject, 2.0f);
			}
		}

		if (other.tag == "Boss") {
			hitPoints = hitPoints - 50;
			UpdateScore (-50);

			if (hitPoints <= 0) {
				anim.SetTrigger ("boom");
				rb2d.isKinematic = true;
				DeactivateColliders ();
				Destroy (gameObject, 2.0f);
			}
		}

		if (other.tag == "Power Up") {

			if (other.GetComponent<PowerUpController> ().buffApplied != true) {

				if (other.name == "HealthPowerUp(Clone)") {
					hitPoints = hitPoints + 35;

					if (hitPoints > 125) {
						hitPoints = 125;
					}

					DisplayStatus ("+35 HP");
				} else if (other.name == "AmmoPowerUp(Clone)") {
					ammo = ammo + 100;

					if (ammo > 200) {
						ammo = 200;
					}

					UpdateAmmo ();

					DisplayStatus ("+50 Ammo");
				} else if (other.name == "SpeedPowerUp(Clone)") {
					speed = speed + 3.75f;
					speedValue = speedValue + 50;

					speedIncreased = true;
					speedIncreaseTimer = 30.0f;

					UpdateSpeed ();
					DisplayStatus ("+50 Speed");
				} else if (other.name == "DamagePowerUp(Clone)") {
					damage = damage + 0.5f;
					damageIncreased = true;
					damageIncreaseTimer = 30.0f;

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
			ammo = (ammo + 50);

			if (ammo > 200) {
				ammo = 200;
			}

			UpdateAmmo ();
			ammoRecharge = 10.0f;
		}
	}

	void SpeedBuffTimer ()
	{
		if (speedIncreased == true) {
			speedIncreaseTimer -= Time.deltaTime;

			if (speedIncreaseTimer <= 0.0f) {
				speedIncreased = false;
				speed = 7.5f;
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
				damage = 1;
			}
		}
	}

	void SlowDebuffTimer ()
	{
		if (slowed == true) {
			debuffTimer -= Time.deltaTime;

			if (debuffTimer <= 0.0f) {
				slowed = false;
				speed = 7.5f;

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
		transform.Translate (Vector2.up * (speed + 2.5f) * Time.deltaTime);
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
			ActivateColliders ();
			startText.enabled = false;
		}
	}

	public void LevelFinished ()
	{
		if (GameManager.instance.levelFinished == true) {
			DeactivateColliders ();
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

	void ActivateColliders ()
	{
		bc2dw.enabled = true;
		bc2dx.enabled = true;
		bc2dy.enabled = true;
		bc2dz.enabled = true;
	}

	void DeactivateColliders ()
	{
		bc2dw.enabled = false;
		bc2dx.enabled = false;
		bc2dy.enabled = false;
		bc2dz.enabled = false;
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

		if (points > 0 && specialActive != true) {
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
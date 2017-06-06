using System.Collections;
using UnityEngine;

/// <summary>
/// Controls how the Warlock behaves and attacks
/// </summary>
public class WarlockController : MonoBehaviour
{
	// WARLOCK CONTROLLER
	public static WarlockController instance = null;

	// STATS
	private float hitPoints = 150.0f;
	private float speed = 5.0f;

	// BODY TRANSFORMS
	public Transform leftHand;
	public Transform rightHand;
	public Transform body;

	// ATTACKS
	private string attack;
	private bool hasAttacked = false;

	public GameObject enderBlast;
	public GameObject darkness;
	public GameObject shield;

	private string enderBlastPattern;
	public bool shieldActive = false;

	// SPAWNING
	private bool startingRitual = true;
	private bool stopped = false;

	// MOVEMENT
	public GameObject[] warlockPoints;
	public GameObject startPoint;
	private ArrayList points = new ArrayList ();

	private bool settingPoints = true;
	private int pointsToMoveTo;
	private bool moving = true;

	private Vector2 destination;
	private bool destinationSet = false;
	private bool destinationReached = false;

	private bool waiting = false;
	private float waitTimer = 1.0f;

	[HideInInspector] public bool isFrozen = false;

	// COMPONENTS
	private Animator animator;
	private PolygonCollider2D pc2d;
	private SpriteRenderer spRen;

	void Start ()
	{
		animator = GetComponent <Animator> ();
		pc2d = GetComponent <PolygonCollider2D> ();
		spRen = GetComponent <SpriteRenderer> ();
		pointsToMoveTo = SetMovePoints ();

		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy (gameObject);

		pc2d.enabled = false;
		spRen.color = new Color (1f, 1f, 1f, .5f);
	}

	void Update ()
	{
		if (!startingRitual && GameManager.instance.gamePlaying) {
			
			if (moving) {
				
				if (!destinationSet) {
					SetRandomDestination ();
				} else {

					if (!destinationReached) {
						transform.position = Vector2.MoveTowards (transform.position, destination, (speed * Time.deltaTime));

						if (transform.position.x == destination.x && transform.position.y == destination.y) {
							destinationReached = true;
							destinationSet = false;
							pointsToMoveTo--;
						}
					}
				}
			} else if (waiting) {
				waitTimer -= Time.deltaTime;
			}

			if (pointsToMoveTo == 0 && !hasAttacked) {
				animator.SetTrigger ("attack");
				hasAttacked = true;
				moving = false;
				waiting = true;
				waitTimer = 1.0f;
			}

			if (waitTimer <= 0.0f) {
				moving = true;
				waiting = false;
				pointsToMoveTo = SetMovePoints ();
				settingPoints = true;
				hasAttacked = false;
				waitTimer = 1.0f;
			}
		} else {
			transform.position = Vector2.MoveTowards (transform.position, startPoint.transform.position, (speed * Time.deltaTime));

			if (transform.position.x == startPoint.transform.position.x && transform.position.y == startPoint.transform.position.y) {
				
				if (stopped == false) {
					spRen.color = new Color (1f, 1f, 1f, 1f);
					animator.SetTrigger ("attack_empty");
					stopped = true;
				}
			}
		}

		if (hitPoints <= 50) {
			speed = 7.5f;
		} else if (hitPoints <= 100) {
			speed = 6.0f;
		}
	}

	void RitualFinished ()
	{
		startingRitual = false;
		pc2d.enabled = true;
	}

	void AttackChoice ()
	{
		SetAttackType ();

		if (attack == "Ender Blast") {
			EnderBlastWave ();
		} else if (attack == "Darkness") {
			DarkShroud ();
		} else if (attack == "Summon Minions") {
			SummonMinions ();
		} else if (attack == "Shield") {
			SummonShield ();
		}
	}

	void EnderBlastWave ()
	{
		var eb1 = (GameObject)Instantiate (enderBlast, leftHand.position, leftHand.rotation);
		var eb2 = (GameObject)Instantiate (enderBlast, leftHand.position, leftHand.rotation);
		var eb3 = (GameObject)Instantiate (enderBlast, leftHand.position, leftHand.rotation);
		var eb4 = (GameObject)Instantiate (enderBlast, leftHand.position, leftHand.rotation);
		var eb5 = (GameObject)Instantiate (enderBlast, leftHand.position, leftHand.rotation);
		var eb6 = (GameObject)Instantiate (enderBlast, rightHand.position, rightHand.rotation);
		var eb7 = (GameObject)Instantiate (enderBlast, rightHand.position, rightHand.rotation);
		var eb8 = (GameObject)Instantiate (enderBlast, rightHand.position, rightHand.rotation);
		var eb9 = (GameObject)Instantiate (enderBlast, rightHand.position, rightHand.rotation);
		var eb10 = (GameObject)Instantiate (enderBlast, rightHand.position, rightHand.rotation);

		eb2.transform.Rotate (0, 0, 45, Space.Self);
		eb3.transform.Rotate (0, 0, -45, Space.Self);
		eb4.transform.Rotate (0, 0, 90, Space.Self);
		eb5.transform.Rotate (0, 0, -90, Space.Self);
		eb7.transform.Rotate (0, 0, 45, Space.Self);
		eb8.transform.Rotate (0, 0, -45, Space.Self);
		eb9.transform.Rotate (0, 0, 90, Space.Self);
		eb10.transform.Rotate (0, 0, -90, Space.Self);

		eb2.GetComponent<EnderBlastController> ().directionX = 3f;
		eb3.GetComponent<EnderBlastController> ().directionX = -3f;
		eb4.GetComponent<EnderBlastController> ().directionX = 6f;
		eb5.GetComponent<EnderBlastController> ().directionX = -6f;
		eb7.GetComponent<EnderBlastController> ().directionX = 3f;
		eb8.GetComponent<EnderBlastController> ().directionX = -3f;
		eb9.GetComponent<EnderBlastController> ().directionX = 6f;
		eb10.GetComponent<EnderBlastController> ().directionX = -6f;

		SetEnderPattern ();

		if (enderBlastPattern == "Cone") {
			eb1.GetComponent<EnderBlastController> ().directionY = -10f;
			eb2.GetComponent<EnderBlastController> ().directionY = -7.5f;
			eb3.GetComponent<EnderBlastController> ().directionY = -7.5f;
			eb4.GetComponent<EnderBlastController> ().directionY = -5f;
			eb5.GetComponent<EnderBlastController> ().directionY = -5f;
			eb6.GetComponent<EnderBlastController> ().directionY = -10f;
			eb7.GetComponent<EnderBlastController> ().directionY = -7.5f;
			eb8.GetComponent<EnderBlastController> ().directionY = -7.5f;
			eb9.GetComponent<EnderBlastController> ().directionY = -5f;
			eb10.GetComponent<EnderBlastController> ().directionY = -5f;
		} else {
			eb1.GetComponent<EnderBlastController> ().attackPattern = enderBlastPattern;
			eb2.GetComponent<EnderBlastController> ().attackPattern = enderBlastPattern;
			eb3.GetComponent<EnderBlastController> ().attackPattern = enderBlastPattern;
			eb4.GetComponent<EnderBlastController> ().attackPattern = enderBlastPattern;
			eb5.GetComponent<EnderBlastController> ().attackPattern = enderBlastPattern;
			eb6.GetComponent<EnderBlastController> ().attackPattern = enderBlastPattern;
			eb7.GetComponent<EnderBlastController> ().attackPattern = enderBlastPattern;
			eb8.GetComponent<EnderBlastController> ().attackPattern = enderBlastPattern;
			eb9.GetComponent<EnderBlastController> ().attackPattern = enderBlastPattern;
			eb10.GetComponent<EnderBlastController> ().attackPattern = enderBlastPattern;
		}

		eb1.SetActive (true);
		eb2.SetActive (true);
		eb3.SetActive (true);
		eb4.SetActive (true);
		eb5.SetActive (true);
		eb6.SetActive (true);
		eb7.SetActive (true);
		eb8.SetActive (true);
		eb9.SetActive (true);
		eb10.SetActive (true);

		Destroy (eb1, 4.0f);
		Destroy (eb2, 4.0f);
		Destroy (eb3, 4.0f);
		Destroy (eb4, 4.0f);
		Destroy (eb5, 4.0f);
		Destroy (eb6, 4.0f);
		Destroy (eb7, 4.0f);
		Destroy (eb8, 4.0f);
		Destroy (eb9, 4.0f);
		Destroy (eb10, 4.0f);
	}

	void DarkShroud ()
	{
		var dk = (GameObject)Instantiate (darkness, body.position, body.rotation);
		dk.SetActive (true);
		Destroy (dk, 10.0f);
	}

	void SummonShield ()
	{
		var sh = (GameObject)Instantiate (shield, transform.position, transform.rotation);
		sh.SetActive (true);
		sh.transform.parent = transform;
		shieldActive = true;
	}

	void SummonMinions ()
	{
		GameManager.instance.waveManager.WarlockWave (GameManager.instance.waveEnemies);
	}

	void OnTriggerEnter2D (Collider2D other)
	{
		if (other.tag == "Laser") {

			if (!shieldActive) {
				hitPoints -= PlayerController.instance.DamageToTake ();
			}

			if (hitPoints <= 0) {
				animator.enabled = true;
				animator.SetTrigger ("die");
				gameObject.GetComponent <PolygonCollider2D> ().enabled = false;
				Destroy (gameObject, 0.8f);

				GameManager.instance.enemiesInPlay--;
				GameManager.instance.bossDefeated = true;
				PlayerController.instance.UpdateScore (200);
			}
		}

		if (other.tag == "BusterShot") {
			hitPoints = hitPoints - 50;

			if (hitPoints <= 0) {
				animator.enabled = true;
				animator.SetTrigger ("die");

				gameObject.GetComponent <PolygonCollider2D> ().enabled = false;
				Destroy (gameObject, 0.8f);

				GameManager.instance.enemiesInPlay--;
				GameManager.instance.bossDefeated = true;
				PlayerController.instance.UpdateScore (200);
			}
		}

		if (other.tag == "Despawn") {
			Destroy (gameObject, 0.0f);
			GameManager.instance.enemiesInPlay--;
			GameManager.instance.bossDefeated = true;
			PlayerController.instance.UpdateScore (200);
		}

		if (other.tag == "Player") {
			
			if (!shieldActive) {
				hitPoints -= 25;
			}

			if (hitPoints <= 0) {
				animator.enabled = true;
				animator.SetTrigger ("die");
				gameObject.GetComponent <PolygonCollider2D> ().enabled = false;
				Destroy (gameObject, 0.8f);

				GameManager.instance.enemiesInPlay--;
				GameManager.instance.bossDefeated = true;
				PlayerController.instance.UpdateScore (200);
			}
		}
	}

	void SetAttackType ()
	{
		int attackType = 0;

		if (!shieldActive) {
			attackType = Random.Range (1, 5);
		} else {
			attackType = Random.Range (1, 4);
		}

		switch (attackType) {
		case 1:
			attack = "Ender Blast";
			break;
		case 2:
			attack = "Darkness";
			break;
		case 3:
			attack = "Summon Minions";
			break;
		case 4:
			attack = "Shield";
			break;
		}
	}

	void SetRandomDestination ()
	{
		if (settingPoints == true) {
			points.Clear ();

			for (int i = 0; i < warlockPoints.Length; i++) {
				points.Add (warlockPoints [i]);
			}

			settingPoints = false;
		}

		if (pointsToMoveTo > 0) {
			bool locationFound = false;
			int randomPos = Random.Range (0, points.Count);

			while (!locationFound) {
				GameObject point = (GameObject)points [randomPos];
				destination = new Vector2 (point.transform.position.x, point.transform.position.y);

				locationFound = true;
				points.RemoveAt (randomPos);

				destinationSet = true;
				destinationReached = false;
			}
		}
	}

	int SetMovePoints ()
	{
		int mp = Random.Range (1, 6);
		return mp;
	}

	void SetEnderPattern ()
	{
		int patternSelection = 0;
		patternSelection = Random.Range (1, 4);

		switch (patternSelection) {
		case 1:
			enderBlastPattern = "Cone";
			break;
		case 2:
			enderBlastPattern = "Diagonal Left";
			break;
		case 3:
			enderBlastPattern = "Diagonal Right";
			break;
		}
	}
}
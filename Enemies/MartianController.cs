using System.Collections;
using UnityEngine;

/// <summary>
/// Controls how the Martian enemy behaves and attacks
/// </summary>
public class MartianController : MonoBehaviour
{
	// PLAYER CONTROLLER
	public PlayerController player;

	// STATS
	private float hitPoints = 10.0f;

	// MOVEMENT
	private bool playerInRange;
	private float maxDistance;
	private Vector2 stoppingPos;
	private string direction;

	[HideInInspector] public bool isFrozen = false;

	// BLASTIE GAMEOBJECT, POSITIONS AND TIMINGS
	public GameObject blastie;
	public Transform leftCannon;
	public Transform rightCannon;

	private float attackTimer;
	private int burstsFired = 0;
	private bool firing = false;

	// COMPONENTS
	private Rigidbody2D rb2d;
	private Animator animator;

	void Start ()
	{
		rb2d = GetComponent <Rigidbody2D> ();
		animator = GetComponent <Animator> ();

		player = PlayerController.instance;
		playerInRange = false;
		maxDistance = Vector3.Distance (player.transform.position, transform.position);
		attackTimer = SetAttackTimer ();

		// HEALTH INCREASES WITH DIFFICULTY
		if (GameManager.instance.difficulty >= 5) {
			hitPoints = 15.0f;
		}
	}

	void Update ()
	{
		if (!isFrozen) {
			if (!playerInRange) {
				float currentDistance = Vector3.Distance (player.transform.position, transform.position);

				if (currentDistance < (maxDistance / 2)) {
					playerInRange = true;
					stoppingPos = new Vector2 (transform.position.x, transform.position.y);
					direction = ChooseStartingDirection ();
				}
			} else {
				
				if (!firing) {
					
					if (direction == "left") {
						rb2d.MovePosition (rb2d.position - new Vector2 (5, 0.5f) * Time.fixedDeltaTime);
					} else if (direction == "right") {
						rb2d.MovePosition (rb2d.position - new Vector2 (-5, 0.5f) * Time.fixedDeltaTime);
					}
				}

				attackTimer -= Time.deltaTime;

				if (attackTimer <= 0.0f & attackTimer > -1.0f) {
					firing = true;
					rb2d.isKinematic = true;
					animator.SetTrigger ("attack");
				}
			}
		}
	}

	void fireCannons ()
	{
		var blastie1 = (GameObject)Instantiate (blastie, leftCannon.position, leftCannon.rotation);
		var blastie2 = (GameObject)Instantiate (blastie, rightCannon.position, rightCannon.rotation);

		blastie1.SetActive (true);
		blastie2.SetActive (true);
		burstsFired++;

		Destroy (blastie1, 2.0f);
		Destroy (blastie2, 2.0f);

		if (burstsFired == 3) {
			attackTimer = SetAttackTimer ();
			firing = false;
			burstsFired = 0;
			rb2d.isKinematic = false;

			if (direction == "left") {
				direction = "right";
			} else {
				direction = "left";
			}
		}
	}

	void OnTriggerEnter2D (Collider2D other)
	{
		if (other.tag == "Laser") {
			hitPoints -= PlayerController.instance.DamageToTake ();

			if (hitPoints <= 0) {
				animator.enabled = true;
				animator.SetTrigger ("martianDie");

				rb2d.isKinematic = true;
				gameObject.GetComponent <CircleCollider2D> ().enabled = false;
				isFrozen = true;
				Destroy (gameObject, 0.8f);

				GameManager.instance.enemiesInPlay--;
				PlayerController.instance.UpdateScore (25);
				RandomPowerUp ();
			}
		}

		if (other.tag == "BusterShot") {
			hitPoints = hitPoints - 50;

			if (hitPoints <= 0) {
				animator.enabled = true;
				animator.SetTrigger ("martianDie");

				rb2d.isKinematic = true;
				gameObject.GetComponent <CircleCollider2D> ().enabled = false;
				isFrozen = true;
				Destroy (gameObject, 0.8f);

				GameManager.instance.enemiesInPlay--;
				PlayerController.instance.UpdateScore (25);
				RandomPowerUp ();
			}
		}

		if (other.tag == "Despawn") {
			Destroy (gameObject, 0f);
			GameManager.instance.enemiesInPlay--;
		}

		if (other.tag == "Player") {
			animator.enabled = true;
			animator.SetTrigger ("martianDie");

			rb2d.isKinematic = true;
			gameObject.GetComponent <CircleCollider2D> ().enabled = false;
			isFrozen = true;
			Destroy (gameObject, 0.8f);
			GameManager.instance.enemiesInPlay--;
		}
	}

	string ChooseStartingDirection ()
	{
		string dir;

		if (stoppingPos.x > 0) {
			dir = "left";
		} else {
			dir = "right";
		}

		return dir;
	}

	float SetAttackTimer ()
	{
		float aT = Random.Range (1.0f, 2.0f);
		return aT;
	}

	void RandomPowerUp ()
	{
		int willItSpawn = Random.Range (1, 11);
		bool itSpawns = false;

		if (willItSpawn == 8) {
			itSpawns = true;
		}

		if (itSpawns) {
			GameManager.instance.powerUpManager.EnemyDropPowerUp (GameManager.instance.powerUps, transform);
		}
	}
}
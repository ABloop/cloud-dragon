using System.Collections;
using UnityEngine;

/// <summary>
/// Controls how the Power Fist enemy behaves.
/// </summary>
public class PowerFistController : MonoBehaviour
{
	// PLAYER CONTROLLER
	public GameObject player;

	// MOVEMENT
	private bool playerInRange;
	private float maxDistance;
	private float hitPoints = 6.0f;

	[HideInInspector] public bool isFrozen = false;

	// COMPONENTS
	private Animator animator;
	private Rigidbody2D rb2d;
	private	PolygonCollider2D pc2d;

	void Start ()
	{
		animator = GetComponent <Animator> ();
		rb2d = GetComponent <Rigidbody2D> ();

		playerInRange = false;
		maxDistance = Vector3.Distance (PlayerController.instance.transform.position, transform.position);
	}

	void Update ()
	{
		if (!isFrozen) {

			if (!playerInRange) {
				float currentDistance = Vector3.Distance (PlayerController.instance.transform.position, transform.position);
		
				if (currentDistance < (maxDistance / 2) || transform.position.y < PlayerController.instance.transform.position.y) {
					playerInRange = true;
				}

				transform.position = Vector3.MoveTowards (transform.position, PlayerController.instance.transform.position, (4.0f * Time.deltaTime));
			} else {
				animator.SetTrigger ("inRange");
				rb2d.isKinematic = false;

				// MOVES FASTER ON HIGHER DIFFICULTY
				if (GameManager.instance.difficulty >= 4) {
					rb2d.gravityScale = 6f;
				} else {
					rb2d.gravityScale = 3f;
				}
			}
		}
	}

	void OnTriggerEnter2D (Collider2D other)
	{
		if (other.tag == "Laser") {
			
			hitPoints -= PlayerController.instance.DamageToTake ();

			if (hitPoints <= 0) {
				animator.enabled = true;
				animator.SetTrigger ("powerFistDie");
			
				rb2d.isKinematic = true;
				gameObject.GetComponent <PolygonCollider2D> ().enabled = false;
				isFrozen = true;
				Destroy (gameObject, 0.8f);

				GameManager.instance.enemiesInPlay--;
				PlayerController.instance.UpdateScore (15);
				RandomPowerUp ();
			}
		}

		if (other.tag == "BusterShot") {
			hitPoints = hitPoints - 50;

			if (hitPoints <= 0) {
				animator.enabled = true;
				animator.SetTrigger ("powerFistDie");

				rb2d.isKinematic = true;
				gameObject.GetComponent <PolygonCollider2D> ().enabled = false;
				isFrozen = true;
				Destroy (gameObject, 0.8f);

				GameManager.instance.enemiesInPlay--;
				PlayerController.instance.UpdateScore (15);
				RandomPowerUp ();
			}
		}

		if (other.tag == "Despawn") {
			Destroy (gameObject, 0.0f);
			GameManager.instance.enemiesInPlay--;
		}

		if (other.tag == "Player") {
			animator.enabled = true;
			animator.SetTrigger ("powerFistDie");

			rb2d.isKinematic = true;
			gameObject.GetComponent <PolygonCollider2D> ().enabled = false;
			isFrozen = true;
			Destroy (gameObject, 0.8f);
			GameManager.instance.enemiesInPlay--;
		}
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
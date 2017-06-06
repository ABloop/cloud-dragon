using System.Collections;
using UnityEngine;

/// <summary>
/// Controls how the Quazzarian behaves, including movement and attacks
/// </summary>
public class QuazzarianController : MonoBehaviour
{
	// STATS
	private float hitPoints = 10.0f;

	// BLOBL GAMEOBJECT, POSITION AND TIMER
	public Transform beak;
	public GameObject blobl;
	private float attackTimer;

	// MOVEMENT
	private float moveTimer;
	private float moveDir;

	[HideInInspector] public bool isFrozen = false;

	// COMPONENTS
	private Animator animator;
	private Rigidbody2D rb2d;

	void Start ()
	{
		animator = GetComponent <Animator> ();
		rb2d = GetComponent <Rigidbody2D> ();

		attackTimer = SetAttackTimer ();
		moveTimer = 3.0f;
		moveDir = SetDirection ();

		// INCREASES HEALTH BASED ON DIFFICULTY
		if (GameManager.instance.difficulty >= 5) {
			hitPoints = 12;
		}
	}

	void Update ()
	{
		if (!isFrozen) {
			rb2d.MovePosition (rb2d.position + new Vector2 (moveDir, -5) * Time.fixedDeltaTime);
			attackTimer -= Time.deltaTime;
			moveTimer -= Time.deltaTime;

			if (attackTimer <= 0.0f) {
				animator.SetTrigger ("attack");
				attackTimer = SetAttackTimer ();
			}

			if (moveTimer <= 0.0f) {
				moveTimer = 3.0f;
				moveDir = SetDirection ();
			}
		}
	}

	void bloblBlast ()
	{
		// SPAWNS MORE BLOBLS ON HIGHER DIFFICULY
		if (GameManager.instance.difficulty >= 5) {
			var bl1 = (GameObject)Instantiate (blobl, beak.position, beak.rotation);
			var bl2 = (GameObject)Instantiate (blobl, beak.position, beak.rotation);
			var bl3 = (GameObject)Instantiate (blobl, beak.position, beak.rotation);
			var bl4 = (GameObject)Instantiate (blobl, beak.position, beak.rotation);
			var bl5 = (GameObject)Instantiate (blobl, beak.position, beak.rotation);

			bl2.transform.Rotate (0, 0, 30, Space.Self);
			bl3.transform.Rotate (0, 0, -30, Space.Self);
			bl4.transform.Rotate (0, 0, 60, Space.Self);
			bl5.transform.Rotate (0, 0, -60, Space.Self);

			bl2.GetComponent<BloblController> ().direction = 2;
			bl3.GetComponent<BloblController> ().direction = -2;
			bl4.GetComponent<BloblController> ().direction = 4;
			bl5.GetComponent<BloblController> ().direction = -4;

			bl1.SetActive (true);
			bl2.SetActive (true);
			bl3.SetActive (true);
			bl4.SetActive (true);
			bl5.SetActive (true);

			Destroy (bl1, 3.0f);
			Destroy (bl2, 3.0f);
			Destroy (bl3, 3.0f);
			Destroy (bl4, 3.0f);
			Destroy (bl5, 3.0f);
		} else {
			var bl1 = (GameObject)Instantiate (blobl, beak.position, beak.rotation);
			var bl2 = (GameObject)Instantiate (blobl, beak.position, beak.rotation);
			var bl3 = (GameObject)Instantiate (blobl, beak.position, beak.rotation);

			bl2.transform.Rotate (0, 0, 45, Space.Self);
			bl3.transform.Rotate (0, 0, -45, Space.Self);

			bl2.GetComponent<BloblController> ().direction = 3;
			bl3.GetComponent<BloblController> ().direction = -3;

			bl1.SetActive (true);
			bl2.SetActive (true);
			bl3.SetActive (true);

			Destroy (bl1, 3.0f);
			Destroy (bl2, 3.0f);
			Destroy (bl3, 3.0f);
		}
	}

	void OnTriggerEnter2D (Collider2D other)
	{
		if (other.tag == "Laser") {
			hitPoints -= PlayerController.instance.DamageToTake ();

			if (hitPoints <= 0) {
				animator.enabled = true;
				animator.SetTrigger ("die");

				gameObject.GetComponent <Rigidbody2D> ().isKinematic = true;
				gameObject.GetComponent <PolygonCollider2D> ().enabled = false;
				isFrozen = true;
				Destroy (gameObject, 0.8f);

				GameManager.instance.enemiesInPlay--;
				PlayerController.instance.UpdateScore (20);
				RandomPowerUp ();
			}
		}

		if (other.tag == "BusterShot") {
			hitPoints = hitPoints - 50;

			if (hitPoints <= 0) {
				animator.enabled = true;
				animator.SetTrigger ("die");

				gameObject.GetComponent <Rigidbody2D> ().isKinematic = true;
				gameObject.GetComponent <PolygonCollider2D> ().enabled = false;
				isFrozen = true;

				Destroy (gameObject, 0.8f);

				GameManager.instance.enemiesInPlay--;
				PlayerController.instance.UpdateScore (20);

				RandomPowerUp ();
			}
		}

		if (other.tag == "Despawn") {
			Destroy (gameObject, 0.0f);
			GameManager.instance.enemiesInPlay--;
		}

		if (other.tag == "Player") {
			animator.enabled = true;
			animator.SetTrigger ("die");

			gameObject.GetComponent <Rigidbody2D> ().isKinematic = true;
			gameObject.GetComponent <PolygonCollider2D> ().enabled = false;
			isFrozen = true;
			Destroy (gameObject, 0.8f);
			GameManager.instance.enemiesInPlay--;
		}
	}

	float SetAttackTimer ()
	{
		float aT = Random.Range (1.0f, 3.0f);
		return aT;
	}

	float SetDirection ()
	{
		float dir = Random.Range (-3.0f, 3.0f);
		return dir;
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
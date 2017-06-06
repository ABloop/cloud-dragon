using System.Collections;
using UnityEngine;

/// <summary>
/// Controls how the Solaris behaves and attacks
/// </summary>
public class SolarisController : MonoBehaviour
{
	// STATS
	private float hitPoints = 8.0f;

	// COMET
	public GameObject orbit;
	public int cometSpeed = 45;

	// FIRE
	public Transform jaw;
	public GameObject fire;
	private float attackTimer;

	// MOVEMENT
	[HideInInspector] public bool isFrozen = false;

	// COMPONENTS
	private Animator animator;
	private Rigidbody2D rb2d;

	void Start ()
	{
		animator = GetComponent <Animator> ();
		rb2d = GetComponentInParent <Rigidbody2D> ();
		attackTimer = SetAttackTimer ();
		cometSpeed = 45;

		// ON HIGHER DIFFICULTY HAS MORE HEALTH, COMETS MOVE FASTER, AND SOLARIS MOVES FASTER
		if (GameManager.instance.difficulty >= 4) {
			hitPoints = 12;
			cometSpeed = 90;
			rb2d.gravityScale = 2.0f;
		}
	}

	void Update ()
	{
		if (!isFrozen) {
			orbit.transform.Rotate (new Vector3 (0, 0, cometSpeed) * Time.deltaTime);
			attackTimer -= Time.deltaTime;

			if (attackTimer <= 0.0f) {
				animator.SetTrigger ("attack");
				attackTimer = SetAttackTimer ();
			}
		}
	}

	void fireBlast ()
	{
		var fb = (GameObject)Instantiate (fire, jaw.position, jaw.rotation);
		fb.SetActive (true);
		Destroy (fb, 2.0f);
	}

	void OnTriggerEnter2D (Collider2D other)
	{
		if (other.tag == "Laser") {
			hitPoints -= PlayerController.instance.DamageToTake ();

			if (hitPoints <= 0) {
				animator.enabled = true;
				animator.SetTrigger ("solarisDie");

				gameObject.GetComponentInParent <Rigidbody2D> ().isKinematic = true;
				gameObject.GetComponent <PolygonCollider2D> ().enabled = false;
				isFrozen = true;

				Destroy (orbit, 0.0f);
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
				animator.SetTrigger ("solarisDie");

				gameObject.GetComponentInParent <Rigidbody2D> ().isKinematic = true;
				gameObject.GetComponent <PolygonCollider2D> ().enabled = false;
				isFrozen = true;

				Destroy (orbit, 0.0f);
				Destroy (gameObject, 0.8f);
				GameManager.instance.enemiesInPlay--;
				PlayerController.instance.UpdateScore (20);
				RandomPowerUp ();
			}
		}

		if (other.tag == "Despawn") {
			Destroy (orbit, 0.0f);
			Destroy (gameObject, 0.0f);
			GameManager.instance.enemiesInPlay--;
		}

		if (other.tag == "Player") {
			animator.enabled = true;
			animator.SetTrigger ("solarisDie");

			gameObject.GetComponentInParent <Rigidbody2D> ().isKinematic = true;
			gameObject.GetComponent <PolygonCollider2D> ().enabled = false;
			isFrozen = true;

			Destroy (orbit, 0.0f);
			Destroy (gameObject, 0.8f);

			GameManager.instance.enemiesInPlay--;
		}
	}

	float SetAttackTimer ()
	{
		float aT = Random.Range (1.0f, 3.0f);
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
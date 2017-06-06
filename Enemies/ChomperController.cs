using System.Collections;
using UnityEngine;

/// <summary>
/// Controls the behaviour of the Chomper enemy
/// </summary>
public class ChomperController : MonoBehaviour
{
	// STATS
	private float hitPoints = 5.0f;

	// COMPONENTS
	private Animator animator;
	private Rigidbody2D rb2d;
	private	PolygonCollider2D pc2d;

	void Start ()
	{
		animator = GetComponent <Animator> ();
		rb2d = GetComponent <Rigidbody2D> ();

		// HEALTH AND SPEED INCREASES BASED ON DIFFICULTY
		if (GameManager.instance.difficulty >= 2) {
			rb2d.gravityScale = 3.5f;
			hitPoints = 6.0f;
		} else if (GameManager.instance.difficulty >= 3) {
			rb2d.gravityScale = 4.5f;
			hitPoints = 7.0f;
		}
	}

	void OnTriggerEnter2D (Collider2D other)
	{
		if (other.tag == "Laser") {
			hitPoints -= PlayerController.instance.DamageToTake ();
			rb2d.gravityScale = rb2d.gravityScale + 0.2f;

			if (hitPoints <= 0.0f) {
				animator.enabled = true;
				animator.SetTrigger ("chomperDie");
		
				rb2d.isKinematic = true;
				gameObject.GetComponent <PolygonCollider2D> ().enabled = false;
				Destroy (gameObject, 0.8f);

				GameManager.instance.enemiesInPlay--;
				PlayerController.instance.UpdateScore (10);
				RandomPowerUp ();
			}
		}

		if (other.tag == "BusterShot") {
			hitPoints = hitPoints - 50.0f;

			if (hitPoints <= 0.0f) {
				animator.enabled = true;
				animator.SetTrigger ("chomperDie");

				rb2d.isKinematic = true;
				gameObject.GetComponent <PolygonCollider2D> ().enabled = false;
				Destroy (gameObject, 0.8f);

				GameManager.instance.enemiesInPlay--;
				PlayerController.instance.UpdateScore (10);
				RandomPowerUp ();
			}
		}

		if (other.tag == "Despawn") {
			Destroy (gameObject, 0f);
			GameManager.instance.enemiesInPlay--;
		}

		if (other.tag == "Player") {
			animator.enabled = true;
			animator.SetTrigger ("chomperDie");

			rb2d.isKinematic = true;
			gameObject.GetComponent <PolygonCollider2D> ().enabled = false;
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
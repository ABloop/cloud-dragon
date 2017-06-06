using System.Collections;
using UnityEngine;

/// <summary>
/// Controls how the Warlock's shield behaves and interacts with taking damage
/// </summary>
public class EnemyShieldController : MonoBehaviour
{
	// STATS
	private float hitPoints = 30.0f;

	// COMPONENTS
	private Animator animator;

	void Start ()
	{
		animator = GetComponent <Animator> ();
	}
		
	void Update ()
	{
		// CHANGES COLOUR BASED ON HITPOINTS
		if (hitPoints < 20.0f && hitPoints > 10.0f) {
			animator.SetTrigger ("phase2");
		}

		if (hitPoints < 10.0f) {
			animator.SetTrigger ("phase3");
		}
	}

	void OnTriggerEnter2D (Collider2D other)
	{
		if (other.tag == "Laser") {
			hitPoints -= PlayerController.instance.DamageToTake ();

			if (hitPoints <= 0.0f) {
				Destroy (gameObject, 0.0f);
				WarlockController.instance.shieldActive = false;
			}
		}

		if (other.tag == "BusterShot") {
			hitPoints = hitPoints - 50.0f;

			if (hitPoints <= 0) {
				Destroy (gameObject, 0.0f);
				WarlockController.instance.shieldActive = false;
			}
		}

		if (other.tag == "Player") {
			hitPoints = hitPoints - 10.0f;

			if (hitPoints <= 0) {
				Destroy (gameObject, 0.0f);
				WarlockController.instance.shieldActive = false;
			}
		}
	}
}
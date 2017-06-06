using System.Collections;
using UnityEngine;

/// <summary>
/// Controls how the Warlock's Ender Blasts behave when instantiated
/// </summary>
public class EnderBlastController : MonoBehaviour
{
	// DIRECTION AND PATTERN
	[HideInInspector] public float directionX = 0;
	[HideInInspector] public float directionY = 0;
	[HideInInspector] public string attackPattern;

	// COMPONENTS
	private Animator animator;
	private Rigidbody2D rb2d;
	private CircleCollider2D cc2d;

	void Start ()
	{
		animator = GetComponent <Animator> ();
		rb2d = GetComponent <Rigidbody2D> ();
		cc2d = GetComponent <CircleCollider2D> ();
	}

	void Update ()
	{
		// MOVE IN A DIRECTION BASED ON PROVIDED NUMBER FROM WARLOCK CONTROLLER
		if (attackPattern == "Diagonal Left") {
			rb2d.MovePosition (rb2d.position + new Vector2 (directionX, (-10 - directionX)) * Time.fixedDeltaTime);
		} else if (attackPattern == "Diagonal Right") {
			rb2d.MovePosition (rb2d.position + new Vector2 (directionX, (-10 + directionX)) * Time.fixedDeltaTime);
		} else {
			rb2d.MovePosition (rb2d.position + new Vector2 (directionX, directionY) * Time.fixedDeltaTime);
		}
	}

	void OnTriggerEnter2D (Collider2D other)
	{
		if (other.tag == "Player") {
			animator.SetTrigger ("hit");

			if (rb2d != null) {
				rb2d.isKinematic = true;
			}

			cc2d.enabled = false;
			Destroy (gameObject, 0.3f);
		}
	}
}
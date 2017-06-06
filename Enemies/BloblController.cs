using System.Collections;
using UnityEngine;

/// <summary>
/// Controls how the Quazzarian's Blobl attack behaves when instantiated
/// </summary>
public class BloblController : MonoBehaviour
{
	// DIRECTION
	[HideInInspector] public int direction = 0;

	// COMPONENTS
	private Animator animator;
	private Rigidbody2D rb2d;
	private BoxCollider2D bc2d;

	void Start ()
	{
		animator = GetComponent <Animator> ();
		rb2d = GetComponent <Rigidbody2D> ();
		bc2d = GetComponent <BoxCollider2D> ();
	}

	void Update ()
	{
		// MOVES IN A DIRECTION BASED ON NUMBER SET IN QUAZZARIAN CONTROLLER
		rb2d.MovePosition (rb2d.position + new Vector2 (direction, -10) * Time.fixedDeltaTime);
	}

	void OnTriggerEnter2D (Collider2D other)
	{
		if (other.tag == "Player") {
			animator.SetTrigger ("hit");

			if (rb2d != null) {
				rb2d.isKinematic = true;
			}

			bc2d.enabled = false;
			Destroy (gameObject, 0.3f);
		}
	}
}
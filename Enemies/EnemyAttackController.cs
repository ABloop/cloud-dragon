using System.Collections;
using UnityEngine;

/// <summary>
/// Controls how basic enemy attacks behave (Solaris' fire blasts, Martian's blasties)
/// </summary>
public class EnemyAttackController : MonoBehaviour
{
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

	void OnTriggerEnter2D (Collider2D other)
	{
		if (other.tag == "Player") {
			animator.SetTrigger ("hit");

			if (rb2d != null) {
				rb2d.isKinematic = true;
			}

			cc2d.enabled = false;
			Destroy (gameObject, 0.6f);
		}
	}
}
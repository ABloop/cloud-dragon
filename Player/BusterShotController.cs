using System.Collections;
using UnityEngine;

/// <summary>
/// Controls Chorg's BusterShot Special
/// </summary>
public class BusterShotController : MonoBehaviour
{
	// COLLISIONS
	private float colliderActivate = 0.25f;
	private bool startExpanding = false;

	// COMPONENTS
	private Rigidbody2D rb2d;
	private CircleCollider2D cc2d;
	private Animator anim;

	void Start ()
	{
		rb2d = GetComponent <Rigidbody2D> ();
		cc2d = GetComponent <CircleCollider2D> ();
		anim = GetComponent <Animator> ();

		cc2d.enabled = false;
	}

	void Update ()
	{
		colliderActivate -= Time.deltaTime;

		if (colliderActivate <= 0) {
			cc2d.enabled = true;
			startExpanding = true;
		}

		if (startExpanding == true) {
			transform.localScale += (new Vector3 (0.5f, 0.5f, 0) * Time.deltaTime);
		}

		transform.Rotate (new Vector3 (0, 0, 45) * Time.deltaTime);
	}

	void OnTriggerEnter2D (Collider2D other)
	{
		if (other.tag == "Boss") {
			PlayerController.instance.ClearChorgSpecial ();
			cc2d.enabled = false;
			rb2d.isKinematic = true;

			anim.SetTrigger ("boom");
			Destroy (gameObject, 0.8f);
		}

		if (other.tag == "Attack Despawn") {
			PlayerController.instance.ClearChorgSpecial ();
			Destroy (gameObject, 3.0f);
		}
	}
}
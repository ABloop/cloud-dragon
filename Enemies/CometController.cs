using System.Collections;
using UnityEngine;

/// <summary>
/// Controls how the Solaris' comets behave
/// </summary>
public class CometController : MonoBehaviour
{
	// COMET
	public Transform cometHead;
	public GameObject cometDust;

	// COMPONENTS
	private Rigidbody2D rb2d;
	private CircleCollider2D cc2d;

	void Start ()
	{
		rb2d = GetComponent <Rigidbody2D> ();
		cc2d = GetComponent <CircleCollider2D> ();
	}

	void OnTriggerEnter2D (Collider2D other)
	{
		if (other.tag == "Player") {
			
			if (rb2d != null) {
				rb2d.isKinematic = true;
			}

			cc2d.enabled = false;
			var cometHit = (GameObject)Instantiate (cometDust, cometHead.position, cometHead.rotation);

			Destroy (gameObject, 0.0f);
			Destroy (cometHit, 0.6f);
		}
	}
}
using System.Collections;
using UnityEngine;

/// <summary>
/// Controls the behaviour of a Power Up
/// </summary>
public class PowerUpController : MonoBehaviour
{
	// STATS
	private int speed;
	[HideInInspector] public bool buffApplied = false;

	// MOVEMENT
	private int direction;

	[HideInInspector] public bool isFrozen = false;

	// COMPONENTS
	private Rigidbody2D rb2d;

	void Start ()
	{
		rb2d = GetComponent <Rigidbody2D> ();
		speed = SetRandomSpeed ();
		direction = 0;
	}

	void Update ()
	{
		if (!isFrozen) {
			transform.Rotate (new Vector3 (0, 0, 90) * Time.deltaTime);
			rb2d.MovePosition (rb2d.position + new Vector2 (direction, -speed) * Time.fixedDeltaTime);
		}
	}

	void OnTriggerEnter2D (Collider2D other)
	{
		if (other.tag == "Player") {
			buffApplied = true;
			Destroy (gameObject, 0.1f);
		} else if (other.tag == "Despawn") {
			Destroy (gameObject, 0.0f);
		}
	}

	int SetRandomSpeed ()
	{
		int spd = Random.Range (5, 11);
		return spd;
	}

	int SetRandomDirection ()
	{
		int spd = Random.Range (-1, 2);
		return spd;
	}
}
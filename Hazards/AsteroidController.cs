using System.Collections;
using UnityEngine;

/// <summary>
/// Controls the behaviour of an Asteroid
/// </summary>
public class AsteroidController : MonoBehaviour
{
	// STATS
	public float hitPoints = 5;
	public GameObject rubble;

	// MOVEMENT
	private bool upperLimitReached = false;
	private bool lowerLimitReached = true;

	private int limit;
	private float upperLimit;
	private float lowerLimit;

	private int direction;

	[HideInInspector] public bool isFrozen = false;

	// COMPONENT
	private Rigidbody2D rb2d;

	void Start ()
	{
		rb2d = GetComponent<Rigidbody2D> ();
		limit = SetRandomRange ();
		direction = SetDirection ();
		upperLimit = transform.position.x + limit;
		lowerLimit = transform.position.x - limit;
	}

	void Update ()
	{
		if (!isFrozen) {
			transform.Rotate (new Vector3 (0, 0, 45) * Time.deltaTime);
		
			if (lowerLimitReached == true && upperLimitReached == false) {
				rb2d.MovePosition (rb2d.position + new Vector2 (direction, -5) * Time.fixedDeltaTime);

				if (gameObject.transform.position.y >= upperLimit) {
					upperLimitReached = true;
					lowerLimitReached = false;
				}
			} else if (lowerLimitReached == false && upperLimitReached == true) {
				rb2d.MovePosition (rb2d.position + new Vector2 (-direction, -5) * Time.fixedDeltaTime);

				if (gameObject.transform.position.y <= lowerLimit) {
					lowerLimitReached = true;
					upperLimitReached = false;
				}
			}
		}
	}

	void OnTriggerEnter2D (Collider2D other)
	{
		if (other.tag == "Laser") {
			hitPoints -= PlayerController.instance.DamageToTake ();

			if (hitPoints <= 0) {
				var roidRubble = (GameObject)Instantiate (rubble, transform.position, transform.rotation);
				roidRubble.SetActive (true);
				
				Destroy (gameObject, 0.0f);
				Destroy (roidRubble, 1.0f);
				PlayerController.instance.UpdateScore (5);
				RandomPowerUp ();
			}
		}

		if (other.tag == "BusterShot") {
			hitPoints = hitPoints - 50;

			if (hitPoints <= 0) {
				var roidRubble = (GameObject)Instantiate (rubble, transform.position, transform.rotation);
				roidRubble.SetActive (true);

				Destroy (gameObject, 0.0f);
				Destroy (roidRubble, 1.0f);
				PlayerController.instance.UpdateScore (5);
				RandomPowerUp ();
			}
		}

		if (other.tag == "Despawn") {
			Destroy (gameObject, 0f);
			GameManager.instance.hazardsInPlay--;
		}

		if (other.tag == "Player" || other.tag == "Enemy") {
			var roidRubble = (GameObject)Instantiate (rubble, transform.position, transform.rotation);
			roidRubble.SetActive (true);

			Destroy (gameObject, 0.0f);
			Destroy (roidRubble, 1.0f);
		}
	}

	int SetRandomRange ()
	{
		int range = Random.Range (1, 4);
		return range;
	}

	int SetDirection ()
	{
		int dir = Random.Range (1, 3);
		return dir;
	}

	void RandomPowerUp ()
	{
		int willItSpawn = Random.Range (1, 21);
		bool itSpawns = false;

		if (willItSpawn == 16) {
			itSpawns = true;
		}

		if (itSpawns) {
			GameManager.instance.powerUpManager.EnemyDropPowerUp (GameManager.instance.powerUps, transform);
		}
	}
}
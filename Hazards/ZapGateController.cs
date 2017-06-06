using System.Collections;
using UnityEngine;

/// <summary>
/// Controls the behaviour of a Zap Gate
/// </summary>
public class ZapGateController : MonoBehaviour
{
	// STATS
	private float speed = 4.0f;

	// GAME OBJECTS
	public GameObject leftGate;
	public GameObject rightGate;
	public GameObject zapBarrier;
	public GameObject lgInactive;
	public GameObject rgInactive;

	// DEACTIVATION TIMERS
	private float activeTimer;
	private float inactiveTimer = 3.0f;
	private bool deactivated = false;

	// MOVEMENT
	[HideInInspector] public bool isFrozen = false;

	void Start ()
	{
		activeTimer = 0.0f + SetActiveTimer ();
	}

	void Update ()
	{
		if (!isFrozen) {
			transform.Translate (Vector3.down * speed * Time.deltaTime);
			
			if (!deactivated) {
				activeTimer -= Time.deltaTime;
			} 
			
			if (deactivated) {
				inactiveTimer -= Time.deltaTime;
			}
			
			if (activeTimer <= 0.0f && deactivated == false) {
				deactivated = true;
				DeactivateGates ();
				inactiveTimer = 3.0f;
			}
			
			if (inactiveTimer <= 0.0f && deactivated == true) {
				deactivated = false;
				ActivateGates ();
				activeTimer = 0.0f + SetActiveTimer ();
			}
		}
	}

	int SetActiveTimer ()
	{
		int newTimer = Random.Range (1, 4);
		return newTimer;
	}

	void ActivateGates ()
	{
		leftGate.SetActive (true);
		rightGate.SetActive (true);
		lgInactive.SetActive (false);
		rgInactive.SetActive (false);

		zapBarrier.SetActive (true);
	}

	void DeactivateGates ()
	{
		leftGate.SetActive (false);
		rightGate.SetActive (false);
		lgInactive.SetActive (true);
		rgInactive.SetActive (true);

		zapBarrier.SetActive (false);
	}

	void OnTriggerEnter2D (Collider2D other)
	{
		if (other.tag == "Despawn") {
			Destroy (gameObject, 0f);
			GameManager.instance.hazardsInPlay--;
		}
	}
}
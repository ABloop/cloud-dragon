﻿using System.Collections;
using UnityEngine;

/// <summary>
/// Controls the Laser Attack from Icarai
/// </summary>
public class LaserController : MonoBehaviour
{
	// COMPONENTS
	private Rigidbody2D rb2d;
	private Animator animator;

	void Start ()
	{
		rb2d = GetComponent <Rigidbody2D> ();
		animator = GetComponent <Animator> ();
	}

	void OnTriggerEnter2D (Collider2D other)
	{
		if (other.tag == "Enemy" || other.tag == "Hazard" || other.tag == "Enemy Shield" || other.tag == "Boss") {
			animator.SetTrigger ("hit");
			rb2d.isKinematic = true;
			Destroy (gameObject, 0.5f);
		}

		if (other.tag == "Attack Despawn") {
			Destroy (gameObject, 0f);
		}
	}
}
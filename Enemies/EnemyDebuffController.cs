using System.Collections;
using UnityEngine;

/// <summary>
/// Controls how the Warlock's Darkness Debuff behaves
/// </summary>
public class EnemyDebuffController : MonoBehaviour
{
	// DETERMINES IF DEBUFF HAS BEEN APPLIED (CAN ONLY BE APPLIED ONCE)
	private bool debuffApplied = false;

	void OnTriggerEnter2D (Collider2D other)
	{
		if (other.tag == "Player") {

			if (debuffApplied == false) {
				PlayerController.instance.ApplyDebuff ();
				debuffApplied = true;
			}
		}
	}
}
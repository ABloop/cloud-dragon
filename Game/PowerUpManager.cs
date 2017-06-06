using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Manages how Power Ups are generated throughout the game
/// </summary>
public class PowerUpManager : MonoBehaviour
{
	// GAMEOBJECTS
	public GameObject healthPowerUp;
	public GameObject ammoPowerUp;
	public GameObject speedPowerUp;
	public GameObject damagePowerUp;
	public GameObject specialPowerUp;

	// SPAWNS
	public ArrayList spawnLocations = new ArrayList ();
	public GameObject[] powerUpSpawns;

	// POWER UPS
	private String powerUpSelected;
	[HideInInspector] public int powerUpsInPlay;
	private int powerUpsCount;

	public void PowerUpsSetup (ArrayList powerUps)
	{
		FillSpawns ();
		powerUpsInPlay = 0;

		for (int i = 0; i < powerUpsCount; i++) {
			SelectPowerUpType ();

			if (powerUpSelected == "Health") {
				bool locationFound = false;
				int randomPos = Random.Range (0, spawnLocations.Count);

				while (locationFound != true) {
					GameObject spawnLoc = (GameObject)spawnLocations [randomPos];
					Vector3 position = new Vector3 (spawnLoc.transform.position.x, spawnLoc.transform.position.y);
					locationFound = true;

					var hp = (GameObject)Instantiate (healthPowerUp, position, Quaternion.identity);
					powerUps.Add (hp);
					spawnLocations.RemoveAt (randomPos);
				}
			} else if (powerUpSelected == "Ammo") {
				bool locationFound = false;
				int randomPos = Random.Range (0, spawnLocations.Count);

				while (locationFound != true) {
					GameObject spawnLoc = (GameObject)spawnLocations [randomPos];
					Vector3 position = new Vector3 (spawnLoc.transform.position.x, spawnLoc.transform.position.y);
					locationFound = true;

					var am = (GameObject)Instantiate (ammoPowerUp, position, Quaternion.identity);
					powerUps.Add (am);
					spawnLocations.RemoveAt (randomPos);
				}
			} else if (powerUpSelected == "Speed") {
				bool locationFound = false;
				int randomPos = Random.Range (0, spawnLocations.Count);

				while (locationFound != true) {
					GameObject spawnLoc = (GameObject)spawnLocations [randomPos];
					Vector3 position = new Vector3 (spawnLoc.transform.position.x, spawnLoc.transform.position.y);
					locationFound = true;

					var sp = (GameObject)Instantiate (speedPowerUp, position, Quaternion.identity);
					powerUps.Add (sp);
					spawnLocations.RemoveAt (randomPos);
				}
			} else if (powerUpSelected == "Damage") {
				bool locationFound = false;
				int randomPos = Random.Range (0, spawnLocations.Count);

				while (locationFound != true) {
					GameObject spawnLoc = (GameObject)spawnLocations [randomPos];
					Vector3 position = new Vector3 (spawnLoc.transform.position.x, spawnLoc.transform.position.y);
					locationFound = true;

					var dm = (GameObject)Instantiate (damagePowerUp, position, Quaternion.identity);
					powerUps.Add (dm);
					spawnLocations.RemoveAt (randomPos);
				}
			} else if (powerUpSelected == "Special") {
				bool locationFound = false;
				int randomPos = Random.Range (0, spawnLocations.Count);

				while (locationFound != true) {
					GameObject spawnLoc = (GameObject)spawnLocations [randomPos];
					Vector3 position = new Vector3 (spawnLoc.transform.position.x, spawnLoc.transform.position.y);
					locationFound = true;

					var sc = (GameObject)Instantiate (specialPowerUp, position, Quaternion.identity);
					powerUps.Add (sc);
					spawnLocations.RemoveAt (randomPos);
				}
			}

			powerUpsInPlay++;
		}
	}

	public void EnemyDropPowerUp (ArrayList powerUps, Transform location)
	{
		SelectPowerUpType ();

		if (powerUpSelected == "Health") {
			Vector3 position = new Vector3 (location.position.x, location.position.y);
			var hp = (GameObject)Instantiate (healthPowerUp, position, Quaternion.identity);
			powerUps.Add (hp);
		} else if (powerUpSelected == "Ammo") {
			Vector3 position = new Vector3 (location.position.x, location.position.y);
			var am = (GameObject)Instantiate (ammoPowerUp, position, Quaternion.identity);
			powerUps.Add (am);
		} else if (powerUpSelected == "Speed") {
			Vector3 position = new Vector3 (location.position.x, location.position.y);
			var sp = (GameObject)Instantiate (speedPowerUp, position, Quaternion.identity);
			powerUps.Add (sp);
		} else if (powerUpSelected == "Damage") {
			Vector3 position = new Vector3 (location.position.x, location.position.y);
			var dm = (GameObject)Instantiate (damagePowerUp, position, Quaternion.identity);
			powerUps.Add (dm);
		} else if (powerUpSelected == "Special") {
			Vector3 position = new Vector3 (location.position.x, location.position.y);
			var sc = (GameObject)Instantiate (specialPowerUp, position, Quaternion.identity);
			powerUps.Add (sc);
		}

		powerUpsInPlay++;
	}

	void SelectPowerUpType ()
	{
		int powerUpSelection = Random.Range (1, 6);

		switch (powerUpSelection) {
		case 1:
			powerUpSelected = "Health";
			break;
		case 2:
			powerUpSelected = "Ammo";
			break;
		case 3:
			powerUpSelected = "Speed";
			break;
		case 4:
			powerUpSelected = "Damage";
			break;
		case 5:
			powerUpSelected = "Special";
			break;
		}
	}

	void FillSpawns ()
	{
		spawnLocations.Clear ();

		for (int i = 0; i < powerUpSpawns.Length; i++) {
			spawnLocations.Add (powerUpSpawns [i]);
		}

		powerUpsCount = Random.Range (2, 6);
	}
}
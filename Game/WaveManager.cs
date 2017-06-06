using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class WaveManager : MonoBehaviour
{
	// GAME OBJECTS
	public GameObject chomper;
	public GameObject powerFist;
	public GameObject solaris;
	public GameObject quazzarian;
	public GameObject martian;
	public GameObject warlock;

	// SPAWNS
	public ArrayList spawnLocations = new ArrayList ();

	public GameObject[] chomperSpawns;
	public GameObject[] powerFistSpawns;
	public GameObject[] solarisSpawns;
	public GameObject[] quazzarianSpawns;
	public GameObject[] martianSpawns;
	public GameObject warlockSpawn;

	// ENEMIES
	private String enemySelected;
	[HideInInspector] public int enemiesInPlay;
	private int enemiesCount;

	public void WaveSetup (ArrayList waveEnemies)
	{
		SelectEnemyType ();
		FillSpawns ();
		enemiesInPlay = 0;

		for (int i = 0; i < enemiesCount; i++) {
			
			if (enemySelected == "Chomper") {
				bool locationFound = false;
				int randomPos = Random.Range (0, spawnLocations.Count);

				while (locationFound != true) {
					GameObject spawnLoc = (GameObject)spawnLocations [randomPos];
					Vector3 position = new Vector3 (spawnLoc.transform.position.x, spawnLoc.transform.position.y);
					locationFound = true;

					var ch = (GameObject)Instantiate (chomper, position, Quaternion.identity);
					waveEnemies.Add (ch);
					spawnLocations.RemoveAt (randomPos);
				}
			} else if (enemySelected == "Power Fist") {
				bool locationFound = false;
				int randomPos = Random.Range (0, spawnLocations.Count);

				while (locationFound != true) {
					GameObject spawnLoc = (GameObject)spawnLocations [randomPos];
					Vector3 position = new Vector3 (spawnLoc.transform.position.x, spawnLoc.transform.position.y);
					locationFound = true;

					var pf = (GameObject)Instantiate (powerFist, position, Quaternion.identity);
					waveEnemies.Add (pf);
					spawnLocations.RemoveAt (randomPos);
				}
			} else if (enemySelected == "Solaris") {
				bool locationFound = false;
				int randomPos = Random.Range (0, spawnLocations.Count);

				while (locationFound != true) {
					GameObject spawnLoc = (GameObject)spawnLocations [randomPos];
					Vector3 position = new Vector3 (spawnLoc.transform.position.x, spawnLoc.transform.position.y);
					locationFound = true;

					var sl = (GameObject)Instantiate (solaris, position, Quaternion.identity);
					waveEnemies.Add (sl);
					spawnLocations.RemoveAt (randomPos);
				}
			} else if (enemySelected == "Quazzarian") {
				bool locationFound = false;
				int randomPos = Random.Range (0, spawnLocations.Count);

				while (locationFound != true) {
					GameObject spawnLoc = (GameObject)spawnLocations [randomPos];
					Vector3 position = new Vector3 (spawnLoc.transform.position.x, spawnLoc.transform.position.y);
					locationFound = true;

					var qu = (GameObject)Instantiate (quazzarian, position, Quaternion.identity);
					waveEnemies.Add (qu);
					spawnLocations.RemoveAt (randomPos);
				}
			} else if (enemySelected == "Martian") {
				bool locationFound = false;
				int randomPos = Random.Range (0, spawnLocations.Count);

				while (locationFound != true) {
					GameObject spawnLoc = (GameObject)spawnLocations [randomPos];
					Vector3 position = new Vector3 (spawnLoc.transform.position.x, spawnLoc.transform.position.y);
					locationFound = true;

					var mt = (GameObject)Instantiate (martian, position, Quaternion.identity);
					waveEnemies.Add (mt);
					spawnLocations.RemoveAt (randomPos);
				}
			} else if (enemySelected == "Warlock") {
				bool locationFound = false;
				int randomPos = Random.Range (0, spawnLocations.Count);

				while (locationFound != true) {
					GameObject spawnLoc = (GameObject)spawnLocations [randomPos];
					Vector3 position = new Vector3 (spawnLoc.transform.position.x, spawnLoc.transform.position.y);
					locationFound = true;

					var wa = (GameObject)Instantiate (warlock, position, Quaternion.identity);
					waveEnemies.Add (wa);
					spawnLocations.RemoveAt (randomPos);
					GameManager.instance.bossInPlay = true;
					PlayerController.instance.BossApproaching ();
				}
			}

			enemiesInPlay++;
		}
	}

	public void WarlockWave (ArrayList waveEnemies)
	{
		int enemySelection = 0;
		enemySelection = Random.Range (1, 3); // CHOMPER AND POWER FIST ONLY

		switch (enemySelection) {
		case 1:
			enemySelected = "Chomper";
			enemiesCount = Random.Range (4, 9);
			break;
		case 2:
			enemySelected = "Power Fist";
			enemiesCount = Random.Range (2, 6);
			break;
		}

		FillSpawns ();

		for (int i = 0; i < enemiesCount; i++) {
			
			if (enemySelected == "Chomper") {
				bool locationFound = false;
				int randomPos = Random.Range (0, spawnLocations.Count);

				while (locationFound != true) {
					GameObject spawnLoc = (GameObject)spawnLocations [randomPos];
					Vector3 position = new Vector3 (spawnLoc.transform.position.x, spawnLoc.transform.position.y);
					locationFound = true;
				
					var ch = (GameObject)Instantiate (chomper, position, Quaternion.identity);
					waveEnemies.Add (ch);
					spawnLocations.RemoveAt (randomPos);
				}

			} else if (enemySelected == "Power Fist") {
				bool locationFound = false;
				int randomPos = Random.Range (0, spawnLocations.Count);

				while (locationFound != true) {
					GameObject spawnLoc = (GameObject)spawnLocations [randomPos];
					Vector3 position = new Vector3 (spawnLoc.transform.position.x, spawnLoc.transform.position.y);
					locationFound = true;

					var pf = (GameObject)Instantiate (powerFist, position, Quaternion.identity);
					waveEnemies.Add (pf);
					spawnLocations.RemoveAt (randomPos);
				}
			}

			enemiesInPlay++;
		}
	}

	void SelectEnemyType ()
	{
		int enemySelection = 0;

		if (GameManager.instance.difficulty == 1) {
			enemySelection = Random.Range (1, 3); // CHOMPER AND POWER FIST
		} else if (GameManager.instance.difficulty == 2) {
			enemySelection = Random.Range (1, 4); // CHOMPER, POWER FIST AND SOLARIS
		} else if (GameManager.instance.difficulty == 3) {
			enemySelection = Random.Range (1, 5); // CHOMPER, POWER FIST, SOLARIS AND QUAZZARIAN
		} else if (GameManager.instance.difficulty == 4) {
			enemySelection = Random.Range (2, 6); // POWER FIST, SOLARIS, AND QUAZZARIAN
		} else if (GameManager.instance.difficulty == 5) {
			enemySelection = Random.Range (3, 6); // SOLARIS, QUAZZARIAN, AND MARTIAN
		} else if (GameManager.instance.difficulty == 6) {
			enemySelection = 6; // WARLOCK
		}

		switch (enemySelection) {
		case 1:
			enemySelected = "Chomper";
			enemiesCount = Random.Range (4, 17);
			break;
		case 2:
			enemySelected = "Power Fist";
			enemiesCount = Random.Range (2, 11);
			break;
		case 3:
			enemySelected = "Solaris";
			enemiesCount = Random.Range (2, 7);
			break;
		case 4:
			enemySelected = "Quazzarian";
			enemiesCount = Random.Range (2, 6);
			break;
		case 5:
			enemySelected = "Martian";
			enemiesCount = Random.Range (1, 5);
			break;
		case 6:
			enemySelected = "Warlock";
			enemiesCount = 1;
			break;
		}
	}

	void FillSpawns ()
	{
		spawnLocations.Clear ();

		if (enemySelected == "Chomper") {
			
			for (int i = 0; i < chomperSpawns.Length; i++) {
				spawnLocations.Add (chomperSpawns [i]);
			}

		} else if (enemySelected == "Power Fist") {

			for (int i = 0; i < powerFistSpawns.Length; i++) {
				spawnLocations.Add (powerFistSpawns [i]);

			}

		} else if (enemySelected == "Solaris") {
			
			for (int i = 0; i < solarisSpawns.Length; i++) {
				spawnLocations.Add (solarisSpawns [i]);
			}

		} else if (enemySelected == "Quazzarian") {

			for (int i = 0; i < quazzarianSpawns.Length; i++) {
				spawnLocations.Add (quazzarianSpawns [i]);
			}

		} else if (enemySelected == "Martian") {

			for (int i = 0; i < martianSpawns.Length; i++) {
				spawnLocations.Add (martianSpawns [i]);
			}

		} else if (enemySelected == "Warlock") {

			spawnLocations.Add (warlockSpawn);

		}
	}
}
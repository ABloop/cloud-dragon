using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Manages how Hazards are generated within the game
/// </summary>
public class HazardManager : MonoBehaviour
{
	// GAME OBJECTS
	public GameObject[] asteroids;
	public GameObject zapGate;

	// SPAWNS
	public ArrayList spawnLocations = new ArrayList ();

	public GameObject[] asteroidSpawns;
	public GameObject[] zapGateSpawns;

	// HAZARDS
	private String hazardSelected;
	[HideInInspector] public int hazardsInPlay;
	private int hazardsCount;

	public void HazardsSetup (ArrayList hazards)
	{
		SelectHazardType ();
		FillSpawns ();
		hazardsInPlay = 0;

		for (int i = 0; i < hazardsCount; i++) {
			
			if (hazardSelected == "Asteroids") {
				bool locationFound = false;
				int randomPos = Random.Range (0, spawnLocations.Count);

				while (locationFound != true) {
					GameObject spawnLoc = (GameObject)spawnLocations [randomPos];
					Vector3 position = new Vector3 (spawnLoc.transform.position.x, spawnLoc.transform.position.y);
					locationFound = true;

					int RandomAsteroid = Random.Range (0, 6);
					var ast = (GameObject)Instantiate (asteroids [RandomAsteroid], position, Quaternion.identity);
					hazards.Add (ast);
					spawnLocations.RemoveAt (randomPos);
				}
			} else if (hazardSelected == "Zap Gates") {

				bool locationFound = false;
				int randomPos = Random.Range (0, spawnLocations.Count);

				while (locationFound != true) {
					GameObject spawnLoc = (GameObject)spawnLocations [randomPos];
					Vector3 position = new Vector3 (spawnLoc.transform.position.x, spawnLoc.transform.position.y);
					locationFound = true;

					var zg = (GameObject)Instantiate (zapGate, position, Quaternion.identity);
					hazards.Add (zg);
					spawnLocations.RemoveAt (randomPos);
				}
			}
		
			hazardsInPlay++;
		}
	}

	void SelectHazardType ()
	{
		int hazardSelection = 0;

		if (GameManager.instance.difficulty == 1) {
			hazardSelection = Random.Range (1, 2);
		} else if (GameManager.instance.difficulty >= 4) {
			hazardSelection = Random.Range (1, 3);
		}

		switch (hazardSelection) {
		case 1:
			hazardSelected = "Asteroids";
			hazardsCount = Random.Range (4, 19);
			break;
		case 2:
			hazardSelected = "Zap Gates";
			hazardsCount = Random.Range (3, 7);
			break;
		}
	}

	void FillSpawns ()
	{
		spawnLocations.Clear ();

		if (hazardSelected == "Asteroids") {
			
			for (int i = 0; i < asteroidSpawns.Length; i++) {
				spawnLocations.Add (asteroidSpawns [i]);
			}

		} else if (hazardSelected == "Zap Gates") {
			
			for (int i = 0; i < zapGateSpawns.Length; i++) {
				spawnLocations.Add (zapGateSpawns [i]);
			}
		}
	}
}
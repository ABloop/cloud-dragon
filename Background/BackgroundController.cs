/*
Scrolling Background adapted from Buckner (2014).
*/

using System.Collections;
using UnityEngine;

/// <summary>
/// Controls how the background constantly scrolls behind the player or UI
/// </summary>
public class BackgroundController : MonoBehaviour
{
	// TILE SPEED, SIZE AND POSITION
	public float scrollSpeed;
	public float tileSizeY;
	private Vector3 startPosition;

	void Start ()
	{
		startPosition = transform.position;
	}

	void Update ()
	{
		// REPEATEDLY MOVE FORWARDS UNTIL REACHES FULL HEIGHT OF BACKGROUND TILE
		float newPosition = Mathf.Repeat (Time.time * scrollSpeed, tileSizeY);
		transform.position = startPosition + Vector3.down * newPosition;
	}
}
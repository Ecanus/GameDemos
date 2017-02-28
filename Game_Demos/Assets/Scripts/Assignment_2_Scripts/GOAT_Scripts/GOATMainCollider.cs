using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Collider attached to each GOAT prefab to check for if the entire object is out of bounds
/// </summary>
public class GOATMainCollider : MonoBehaviour {


	/// <summary>
	/// Offset between the actual GOAT object, and one of its particles that is moved via
	/// Verlet Integration
	/// </summary>
	private Vector3 offset;

	/// <summary>
	/// The Transform used to calculate the offset for this instance's positioning
	/// </summary>
	[SerializeField]
	private Transform target;

	/// <summary>
	/// The target position to place this instance
	/// </summary>
	private Vector3 targetPos;

	/// <summary>
	/// Check for if this GOAT instance is too far right, left or down
	/// </summary>
	private void checkOutOfBounds()
	{
		// Get the position of this instance in terms of the screen
		Vector2 screenPosition = Camera.main.WorldToScreenPoint (transform.position);



		bool tooFarRight = (screenPosition.x >= Camera.main.pixelWidth + 5f);
		bool tooFarLeft = (screenPosition.x < -5f);
		bool tooLow = (screenPosition.y < (0.20f * Camera.main.pixelHeight));


		// Check if it is either too far right, left or down. If so, destroy it
		if (tooFarRight || tooFarLeft || tooLow) 
		{
			Destroy (transform.parent.gameObject);
		}

	}
		

	// Use this for initialization
	void Start () {

		// Set the offset based on this instance's position and the position of the target
		offset = transform.position - target.position;
	}

	void FixedUpdate()
	{
		// Update the position of this instance based on its target position
		targetPos = target.position + offset;
		transform.position = targetPos;

		// Check for if this instance is out of bounds or not
		checkOutOfBounds();

	}

}

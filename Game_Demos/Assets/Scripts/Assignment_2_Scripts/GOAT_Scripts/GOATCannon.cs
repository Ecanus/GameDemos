using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Cannon which fires out a GOAT object
/// </summary>
public class GOATCannon : MonoBehaviour {

	/// <summary>
	/// Icon showing which cannon is selected
	/// </summary>
	[SerializeField]
	private GameObject a_SelectedIcon;

	/// <summary>
	/// The spawn location of the cannonball
	/// </summary>
	[SerializeField]
	private Transform a_SpawnLocation;

	/// <summary>
	/// The GOAT prefab used for instantiation
	/// </summary>
	[SerializeField]
	private Transform a_GOAT;


	// FIRING VECTOR Atrributes
	/// <summary>
	/// PointA used in determining the firing vector of the cannon
	/// </summary>
	[SerializeField]
	private Transform a_FiringPointA;
	/// <summary>
	/// PointB used in determining the firing vector of the cannon
	/// </summary>
	[SerializeField]
	private Transform a_FiringPointB;

	/// <summary>
	/// The firing vector that all cannons access on instantiation
	/// </summary>
	public static Vector2 FiringVector;

	/// <summary>
	/// The coefficient of direction, changed based on the currently selected cannon
	/// </summary>
	public static float Direction;

	/// <summary>
	/// The firing force by which FiringVector is scaled
	/// </summary>
	private float a_FiringForce;

	/// <summary>
	/// Instantiates a GOAT object to be fired outwards
	/// </summary>
	public void fire()
	{
		Instantiate (a_GOAT, a_SpawnLocation.position, Quaternion.identity);
	}

	/// <summary>
	/// Updates the firing angle of the GOAT cannon. Called after every switch of selected cannon
	/// </summary>
	public void updateFiringAngle()
	{
		// Instantiate a vector of the same position as the FiringPoint B
		Vector2 newPosition = a_FiringPointB.position;

		// Apply a new height to the FiringPoint B, effectively creating a new firing angle
		// The range of distances is from 0.5 to 2.5, using local coords
		float newPositionY = Random.Range (0.5f, 2.5f);
		newPosition.y = newPositionY;

		// Update the FiringPointB position to the newPosition
		a_FiringPointB.position = newPosition;

		// Set the firingVector equal to this updated vector. Scale it by the firing force
		FiringVector = (a_FiringPointB.position - a_FiringPointA.position).normalized;
		FiringVector *= a_FiringForce;


	}

	/// <summary>
	/// Activates the selectedIcon when called
	/// and updates the Firing Angle as well
	/// </summary>
	public void selected()
	{
		a_SelectedIcon.SetActive (true);
		updateFiringAngle();
	}

	/// <summary>
	/// Deactivates the deselectedIcon when called
	/// </summary>
	public void deselected()
	{
		a_SelectedIcon.SetActive (false);
	}


	// Use this for initialization
	void Awake () {

		// Instantiate a firing angle
		a_FiringForce = 0.5f;

		// Instantiate a firing vector and scale it by the firing force
		FiringVector = (a_FiringPointB.position - a_FiringPointA.position).normalized;
		FiringVector *= a_FiringForce;

	}
}

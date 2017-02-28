using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class managing all behaviour for cannonball cannons in game
/// </summary>
public class Cannon : MonoBehaviour {


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
	/// The cannonball prefab used for instantiation
	/// </summary>
	[SerializeField]
	private Transform a_Cannonball;


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
	/// The firing force by which FiringVector is scaled
	/// </summary>
	private float a_FiringForce;

	/// <summary>
	/// Instantiate a cannonball and then fire into the world
	/// </summary>
	public void fire()
	{
		Instantiate (a_Cannonball, a_SpawnLocation.position, Quaternion.identity);
	}

	/// <summary>
	/// Updates the firing angle of the cannon. Called after every switch of selected cannon
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


		FiringVector = (a_FiringPointB.position - a_FiringPointA.position).normalized;
		FiringVector *= a_FiringForce;

		
	}

	/// <summary>
	/// Activates the selectedIcon when called and updates the Firing angle
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

		a_FiringForce = 11.5f;

		FiringVector = (a_FiringPointB.position - a_FiringPointA.position).normalized;
		FiringVector *= a_FiringForce;

	}
}

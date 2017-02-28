using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manager Class, handles lists containing all Transforms with relevant bounds
/// for collidable objects to check against
/// </summary>
public class BoundsManager : MonoBehaviour {


	/// <summary>
	/// List of all relevant bounds in the scene
	/// </summary>
	public static List<Transform> a_BoundsList;

	/// <summary>
	/// List of all relevant projectiles on screen at any moment
	/// </summary>
	public static List<Transform> a_ProjectilesList;

	public static Transform a_BoundsContainerA;
	public static Transform a_BoundsContainerB;

	/// <summary>
	/// Instantiate and/or populate the lists. Called once the terrain has been created fully
	/// </summary>
	public static void populateBoundsList()
	{
		// Initialise list of all bounds in game
		a_BoundsList = new List<Transform> ();
		a_ProjectilesList = new List<Transform> ();

		// Add the top terrain to the list of Bounds to be checked against by the projectile
		a_BoundsList.Add (GameObject.Find ("Top_Terrain").transform);


		// Set the two BoundsContainers objects to the GameObjects containing all instantiated terrain
		a_BoundsContainerA = GameObject.Find ("Terrains_Object_Left").transform;
		a_BoundsContainerB = GameObject.Find ("Terrains_Object_Right").transform;
	

		// Loop through all children in first Bounds gameObject and add their bounds to a_BoundsList
		foreach (Transform child in a_BoundsContainerA) 
		{
			a_BoundsList.Add(child);
		}

		// Loop through all children in second Bounds gameObject and add their bounds to a_BoundsList
		foreach (Transform child in a_BoundsContainerB) 
		{
			a_BoundsList.Add(child);
		}
	}
}

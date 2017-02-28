using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class managing player input
/// </summary>
public class Player : MonoBehaviour {

	/// <summary>
	/// The left cannon.
	/// </summary>
	[SerializeField]
	private Cannon a_LeftCannon;

	/// <summary>
	/// The right cannon.
	/// </summary>
	[SerializeField]
	private GOATCannon a_RightCannon;

	/// <summary>
	/// A cannon toggle, used to determine which cannon is selected
	/// when True, LeftCannon is selected
	/// when False, RightCannon is selected
	/// </summary>
	private bool a_CannonToggle;

	/// <summary>
	/// Handles player input
	/// </summary>
	private void handleInput()
	{
		// On Tab press, swap cannons
		if (Input.GetKeyDown(KeyCode.Tab))
		{
			chooseCannon();	
		}

		// On Space press, fire the specified cannon
		if (Input.GetKeyDown(KeyCode.Space))
		{
			fireCannon();	
		}
	}

	/// <summary>
	/// Toggles between cannons whenever it is called
	/// </summary>
	private void chooseCannon()
	{
		// Updates cannon toggle so it chooses the opposite cannon on next press
		a_CannonToggle = !a_CannonToggle;

		if (a_CannonToggle) 
		{
			a_LeftCannon.selected();
			a_RightCannon.deselected();
		}

		else
		{
			a_RightCannon.selected();
			a_LeftCannon.deselected();
		}
			
	}

	/// <summary>
	/// Fires the currently selected cannon. 
	/// If a_CannonToggle fire LeftCannon
	/// if !a_CanonToggle fire RightCannon
	/// </summary>
	private void fireCannon()
	{
		if (a_CannonToggle) { a_LeftCannon.fire(); }
		else { a_RightCannon.fire (); }

	}
	// Use this for initialization
	void Start () {

		// Instantiate the starting cannon to the left one
		a_CannonToggle = true;
		a_LeftCannon.selected ();
		a_RightCannon.deselected ();

	}
	
	// Update is called once per frame
	void Update () {

		handleInput ();
	}
}

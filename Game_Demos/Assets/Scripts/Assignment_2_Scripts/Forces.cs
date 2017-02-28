using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class managing all forces ingame.
/// </summary>
public class Forces : MonoBehaviour {


	/// <summary>
	/// The Gravity force
	/// </summary>
	public static float Gravity = -5.8f;

	/// <summary>
	/// The wind force
	/// </summary>
	public static float Wind;


	/// <summary>
	/// Updates the wind every 0.5 seconds, within a range of values
	/// where the lowest value prevents cannonball from reaching top with all other
	/// default parameters
	/// </summary>
	private void updateWind()
	{
		Wind = Random.Range (-1.5f, 1.5f);
	}

	// Use this for initialization
	void Start () {
		InvokeRepeating ("updateWind", 0f, 0.5f);
	}
}

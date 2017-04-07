using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Cannonball manages all motion-based behaviour
/// </summary>
public class Cannonball : MonoBehaviour {

	/// <summary>
	/// Time passed between current time and next update of projectile motion
	/// </summary>
	private float a_TimePassed;

	/// <summary>
	/// The horizontal and vertical components of the initial velocity
	/// </summary>
	private float Vx;
	private float Vy;

	/// <summary>
	/// Value approximating when the cannonball is considered stationary
	/// </summary>
	private float epsilon;

	/// <summary>
	/// This is a normalised vector of the initial velocity and direction of the cannonball
	/// </summary>
	private Vector2 a_MotionVector;

	/// <summary>
	/// Accumulation of all forces on the projectile
	/// </summary>
	private Vector2 a_CollisionForce;

	/// <summary>
	/// The rotation in degrees of the firing vector.
	/// </summary>
	[SerializeField]
	private float a_RotationInDegrees;

	/// <summary>
	/// Launches this instance of cannonball using simple projectile motion
	/// </summary>
	private void fire()
	{
		// Get the angle between the motion vector and the horizontal vector.right
		a_RotationInDegrees = Vector3.Angle (Vector3.right, a_MotionVector);

		// Set Vx to be the motionVector times the cos of firing angle (converted to Radians)
		Vx = a_MotionVector.x * Mathf.Cos (a_RotationInDegrees * Mathf.Deg2Rad);

		// Add the force of a collision. Should be 0 if no collision occurs
		Vx += a_CollisionForce.x;

		// Add the wind force
		Vx += Forces.Wind;

		// Multiply the value by the change in time.
		Vx *= Time.deltaTime;

		//-----------------------

		// Set Vy to be the motionVector times the sin of the firing angle (converted to Radians)
		Vy = a_MotionVector.y * Mathf.Sin (a_RotationInDegrees * Mathf.Deg2Rad);

		// Apply the force of gravity. Multiplied by the elapsed time to simulate acceleration
		Vy += (Forces.Gravity * a_TimePassed);

		// Add the force of collision. Should be 0 if no collisions occur
		Vy += a_CollisionForce.y;

		// Multiply the value by the change in time
		Vy *= Time.deltaTime;

		//-------------------------

		// Update the cannonball's position
		transform.Translate (Vx, Vy, 0);

		// Incrememnt time passed to be applied to the gravity calculations
		a_TimePassed += Time.deltaTime;

		// Check for if cannonball is relatively motionless
		checkMotion ();
	}


	/// <summary>
	/// Applies the force to the cannonball
	/// </summary>
	/// <param name="p_Force">P force.</param>
	public void applyForce(Vector2 p_Force)
	{
		a_CollisionForce += p_Force;
	}


	/// <summary>
	/// Checks the motion of this cannonball to see whether stationary or not against an epsilon
	/// </summary>
	private void checkMotion()
	{
		bool xCheck = Mathf.Abs(0 - Vx) < epsilon;
		bool yCheck = Mathf.Abs (0 - Vy) < epsilon;

		if (xCheck && yCheck) 
		{
			destroyThis ();
		}
	}

	/// <summary>
	/// Return the motion vector of the cannonball
	/// </summary>
	/// <returns>The v.</returns>
	public Vector3 getV()
	{
		Vector3 r_Vec = new Vector3(a_MotionVector.x * 0.1f, a_MotionVector.y * 0.1f, 0);
		return r_Vec;
	}

	/// <summary>
	/// Remove this instance from the list of projectiles in the BoundsManager, then destroy it
	/// </summary>
	public void destroyThis()
	{
		BoundsManager.a_ProjectilesList.Remove (transform);
		Destroy (gameObject);
	}

	// Use this for initialization
	void Start () {

		// Set the epsilon for determining a stationary state
		epsilon = 0.0005f;


		// Set the applied external forces equal to 0 at start
		a_CollisionForce = Vector2.zero;

		// Set the vector of motion equal to the Cannon's initial firing vector
		a_MotionVector = Cannon.FiringVector;

		// Add this cannonball to the list of projectiles on screen
		BoundsManager.a_ProjectilesList.Add (transform);
		
	}
	
	// Update is called once per frame
	void Update () {

		//handleInput ();
		fire ();
		
	}
}

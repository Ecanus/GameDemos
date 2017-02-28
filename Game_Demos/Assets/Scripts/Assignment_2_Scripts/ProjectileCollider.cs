using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class managing projectile collision detection and resolution
/// </summary>
public class ProjectileCollider : MonoBehaviour {


	/// <summary>
	/// The cannonball component attached to this gameObject
	/// </summary>
	private Cannonball _Cannonball;

	/// <summary>
	/// The coefficient of restitution, which affects cannonball bounce
	/// </summary>
	private float a_CoeffRestitution;

	/// <summary>
	/// Damper variable to help keep bounces from going too high
	/// </summary>
	private float a_Damper;

	/// <summary>
	/// The bounds of this collider
	/// </summary>
	private Bounds _Bounds;

	/// <summary>
	/// The bounds of a potential object to be collided with
	/// </summary>
	private Bounds a_PotentialIntersection;


	/// <summary>
	/// Loop through all objects containing bounds, and check if they intersect with this collider
	/// </summary>
	private void checkAllBounds()
	{
		// Get the bounds component of this transform at this frame
		_Bounds = GetComponent<Renderer> ().bounds;

		foreach (Transform p_BoundsTransform in BoundsManager.a_BoundsList) 
		{
			// Set the current investigated bounds to the bounds of the p_BoundsTransform
			a_PotentialIntersection = p_BoundsTransform.GetComponent<Renderer> ().bounds;

			// If a collision occurs between transform and ProjectileCollider, update
			// projectile motion vector with the normal of the Transform collided with
			if (_Bounds.Intersects (a_PotentialIntersection)) 
			{
				resolveCollision(p_BoundsTransform);
			}
		}
	}

	/// <summary>
	/// Check if this projectile is out of bounds with regards to screen space
	/// </summary>
	private void checkOutOfBounds()
	{
		// Get position of this cannonball on screen
		Vector2 screenPosition = Camera.main.WorldToScreenPoint (transform.position);

		bool tooFarRight = (screenPosition.x >= Camera.main.pixelWidth);
		bool tooFarLeft = (screenPosition.x < 0);
		bool tooLow = (screenPosition.y < (0.26f * Camera.main.pixelHeight));


		// Destroy this cannonball if it is too far right, left or down
		if (tooFarRight || tooFarLeft || tooLow) 
		{
			_Cannonball.destroyThis ();
		}
			
	}

	/// <summary>
	/// Resolves the collision by adding a force equal to the transform's scaled normal (up) vector
	/// </summary>
	/// <param name="p_CollisionObject">P collision object.</param>
	private void resolveCollision(Transform p_CollisionObject)
	{
		// Get normal of collided Transform
		Vector2 collisionNormal =  p_CollisionObject.transform.TransformPoint(Vector2.up).normalized;
		collisionNormal *= 10.5f;
		collisionNormal *= a_CoeffRestitution;

		// Apply that normal as a force
		_Cannonball.applyForce (collisionNormal);
	}
		

	// Use this for initialization
	void Start () {

		// Instantiate Bounds and Cannonball using components
		_Bounds = GetComponent<Renderer> ().bounds;
		_Cannonball = GetComponent<Cannonball> ();

		// Instantiate Damper and Restitution to modify collision resolution
		a_Damper = 1;
		a_CoeffRestitution = 0.10f;
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		// Check if out of bounds or if intersecting per frame
		checkOutOfBounds();
		checkAllBounds();
	}
}

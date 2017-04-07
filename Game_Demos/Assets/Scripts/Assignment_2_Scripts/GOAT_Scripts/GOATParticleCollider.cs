using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Collider attached to each GOAT prefab to check for if the single if the single particle
/// is colliding with any collidable objects in the scene
/// </summary>
public class GOATParticleCollider : MonoBehaviour {

	/// <summary>
	/// The GOAT component attached to this gameObject
	/// </summary>
	private GOATParticle _GOATParticle;


	/// <summary>
	/// The bounds of this collider
	/// </summary>
	private Bounds _Bounds;

	/// <summary>
	/// The bounds of a potential object to be collided with
	/// </summary>
	private Bounds a_PotentialIntersection;

	/// <summary>
	/// True if a collider for a foot particle
	/// </summary>
	[SerializeField]
	private bool isSticky;


	/// <summary>
	/// Loop through all objects containing bounds, and check if they intersect with this collider
	/// </summary>
	private void checkAllBounds()
	{
		_Bounds = GetComponent<Renderer> ().bounds;

		// Loop through all transforms in the BoundsManager's list of objects
		foreach (Transform p_BoundsTransform in BoundsManager.a_BoundsList) 
		{
			// Set the current investigated bounds to the bounds of the p_BoundsTransform
			a_PotentialIntersection = p_BoundsTransform.GetComponent<Renderer> ().bounds;


			// If a collision occurs between transform and ProjectileCollider, update
			// projectile motion vector with the normal of the Transform collided with
			if (_Bounds.Intersects (a_PotentialIntersection)) 
			{
				resolveCollision (p_BoundsTransform, a_PotentialIntersection);
			}
		}

		// Loop through list of projectiles if there exists anything in the list
		if (BoundsManager.a_ProjectilesList.Count > 0) 
		{	
			// Temporarily hold on to the list of projectiles on screen and loop through them
			List<Transform> temp = new List<Transform> (BoundsManager.a_ProjectilesList);

			foreach (Transform p_BoundsTransform in temp) {

				// Set the current investigated bounds to the bounds of the p_BoundsTransform
				a_PotentialIntersection = p_BoundsTransform.GetComponent<Renderer> ().bounds;

				// If a collision occurs between GOAT Particle and a Cannonball, transfoer the motion
				// to the GOAT and destroy the cannonball
				if (_Bounds.Intersects (a_PotentialIntersection)) {

					// Get the cannonball component of the BoundsTransform
					Cannonball _cannonball = p_BoundsTransform.GetComponent<Cannonball> ();

					// Add the motion
					_GOATParticle.addForce (_cannonball.getV ());

					// Destroy the cannonball
					_cannonball.destroyThis ();
				}
			}
		}
	}
		

	/// <summary>
	/// Resolves the collision by adding a force equal to the transform's scaled normal (up) vector
	/// </summary>
	/// <param name="p_CollisionObject">P collision object.</param>
	private void resolveCollision(Transform p_CollisionObject, Bounds p_CollisionBounds)
	{
		// If a stick particle, make it stay still. Couldn't get working well

		/*
		if (isSticky) 
		{
			_StickyPosition = transform.position;
			isStuck = true;
			_GOATManager.stabilise ();
		}*/

		// Get the normal of the Transform
		Vector3 collisionNormal =  p_CollisionObject.transform.TransformPoint(Vector2.up).normalized;
		collisionNormal *= 0.1f;

		// Apply the force to the GOAT particle
		_GOATParticle.addForce(collisionNormal);

	}

	// Use this for initialization
	void Start () {


		_Bounds = GetComponent<Renderer> ().bounds;
		_GOATParticle = GetComponent<GOATParticle> ();

	}

	// Update is called once per frame
	void FixedUpdate () {

		// Check for if colliding with terrain or projectiles
		checkAllBounds();
	}
}

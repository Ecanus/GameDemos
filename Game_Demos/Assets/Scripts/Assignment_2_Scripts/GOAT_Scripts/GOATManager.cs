using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class which loops through a GOAT object and structurally applies force to each GOAT particle
/// </summary>
public class GOATManager : MonoBehaviour {

	/// <summary>
	/// Eye GOAT particle. Added externally
	/// </summary>
	[SerializeField]
	private GOATParticle a_Eye;


	/// <summary>
	/// The body Transform of this GOAT instance
	/// </summary>
	[SerializeField]
	private Transform a_GOATBody;

	/// <summary>
	/// Keep track of whether to move or to constrain in the current frame
	/// </summary>
	[SerializeField]
	private bool swapState;

	/// <summary>
	/// Move all particles in a structured, rigorous fashion
	/// </summary>
	private void moveAllParticles()
	{

		// Handle eye integration seperately as it is not part of the other grouped 'body' particles
		GOATParticle c_GOAT = a_Eye;
		a_Eye.verletIntegrate();

		// Loop through rest of particles, verlet integrating all non-kinematic ones
		foreach (Transform child in a_GOATBody) 
		{
			c_GOAT = child.GetComponent<GOATParticle> ();
			c_GOAT.verletIntegrate ();
		}
	
		// -------------

		// SwapState. This signals the constraints to be checked for in the next frame
		swapState = false;

	}

	/// <summary>
	/// Checks against all particles to make sure they are obeyeing constraints
	/// </summary>
	private void constrainAllParticles()
	{

		// Handle Eye constraints seperately as it isn't part of the other grouped 'body' particles
		GOATParticle c_GOAT = a_Eye;
		a_Eye.obeyConstraints();

		// Loop through rest of particles, making sure each obeys its constraints
		foreach (Transform child in a_GOATBody) 
		{
			c_GOAT = child.GetComponent<GOATParticle> ();
			c_GOAT.obeyConstraints ();
		}
			
		// swap state, signalling the verlet integration to be done in the next frame
		swapState = true;

	}

	public void stabilise()
	{
		StartCoroutine ("resolveGoat");
	}

	private IEnumerator resolveGoat()
	{
		yield return new WaitForSeconds(1.5f);

		// Handle Eye constraints seperately as it isn't part of the other grouped 'body' particles
		GOATParticle c_GOAT = a_Eye;
		a_Eye.setKinematic(true);

		// Loop through rest of particles, making sure each obeys its constraints
		foreach (Transform child in a_GOATBody) 
		{
			c_GOAT = child.GetComponent<GOATParticle> ();
			c_GOAT.setKinematic (true);
		}
	}

	// Use this for initialization
	void Start () {
		swapState = true;
	}
	
	// Update is called once per frame
	void Update () {


		// Move particles in one frame
		if (swapState) 
		{ 
			moveAllParticles (); 
		}

		// Check for constraints in the other
		if (!swapState) 
		{ 
			constrainAllParticles();
		}

	}
}

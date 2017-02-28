using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Particles that comprise the GOAT model. Each being moved by Verlet Integration and obeying constraints
/// set in place
/// </summary>
public class GOATParticle: MonoBehaviour {

	/// <summary>
	/// State determining whether this instance of particle is affected by force or not
	/// </summary>
	[SerializeField]
	private bool isKinematic;

	/// <summary>
	/// The highbounding value for maintaining constraints
	/// </summary>
	[SerializeField]
	private float a_HighConstraint;

	/// <summary>
	/// The low bounding value for maintaining constraints
	/// </summary>
	[SerializeField]
	private float a_LowConstraint;

	/// <summary>
	/// The number of particles this instance must keep track of within its constraints
	/// </summary>
	[SerializeField]
	private int a_DependenciesCount;

	/// <summary>
	/// Other GOAT particles that this instance is constrained to keeping boundaries between
	/// </summary>
	[SerializeField]
	private Transform a_CParticle1;
	[SerializeField]
	private Transform a_CParticle2;
	[SerializeField]
	private Transform a_CParticle3;
	[SerializeField]
	private Transform a_CParticle4;
	[SerializeField]
	private Transform a_CParticle5;

	/// <summary>
	/// List used to contain the particles this instance is constrained to
	/// </summary>
	private Transform[] a_CParticles;

	/// <summary>
	/// Edges needed to keep track of the original lengths between particle constraints
	/// </summary>
	private Vector2[] a_Edges;

	/// <summary>
	/// Position Vectors used for Verlet Integration
	/// </summary>
	private Vector3 a_NewPosition;
	private Vector3 a_LastPosition;

	/// <summary>
	/// The time step used in the Verlet Integration
	/// </summary>
	private float a_TimeStep;

	/// <summary>
	/// Acceleration vector used in Verlet Integration
	/// </summary>
	private Vector2 a_Acceleration;

	/// <summary>
	/// Vector for calculating gravity of the GOAT particle
	/// </summary>
	public Vector3 gravityForce;

	/// <summary>
	/// This is a normalised vector of the direction of the cannon barrel, scaled by the velocity
	/// </summary>
	[SerializeField]
	private Vector2 a_MotionVector;


	/// <summary>
	/// Fire this instance of GOAT particle using verlet integration
	/// </summary>
	public void verletIntegrate()
	{
		// If kinematic return immediately
		if (isKinematic) { return; }
	

		// update LastPosition with the windforce (scaled to work better for the integration formula)
		a_LastPosition.x -= (Forces.Wind * 0.008f);

		// Get velocity by calculating difference between new position and the last position
		Vector3 velocity = (a_NewPosition - a_LastPosition);


		// Get to next position using previous position and acceleration
		a_NewPosition = transform.position + velocity + (gravityForce * a_TimeStep * a_TimeStep);

		// Set old position to the position of cannonball before the integration step
		a_LastPosition = transform.position;

		// Update current position
		transform.position = a_NewPosition;
	
	}

	/// <summary>
	/// Adds a force to the verlet integration. Only if non-kinematic
	/// </summary>
	/// <param name="p_Force">P force.</param>
	public void addForce(Vector3 p_Force)
	{
		if (!isKinematic) { a_LastPosition -= p_Force; }
	}

	/// <summary>
	/// Called in the update() method to maintain constraints all throughout flight
	/// </summary>
	public void obeyConstraints()
	{
		// Vector to check the position between this instance and each of the constrained particles
		Vector3 edgeCheck;

		// Float to hold the length of the edgeCheck value
		float edgeCheckLength;

		// Float to hold the difference between the updated positions and original edge length
		float constraintDiff;

		// Update the particle positions before checking constraints between them
		getConstrainedParticles ();

		for (int x = 0; x < a_DependenciesCount; x++) 
		{
			// Get the vector between this instance and the x-th other particle
			edgeCheck = transform.position - a_CParticles[x].position;
			edgeCheckLength = edgeCheck.magnitude;

			// Calculate the constraint differences by checking the current length against original length
			constraintDiff = edgeCheck.magnitude - a_Edges [x].magnitude;

			// If no obeying is needed, immediately return
			if (constraintDiff < a_HighConstraint && constraintDiff > a_LowConstraint) {return;}

			// Push points together if too far
			if (constraintDiff > a_HighConstraint) 
			{
				edgeCheck.Normalize ();

				transform.position -= (edgeCheck * constraintDiff * 0.5f);
				a_CParticles [x].position += (edgeCheck * constraintDiff * 0.5f);
			}


			// Pull points together if too far in other direction
			if (constraintDiff < a_LowConstraint) 
			{
				edgeCheck.Normalize ();

				transform.position -= (edgeCheck * constraintDiff * 0.5f);
				a_CParticles [x].position += (edgeCheck * constraintDiff * 0.5f);
			}
		}
	}

	/// <summary>
	/// Creates the edges and their lengths needed to be kept constant throughout runtime between particles
	/// </summary>
	private void setConstraints()
	{
		a_Edges = new Vector2[a_DependenciesCount];

		for (int x = 0; x < a_DependenciesCount; x++) 
		{
			a_Edges [x] = transform.position - a_CParticles[x].position;
		}
	}

	/// <summary>
	/// Instantiates the list of constrainted particles
	/// based on how many particle dependencies were specifed in scene view
	/// </summary>
	private void getConstrainedParticles()
	{
		a_CParticles = new Transform[a_DependenciesCount];

		switch (a_DependenciesCount) 
		{
		case 1:
			a_CParticles [0] =  a_CParticle1;
			break;
		case 2:
			a_CParticles [0] = a_CParticle1;
			a_CParticles [1] = a_CParticle2;
			break;
		case 3:
			a_CParticles [0] = a_CParticle1;
			a_CParticles [1] = a_CParticle2;
			a_CParticles [2] = a_CParticle3;
			break;
		case 4:
			a_CParticles [0] = a_CParticle1;
			a_CParticles [1] = a_CParticle2;
			a_CParticles [2] = a_CParticle3;
			a_CParticles [3] = a_CParticle4;
			break;
		case 5:
			a_CParticles [0] = a_CParticle1;
			a_CParticles [1] = a_CParticle2;
			a_CParticles [2] = a_CParticle3;
			a_CParticles [3] = a_CParticle4;
			a_CParticles [4] = a_CParticle5;
			break;
		default:
			break;

		}
	}

	/// <summary>
	/// Sets the initial positionings for the verlet integration to use
	/// </summary>
	private void setPositions()
	{
		a_NewPosition = transform.position;
		a_LastPosition = transform.position;
	}

	/// <summary>
	/// Sets the initial motion vector using the GOATCannon's firing vector value
	/// </summary>
	private void setMotionVector()
	{
		a_MotionVector = GOATCannon.FiringVector;
		addForce (a_MotionVector);
	}
		
	/// <summary>
	/// Sets this instance's kinematic value to whichever value p_NewState is
	/// </summary>
	/// <param name="p_NewState">If set to <c>true</c> p new state.</param>
	public void setKinematic(bool p_NewState)
	{
		isKinematic = p_NewState;
	}

	// Use this for initialization
	void Start () {

		// Set the Particle positionings
		setPositions ();

		// Only apply initial velocity to non-kinematic GOAT particles
		setMotionVector();

		// Instantiate all constraints
		getConstrainedParticles ();
		setConstraints();

		// Instantiate the timestep to be used
		a_TimeStep = 0.45f;

		// Instantiate the gravity to be used
		gravityForce = Vector2.down * 0.025f;

	}
		
}


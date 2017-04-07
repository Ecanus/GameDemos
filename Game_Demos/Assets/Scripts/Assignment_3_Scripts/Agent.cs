using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour {

	#region Mood and Behaviour Attributes

	/// <summary>
	/// Agent state enum, and attribute containing this intance's behaviour at any given time
	/// </summary>
	public enum AgentState {IDLE, IDLING, MOVING, SEARCH, CONSIDERING}
	public AgentState _myState;

	/// <summary>
	/// Agent mood enum, and attribute containing this instance's mood at any given time
	/// </summary>
	public enum AgentMood {CALM, ANNOYED, FRUSTRATED};
	public AgentMood _myMood;

	#endregion

	[SerializeField]
	private GameManager _GM;

	/// <summary>
	/// The pathing component attached to this instance
	/// </summary>
	private Pathing _Pathing;

	/// <summary>
	/// The destination node of this instance
	/// </summary>
	private Node _Destination;

	/// <summary>
	/// The professor target number this agent is currently in search of
	/// </summary>
	[SerializeField]
	private int _TargetNumber;

	/// <summary>
	/// The current node info, allowing agent to retrieve its information once adjacent
	/// </summary>
	[SerializeField]
	private int _CurrentNodeInfo;


	#region Time/Mood Attributes

	public float _timeSearchStarted;

	/// <summary>
	/// The original goal proximity between agent and destination. Set inside every recalculatePath() call.
	/// </summary>
	public float _originalGoalProx;

	#endregion


	#region Memory Atributes
	/// <summary>
	/// List of recently visited targets for agent to quickly choose from.
	/// </summary>
	[SerializeField]
	private List<int> _MemorisedTargets;

	/// <summary>
	/// The index of the memorisedTarget. Put against modulo four to constantly override up to 
	/// four most previously visited plaque/professors
	/// </summary>
	private int memIndex;

	#endregion


	/// <summary>
	/// The array of plaque node candidates, for agent to randomly select from as destination
	/// </summary>
	[SerializeField]
	private List<Node> plaqueNodeCandidates;

	/// <summary>
	/// The list of prof node candidates for agent to randomly select from as a target
	/// </summary>
	[SerializeField]
	private List<Node> profNodeCandidates;



	// Use this for initialization
	void Start () {

		// Set initial state to SEARCH
		_myState = AgentState.SEARCH;

		// Ensure the array of potential candidates is full
		resetPotentialCandidates ();

		// Set the index of the memorised targets to 0
		memIndex = 0;
		// Initialise the array of memorised targets to zero in each position
		_MemorisedTargets = new List<int>() {0,0,0,0};

		// Randomly select a number from 1 to 6 to be the Professor target
		_TargetNumber = Random.Range (1, 7);

		// Get the Patching component of this instance
		_Pathing = GetComponent<Pathing> ();

		// Set the start node for this agent to begin its operations
		setStart ();
	}
		

	/// <summary>
	/// Handles the mood of agent, changing its material accordingly, based on timewindows and goal proximity
	/// </summary>
	private void handleMood()
	{
		// Get the elapsed time since the agent search began
		float timeDiff = Time.time - _timeSearchStarted;
		//Debug.Log ("TimeDiff: " + timeDiff);

		// Check that the time from beginning search until now is withing a certain range
		bool annoyedTimeWindowCheck = (timeDiff >= 0.1f) && (timeDiff <= 3f);
		//Debug.Log ("annoyedWindowCheck : " + annoyedTimeWindowCheck);

		bool frustratedTimeWindowCheck = (timeDiff >= 3f);
		//Debug.Log ("frustratedWindowCheck : " + frustratedTimeWindowCheck);

		// If within that annoyed range, and distance between agent and destination hasn't shrunk by a noticeable amount
		// set AgentMood to ANNOYED
		if (annoyedTimeWindowCheck && checkGoalProximity ()) 
		{
			_myMood = AgentMood.ANNOYED;


			SpriteRenderer _sprite = GetComponent<SpriteRenderer> ();
			_sprite.color = Color.yellow;
			//GetComponent<Renderer> ().material = _myMaterials._AnnoyedMaterial;
		}

		// If exceeded annoyed range, and distance between agent and destination hasn't shrunk by a noticeable amount
		// set AgentMood to FRUSTRATED
		else if (frustratedTimeWindowCheck && checkGoalProximity ()) 
		{
			_myMood = AgentMood.FRUSTRATED;

			SpriteRenderer _sprite = GetComponent<SpriteRenderer> ();
			_sprite.color = Color.red;

			//GetComponent<Renderer> ().material = _myMaterials._FrustratedMaterial;
		}

		// Else, assume Agent is in a CALM mood
		else 
		{
			_myMood = AgentMood.CALM;
			SpriteRenderer _sprite = GetComponent<SpriteRenderer> ();
			_sprite.color = Color.blue;

			//GetComponent<Renderer> ().material = _myMaterials._CalmMaterial;
		}
			
	}

	/// <summary>
	/// Handles the behaviour of the agent
	/// </summary>
	private void handleBehaviour()
	{

		switch (_myState) 
		{
		case AgentState.MOVING:
			break;
		case AgentState.IDLING:
			break;
		case AgentState.SEARCH:
			setPlaqueDestination ();
			_Pathing.recalculatePath (_Pathing.currentNode, _Destination);
			_myState = AgentState.MOVING;
			break;
		case AgentState.CONSIDERING:
			break;
		case AgentState.IDLE:
			setRandomDestination ();
			_Pathing.recalculatePath (_Pathing.currentNode, _Destination);
			_myState = AgentState.MOVING;
			break;
		default:
			break;
		}
	}

	/// <summary>
	/// Sets the start node of the Agent to a random (TODO: unoccupied) node in the scene
	/// </summary>
	private void setStart()
	{
		List<Node> _tempDefaultNodes = new List<Node>(_GM.allDefaultNodes);
		int defaultCount = _tempDefaultNodes.Count;

		Node _newStart = _tempDefaultNodes [Random.Range(0,defaultCount)];
		_Pathing.setPosition (_newStart);
	}

	/// <summary>
	/// Gets a random PlaqueNode from GameManager, and sets as this agent's destination
	/// Does not look against nodes in previouslyVisitedPlaques
	/// </summary>
	private void setPlaqueDestination()
	{

		// ----IF _Target is in agent's list of recently visited professors already, go immediately there

		if (_MemorisedTargets.Contains (_TargetNumber)) 
		{
			setProfDestination (_TargetNumber);
			return;
		}


		// ----ELSE, randomly choose a plaqueNumber for agent to traverse towards

		// Initialise int to get random index from list of plaqueCandidate Nodes
		int candidatesCount = plaqueNodeCandidates.Count;

		// Use that value to pick a new plaqueNode to traverse towards
		Node _newDestination = plaqueNodeCandidates [Random.Range(0,candidatesCount)];


		bool check1 = !(_newDestination == _Destination);


		// If the new choice of assigned destination is not already the_Destination attribute value
		if (check1){
			_Destination = _newDestination;
		} 

	}


	/// <summary>
	/// Sets the teacher destination based on the parameter given
	/// </summary>
	/// <param name="p_TeacherNumber">P teacher number.</param>
	private void setProfDestination(int p_TeacherNumber)
	{
		// Get the list of nodes corresponding to Prof number from the GameManager method
		profNodeCandidates = new List<Node> (_GM.getProfNodeArray(p_TeacherNumber));

		// Set new destination to one of the Prof nodes that is not currently occupied
		Node _newDestination = pickProfNode();

		_Destination = _newDestination;
	}


	/// <summary>
	/// Sets the random destination for the Idle behaviour to send agent to
	/// </summary>
	private void setRandomDestination()
	{
		// Duplicate GameManager's list of all default nodes
		List<Node> _tempDefaultNodes = new List<Node>(_GM.allDefaultNodes);
		int defaultCount = _tempDefaultNodes.Count;

		Node _newDestination = _tempDefaultNodes [Random.Range(0,defaultCount)];
		_Destination = _newDestination;
	}


	/// <summary>
	/// Picks the prof node that is not currently occupied
	/// </summary>
	/// <returns>The prof node.</returns>
	private Node pickProfNode()
	{
		Node _newDestination;

		if (!profNodeCandidates [0].isOccupied) 
		{	
			_newDestination = profNodeCandidates [0];
		} 
		else if (!profNodeCandidates [1].isOccupied)
		{
			_newDestination = profNodeCandidates [1];
		} 
		else 
		{
			_newDestination = profNodeCandidates [2];
		}

		return _newDestination;
	}


	/// <summary>
	/// Method which is called by _Pathing when agent is adjacent to either PlaqueNode or ProfessorNode
	/// </summary>
	public void isAdjacent()
	{
		// Retrieves node information based on node type
		switch (_Pathing.currentNode._NodeType) 
		{
		case Node.NodeType.Plaque:
			StartCoroutine ("retrievePlaqueInformation");
			break;
		case Node.NodeType.Professor:
			StartCoroutine ("retrieveProfInformation");
			break;
		default:
			StartCoroutine ("idleAround");
			break;
		}

	}


	/// <summary>
	/// Retrieves the prof information for agent to use in setting a new target
	/// </summary>
	/// <returns>The prof information.</returns>
	private IEnumerator retrieveProfInformation()
	{
		// Set agent to the state of being advised
		_myState = AgentState.CONSIDERING;

		// Wait for some time before receiving advise
		yield return new WaitForSeconds (0.5f);

		// Get list of what may be one of the new professors to search for
		List<int> _potentialNewTargets = new List<int>() {1, 2, 3, 4, 5, 6};
		// Remove the agent's current target professor from the list
		_potentialNewTargets.Remove (_TargetNumber);

		// Set agent's new target to one of the other potential professors at random
		_TargetNumber = _potentialNewTargets[Random.Range(0, _potentialNewTargets.Count)];


		// Reset agent's list of potential candidates
		resetPotentialCandidates ();

		// Set the agent's state to either Idle or to Search, with a 50% chance of either state being chosen
		idleOrSearch ();

	}


	/// <summary>
	/// Randomly sets agent state to either Idle or Search, with a 50% chance of either being chosen
	/// </summary>
	private void idleOrSearch()
	{
		// Pick a random number between 0 and 50
		int _chance = Random.Range(1, 51);

		// If an even number, set state to Idle
		if (_chance % 2 == 0) _myState = AgentState.IDLE;

		// If an odd number, set state to search
		else _myState = AgentState.SEARCH;
	}


	/// <summary>
	/// Retrieves the plaque information after brief delay.
	/// If matches, tells agent to enter Professor's room and get advice
	/// If no match, tells agent to go looking for another plaqueNode
	/// </summary>
	/// <returns>The plaque information.</returns>
	private IEnumerator retrievePlaqueInformation()
	{
		yield return new WaitForSeconds (0.35f);
		_CurrentNodeInfo = _Pathing.currentNode.getInfo ();

		// Check if plaque NodeInfo matches agent's _Target
		if (_TargetNumber == _CurrentNodeInfo) 
		{

			// set agent destination to one of the professor nodes
			setProfDestination (_TargetNumber);
			_Pathing.recalculatePath (_Pathing.currentNode, _Destination);
		}
			
		 
		// If NodeInfo doesn't match agent's _Target 
		if (_TargetNumber != _Pathing.currentNode.getInfo ()) 
		{

			// Add  node, and its sister if it has one
			// to list of visited nodes that can no longer be a destination
			removeFromCandidates(_Pathing.currentNode);

			_myState = AgentState.SEARCH;
		}

		// Add this information to memorisedTargets array
		_MemorisedTargets [memIndex] = _Pathing.currentNode.getInfo ();
		memIndex = (memIndex + 1) % 4;
	}

	/// <summary>
	/// Set agent mood based on proximity to goal
	/// </summary>
	/// <returns>The goal proximity.</returns>
	private bool checkGoalProximity()
	{

		// Get the current distance between agent
		float _currGoalProx = Vector3.Magnitude (transform.position -_Destination.GetComponent<Transform> ().position);

		// Check if the distance between the agent and destination hasn't changed by a threshold
		// if this is the case, return true
		bool _toReturn = (_originalGoalProx - _currGoalProx) <= 1.5f;

		_originalGoalProx = _currGoalProx;

		return _toReturn;

	}

	/// <summary>
	/// Has agent idle at random destination for a period of time, before resuming its searching
	/// </summary>
	/// <returns>The around.</returns>
	private IEnumerator idleAround()
	{
		_myState = AgentState.IDLING;

		float _idleTime = Random.Range (0.5f, 2f);
		yield return new WaitForSeconds (_idleTime);

		_myState = AgentState.SEARCH;
	}

	/// <summary>
	/// Has agent step aside to a neighbouring node to make way for another passing agent
	/// </summary>
	public void stepAside()
	{

		// Get the list of neighbours of the currentNode agent is located on
		List<Node> nodeNeighbours = _Pathing.currentNode.getNeighbours ();

		// Initialise int to get the number of elements in the neighbours list
		int neighboursCount = nodeNeighbours.Count;

		// Use that value to pick a new plaqueNode to traverse towards
		Node _newDestination = nodeNeighbours [Random.Range(0,neighboursCount)];

		// Go to that neighbour
		_Pathing.recalculatePath(_Pathing.currentNode, _newDestination);
	}

	/// <summary>
	/// Removes parameter from plaqueNodeCandidates array. Removes sister if one exists
	/// </summary>
	/// <param name="p_Node">P node.</param>
	private void removeFromCandidates(Node p_Node)
	{
		if (plaqueNodeCandidates.Contains (p_Node)) 
		{

			plaqueNodeCandidates.Remove (p_Node);
			if (p_Node.sister != null) { plaqueNodeCandidates.Remove (p_Node.sister); }
		}
	}
		

	/// <summary>
	/// Resets the potential candidates array with the GameManager's PlaqueNodes array
	/// </summary>
	private void resetPotentialCandidates()
	{
		plaqueNodeCandidates = new List<Node> (_GM.PlaqueNodes);
	}

	// Update is called once per frame
	void Update () {
		//handleInput ();

		handleBehaviour();
		handleMood();
	}
}

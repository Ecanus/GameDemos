using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class Pathing : MonoBehaviour {

	private Agent _Agent;

	/// <summary>
	/// Value changing how far back in time reservations are made
	/// during group pathfinding
	/// </summary>
	public static int _lookBack;

	#region Node Attributes
	/// <summary>
	/// The start node in the Pathing's path
	/// </summary>
	[SerializeField]
	private Node startNode;

	/// <summary>
	/// The destination node in the Pathing's path
	/// </summary>
	[SerializeField]
	private Node destinationNode;

	/// <summary>
	/// The current node this instance is at within its path
	/// </summary>
	[SerializeField]
	public Node currentNode;

	/// <summary>
	/// The next node to go to in this instance's path
	/// </summary>
	[SerializeField]
	private Node nextNode;

	private Node previousPathNode;
	#endregion


	#region Motion Attributes
	/// <summary>
	/// Bool allowing instance to call the move() method
	/// </summary>
	public bool canMove;

	public bool atNextNode;

	/// <summary>
	/// Bool stating this instance can begin to traverse along the path it has created
	/// </summary>
	public bool isInMotion;

	/// <summary>
	/// The move speed of the instance along its path
	/// </summary>
	[SerializeField]
	private float moveSpeed;

	/// <summary>
	/// The index of where in traverse path array pathing instance currently is
	/// </summary>
	[SerializeField]
	private int traversePathIndex;

	/// <summary>
	/// Trial int, checking how many times a traversal has failed to work
	/// </summary>
	private int trial;
	#endregion

	#region Node Containers

	// Set containing all 'open' nodes to be considered
	private List<Node> openSet;

	// Set containing all 'closed' nodes, that have been looked at
	private HashSet<Node> closedSet;

	[SerializeField]
	private List<Node> finalPath;

	#endregion

	// Use this for initialization
	void Start () {

		_lookBack = 1;

		traversePathIndex = -2;

		atNextNode = false;

		// Add this Pathing instance to the GameClock so that its movement in time can be standardised
		GameClock.PathingAgents.Add (this);

		_Agent = GetComponent<Agent> ();

		canMove = false;
		isInMotion = false;

		//setPosition (startNode); 

	}

	/// <summary>
	/// Sets the position of the instance, and sets the currentNode attribute to the parameter of this method
	/// </summary>
	/// <param name="p_Node">P node.</param>
	public void setPosition(Node p_Node)
	{
		Vector3 _tempPos = p_Node.GetComponent<Transform>().position;
		transform.position = _tempPos;

		currentNode = p_Node;
		currentNode.isOccupied = true;

	}
		

	/// <summary>
	/// Initialises the openSet and closedSet before beginning traversal
	/// </summary>
	private void initialiseSets()
	{
		// Set containing all 'open' nodes to be considered
		openSet = new List<Node>();

		// Set containing all 'closed' nodes, that have been looked at
		closedSet = new HashSet<Node> ();

		// Add the startNode to list of open nodes
		openSet.Add (startNode);
		//startNode.isOpen ();
	}


	/// <summary>
	/// Calculates the path for the agent to traverse over
	/// </summary>
	public void calculatePath()
	{

		// If there exists a node in the list of 'open' nodes, go through once
		while (openSet.Count > 0) 
		{
			Node _node = openSet [0];
			for (int x = 1; x < openSet.Count; x++) 
			{
				// If the current node under inspection has an fCost lower than the best candidate node in 
				// openSet, and an hCost lower than best candidate node in openSet, then add to the openSet
				if (openSet [x].fCost <= _node.fCost && openSet [x].hCost < _node.hCost) 
				{
					_node = openSet [x];
				}
			}

			// Remove the previous best candidate from the openSet
			openSet.Remove (_node);

			// Place that node into the closedSet
			closedSet.Add (_node);
			//_node.isClosed ();

			// If the best candidate node is also destination, retrace the path to form
			// final traversable route, and then return
			if (_node == destinationNode) 
			{
				createFinalPath (startNode, destinationNode);
				return;
			}

			// Loop through the neighbours of _node, updating costs of nodes to find
			// best path
			foreach (Node _neighbour in _node.getNeighbours()) 
			{
				// If neighbour is already in closedSet, continue
				if (closedSet.Contains (_neighbour)) 
				{
					continue;
				}

				// set the potential updated cost to _node.gCost + distance from node to the neighbour
				int updatedCost = _node.gCost + calculateDistance (_node, _neighbour);

				// If the updatedCost is less than _neighbour's already calculated gCost
				// of if openSet does not contain this _neighbour
				// update this neighbour's gCost, update its hCost and set its parent to _node
				// add to openSet if it is not already contained within
				if (updatedCost < _neighbour.gCost || !openSet.Contains (_neighbour)) 
				{
					_neighbour.gCost = updatedCost;
					_neighbour.hCost = calculateDistance (_neighbour, destinationNode);
					_neighbour.setParent (_node);

					if (!openSet.Contains (_neighbour)) 
					{
						openSet.Add (_neighbour);
						//_neighbour.isOpen ();
					}
				}

			}

		}
	}


	/// <summary>
	/// Recalculates the path for agent, using parameters given
	/// </summary>
	/// <param name="p_NewStart">P new start.</param>
	/// <param name="p_NewDestination">P new destination.</param>
	public void recalculatePath(Node p_NewStart, Node p_NewDestination)
	{
		//if (finalPath.Count > 1)
		//	previousPathNode = finalPath [finalPath.Count - 1];

		startNode = p_NewStart;
		destinationNode = p_NewDestination;

		setPosition (p_NewStart);

		initialiseSets ();

		calculatePath ();

		// AgentMOOD behaviour ----

		_Agent._originalGoalProx = Vector3.Magnitude (transform.position - p_NewDestination.GetComponent<Transform> ().position);
		_Agent._timeSearchStarted = Time.time;

		// ------
		isInMotion = true;
	}


	/// <summary>
	/// Creates the final path by retracing each node from the A* calculation via the parent
	/// and then once the startNode is reached, reversing the list of nodes
	/// </summary>
	/// <param name="p_StartNode">P start node.</param>
	/// <param name="p_EndNode">P end node.</param>
	private void createFinalPath(Node p_FirstNode, Node p_LastNode)
	{
		

		// Instantiate list to hold the nodes
		finalPath = new List<Node>();

		// Node to manage which node in the path is being observed at a point in time
		Node _tempNode = p_LastNode;

		// Loop through remaining nodes in the list, up until startNode is reached
		while (_tempNode != p_FirstNode) 
		{
			finalPath.Add (_tempNode);

			//_tempNode.isPathNode ();

			_tempNode = _tempNode.getParent();
		}

		// Add startNode 
		finalPath.Add (_tempNode);
		//_tempNode.isPathNode ();

		// Reverse the accumulated list 
		finalPath.Reverse ();
		handleReservations ();
	}

	/// <summary>
	/// Handles the reservations-based pathfinding for this scenario
	/// </summary>
	private void handleReservations()
	{
		int indx = 0;

		// TIME variables -------
		// Get what time it is in the game world at this very instance
		int _timer = GameClock._Time;
		//----------

		// TEMP FINAL PATH
		// Temporary list to contain the new, reservation-included, final path
		List<Node> tempFinalPath = new List<Node>();
		//---------


		bool reserveConflict = false;


		// Loop through each node in the final path
		foreach (Node _pathNode in finalPath) 
		{
			// If reservation can be placed at this node, do so
			// add this node to temporaryFinalPath list
			// increment timer, and tempTime index
			if (_pathNode.canPlaceReservation (_timer)) 
			{

				_pathNode.placeReservation (_timer, name);
				tempFinalPath.Add (_pathNode);

				_timer++;

			} 


			// If unsuccessful, set reserveConflict to true
			// and continuously place 'waiting' nodes into the temp
			else 
			{
				
				int tempLookBack = _lookBack;

				int safetyCheck = 0;

				reserveConflict = true;
				while (reserveConflict) 
				{

					if (safetyCheck >= 40) 
					{
						return;
					}

					// Clamp lookBack value to never exceed how far into reservations the method has gone
					tempLookBack = Mathf.Clamp (_lookBack, 0, indx);


					// Get the index of the node located at 'lookBack' positions away
					int copyIndex = indx - tempLookBack;

					// Look back by the _lookBack amount, inside of finalPath
					// take that Node at that index, and make note of it

					// Get Node to duplicate
					Node toDuplicate = finalPath [copyIndex];

					// check if reservation can be made at this node at this time
					if (toDuplicate.canPlaceReservation (_timer)) {

						// If lookBack is 1, then just add duplicate.
						if (tempLookBack == 1)
							tempFinalPath.Add (toDuplicate);
						// Else, look at the precise index and add over there
						else if (tempLookBack > 1)
							tempFinalPath.Insert (copyIndex + 1, toDuplicate);


						toDuplicate.placeReservation (_timer, name);

						_timer++;
						indx++;
					}


					// If not, continuously backtrack in the finalPath array
					// until a node is found that allows for the agent to stay there
					// for this time reservation
					else if (!toDuplicate.canPlaceReservation (_timer)) 
					{
						_timer++;

					}


					// Now, see if it is possible to place a reservation at pathingNode
					// at the newly incremented time
					if (_pathNode.canPlaceReservation (_timer)) 
					{

						_pathNode.placeReservation (_timer, name);

						tempFinalPath.Add (_pathNode);

						_timer++;

						reserveConflict = false;

					}

					safetyCheck++;
				}
				
			}

			// Increment index, to keep track of where in finalPath we currently are
			indx++;
		}

		// Lastly, set the final path equal to the tempFinalPath, that has taken reservations into account
		finalPath = tempFinalPath;

	}
	/// <summary>
	/// Calculates the distance between parameters p_Node1 and p_Node2
	/// </summary>
	/// <param name="p_Node1">P node1.</param>
	/// <param name="p_Node2">P node2.</param>
	private int calculateDistance(Node p_Node1, Node p_Node2)
	{
		// Get the position of each node to perform calculations on
		Vector3 p_Node1Pos = p_Node1.GetComponent<Transform> ().position;
		Vector3 p_Node2Pos = p_Node2.GetComponent<Transform> ().position;

		// Get the magnitude of the vector between the two points
		float _distance = Vector3.Magnitude (p_Node1Pos - p_Node2Pos);

		// Return the floor of that magnitude
		return (int) Mathf.Floor (_distance);
	}

	/// <summary>
	/// Updates currentNode and nextNode attributes for agent to move between
	/// </summary>
	public void traversePath()
	{


		// Get index of currentNode agent is at
		traversePathIndex++;
		//int _currentIndex = finalPath.IndexOf(currentNode);
	
		// If at the end of the path, halt traversal
		// Tell Agent it is adjacent to either Plaque or Professor
		if (traversePathIndex == finalPath.Count-1) 
		{ 
			isInMotion = false;
			_Agent.isAdjacent();
			traversePathIndex = -2;
			return;
		}
			
			
		// Get index of next node to traverse through by incrementing _currentIndex
		nextNode = finalPath[traversePathIndex+1];
	
		// Path correction algorithm, that makes sure every successive node is a neighbour to both the previous node
		// and the node two steps ahead.
		// If not, a node matching that criteria will be placed in between to remedy
		canMove = true;

	}
		

	/// <summary>
	/// Move this agent instance
	/// </summary>
	public void move()
	{
		// Check if the next node is Occupied, and if so, handle accordingly
		checkIfOccupied ();


		
		Vector3 _newPos = nextNode.GetComponent<Transform> ().position;
		Vector3 _oldPos = currentNode.GetComponent<Transform> ().position;

		transform.position = Vector3.MoveTowards (transform.position, _newPos, moveSpeed * Time.deltaTime);
		float _distanceToNew = Vector3.Magnitude (transform.position - _newPos);
		float _distanceToOld = Vector3.Magnitude (transform.position - _oldPos);

		// If distance between agent and current node exceeds threshold, currentNode no longer occupied
		if (_distanceToOld >= 1f) 
		{
			currentNode.isOccupied = false;
			currentNode.Occupant = null;
		}

		// If distance between agent and next node is beneath threshold, next node is occupied
		// set current node to next node
		// set canMove to false, so that traversePath() will be called in the next frame
		if (_distanceToNew <= 0.2f) 
		{
			canMove = false;

			atNextNode = true;

			int _nextIndex = finalPath.IndexOf(nextNode);
			currentNode = finalPath [_nextIndex];
			currentNode.isOccupied = true;
			currentNode.Occupant = GetComponent<Agent> ();
		}
	}

	/// <summary>
	/// Checks if this Pathing instance's nextNode is occupied
	/// if the node is occupied by an Agent that is currently Idling,
	/// then kindly ask that agent to step aside
	/// </summary>
	private void checkIfOccupied()
	{
		// If the nextNode does not have an Occupant, return immediately
		if (nextNode.Occupant == null)
		{ 
			return;
		}

		Agent.AgentState _nextOccupantState = nextNode.Occupant._myState;
			
		// If the nextNode is occupied, and the occupant is currently IDLING
		if (nextNode.isOccupied && (_nextOccupantState == Agent.AgentState.IDLING)) {
			// Then kindly ask occupant to step aside to one of the neighbours of its currentNode
			nextNode.Occupant.stepAside ();
		} 
	}


	// Update is called once per frame
	void Update () {

		/*
		if (isInMotion) {
			if (!canMove) { traversePath (); }
			if (canMove) {	move (); }
		}*/
	}
}

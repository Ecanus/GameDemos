using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Game clock. Increments the game's 'time' only when every single agent has arrived at the successive node
/// in its completed path.
/// This is to standardise reservation times and allow group pathfinding to occur
/// </summary>
public class GameClock : MonoBehaviour {

	/// <summary>
	/// Array of all agents in the scene
	/// </summary>
	public static List<Pathing> PathingAgents;

	public static int _Time;


	private int _agentsCount;

	// Use this for initialization
	void Awake () {

		_Time = 0;

		PathingAgents = new List<Pathing> ();
	}


	/// <summary>
	/// Move the game's clock by one game second (the time it takes to get EVERY non-Idle agent to the next node
	/// in its path)
	/// </summary>
	private void passTime()
	{

		// Loop through every pathing agent in the scene
		foreach(Pathing pathingAgent in PathingAgents)
		{
			if (pathingAgent.atNextNode) continue;

			// If its path has finished being made then:
			if (pathingAgent.isInMotion) 
			{ 
				if (!pathingAgent.canMove) { pathingAgent.traversePath (); }
				if (pathingAgent.canMove) {	pathingAgent.move (); }
			}
			
		}
	}

	/// <summary>
	/// Checks the agents motion, and only allows time to pass when every agent has arrived at its destination
	/// </summary>
	private void checkAgentsMotion()
	{

		// Get the current number of agents in the scene
		_agentsCount = PathingAgents.Count;

		// Initialise value to count for the number of agents that have arrived at their successive node in their path
		int haltedAgentsCheck = 0;

		// Loop through every agent in the scene
		foreach(Pathing pathingAgent in PathingAgents)
		{
			// Get the agent component of this Pathing instance
			Agent agent = pathingAgent.GetComponent<Agent> ();

			// If the agent is not IDLE or IDLING, then ensure it has arrived at a node in its path
			bool agentIsActive = !(agent._myState == Agent.AgentState.IDLE) && !(agent._myState == Agent.AgentState.IDLING);

			if (agentIsActive && pathingAgent.isInMotion) 
			{
				// Only look for those pathing agents that are have finished calculating their path
				// and have reached the successive node in that path
				if (pathingAgent.atNextNode) 
				{
					// If that is the case, increment the counter of all halted agents
					haltedAgentsCheck++;
				}
				
			}
		}

	
		// If number of agents at next node is equal to number of agents in scene
		// then increment time, and clear the atNextNode attribute of every agent in the scene
		if (haltedAgentsCheck == _agentsCount) 
		{
			clearNextNodeAttribute ();
		}
	}

	/// <summary>
	/// Clears the next node attribute of every agent in scene,
	/// and then increments the _Time attribute once
	/// </summary>
	private void clearNextNodeAttribute()
	{
		_Time++;

		foreach(Pathing pathingAgent in PathingAgents)
		{
			pathingAgent.atNextNode = false;
		}

		//Debug.Log ("CLeared all atNextNode values");
	}
		

	// Update is called once per frame
	void Update () {

		checkAgentsMotion();
		passTime ();
	}
}

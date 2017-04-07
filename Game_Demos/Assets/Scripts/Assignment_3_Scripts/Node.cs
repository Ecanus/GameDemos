using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour {


	public enum NodeType {Default, Professor, Plaque};
	public NodeType _NodeType;


	public enum ProfessorInfo {None, Prof_1, Prof_2, Prof_3, Prof_4, Prof_5, Prof_6};
	public ProfessorInfo _ProfessorInfo;

	#region Occupation Attributes
	public bool isOccupied;

	public Agent Occupant;

	#endregion


	/// <summary>
	/// The g cost of this instance
	/// </summary>
	public int gCost;

	/// <summary>
	/// The h cost of this instance
	/// </summary>
	public int hCost;

	/// <summary>
	/// The parent node of this instance
	/// </summary>
	private Node parent;

	/// <summary>
	/// The sister node, which is additionally excluded in destination creation for agent
	/// </summary>
	public Node sister;

	[SerializeField]
	private Dictionary<int, string> reservations;

	#region Neighbours
	[SerializeField]
	private int _NeighboursCount;

	/// <summary>
	/// List of Neighbours that Node instance may have
	/// </summary>
	private List<Node> _Neighbours;

	/// <summary>
	/// Neighbours a Node may have. Up to 8
	/// </summary>
	[SerializeField]
	private Node _Neighbour1;
	[SerializeField]
	private Node _Neighbour2;
	[SerializeField]
	private Node _Neighbour3;
	[SerializeField]
	private Node _Neighbour4;
	[SerializeField]
	private Node _Neighbour5;
	[SerializeField]
	private Node _Neighbour6;
	[SerializeField]
	private Node _Neighbour7;
	[SerializeField]
	private Node _Neighbour8;

	#endregion


	// Use this for initialization
	void Awake () {

		isOccupied = false;

		reservations = new Dictionary<int, string> ();

		setNodeType ();
		setNeighbours ();

		makeClear ();

	}


	public int fCost
	{
		get
		{
			return gCost + hCost;
		}
	}



	/// <summary>
	/// Places a reservation in this node instance's reservation attribute
	/// </summary>
	public void placeReservation(int p_ReserveTime, string p_Name)
	{
		reservations.Add (p_ReserveTime, p_Name);
	}

	/// <summary>
	/// Places a reservation in this node instance's reservation attribute
	/// returns true if parameter can be successfully added, else false
	/// </summary>
	public bool canPlaceReservation(int p_BookingTime)
	{

		if (reservations.ContainsKey (p_BookingTime)) {
			return false;
		} 
		else {	
			return true;
		}

	}

	public string getReservation(int p_ReserveTime)
	{
		return "<" + p_ReserveTime + ", " + reservations [p_ReserveTime] + ">";
	}

	/// <summary>
	/// Instantiates the List of Neighbouring nodes for this instance.
	/// </summary>
	private void setNeighbours()
	{
		_Neighbours = new List<Node> ();
		switch (_NeighboursCount) 
		{
		case 1:
			_Neighbours.Add (_Neighbour1);
			break;
		case 2:
			_Neighbours.Add (_Neighbour1);
			_Neighbours.Add (_Neighbour2);
			break;
		case 3:
			_Neighbours.Add (_Neighbour1);
			_Neighbours.Add (_Neighbour2);
			_Neighbours.Add (_Neighbour3);
			break;
		case 4:
			_Neighbours.Add (_Neighbour1);
			_Neighbours.Add (_Neighbour2);
			_Neighbours.Add (_Neighbour3);
			_Neighbours.Add (_Neighbour4);
			break;
		case 5:
			_Neighbours.Add (_Neighbour1);
			_Neighbours.Add (_Neighbour2);
			_Neighbours.Add (_Neighbour3);
			_Neighbours.Add (_Neighbour4);
			_Neighbours.Add (_Neighbour5);
			break;
		case 6:
			_Neighbours.Add (_Neighbour1);
			_Neighbours.Add (_Neighbour2);
			_Neighbours.Add (_Neighbour3);
			_Neighbours.Add (_Neighbour4);
			_Neighbours.Add (_Neighbour5);
			_Neighbours.Add (_Neighbour6);
			break;
		case 7:
			_Neighbours.Add (_Neighbour1);
			_Neighbours.Add (_Neighbour2);
			_Neighbours.Add (_Neighbour3);
			_Neighbours.Add (_Neighbour4);
			_Neighbours.Add (_Neighbour5);
			_Neighbours.Add (_Neighbour6);
			_Neighbours.Add (_Neighbour7);
			break;
		case 8:
			_Neighbours.Add (_Neighbour1);
			_Neighbours.Add (_Neighbour2);
			_Neighbours.Add (_Neighbour3);
			_Neighbours.Add (_Neighbour4);
			_Neighbours.Add (_Neighbour5);
			_Neighbours.Add (_Neighbour6);
			_Neighbours.Add (_Neighbour7);
			_Neighbours.Add (_Neighbour8);
			break;
		default:
			break;

		}
			
	}

	public Node getUnoccupiedNeighbour(int p_Attempt)
	{
		Node toReturn = null;
		bool foundUnoccupied = false;

		if (!_Neighbours [p_Attempt].isOccupied) 
		{
			toReturn = _Neighbours [p_Attempt];
			foundUnoccupied = true;
		}
			
		/*
		foreach (Node neighbour in _Neighbours) 
		{
			if (!neighbour.isOccupied) 
			{
				toReturn = neighbour;
				foundUnoccupied = true;
				break;
			}
		}*/


		if (foundUnoccupied)
			return toReturn;
		else 
			return this;
		
	}
	/// <summary>
	/// Sets the type of the node.
	/// </summary>
	private void setNodeType()
	{
		if (name.Contains ("PlaqueNode")) {
			_NodeType = NodeType.Plaque;
		} 
		else if (name.Contains ("Professor")) {
			_NodeType = NodeType.Professor;
		} 
		else {
			_NodeType = NodeType.Default;
		}
	}

	private void makeClear()
	{
		SpriteRenderer _sprite = GetComponent<SpriteRenderer> ();
		_sprite.color = Color.clear;
	}

	/// <summary>
	/// Gets the info this node contains
	/// </summary>
	/// <returns>The info.</returns>
	public int getInfo()
	{
		return (int)_ProfessorInfo;
	}

	/// <summary>
	/// Checks the that parameter is neighbour of this instance of node
	/// </summary>
	/// <returns><c>true</c>, if is neighbour was checked, <c>false</c> otherwise.</returns>
	/// <param name="p_Node">P node.</param>
	public bool checkIsNeighbour(Node p_Node)
	{
		return _Neighbours.Contains (p_Node);
	}

	/// <summary>
	/// Gets the common neighbour between this instance and the parameter p_Node
	/// </summary>
	/// <returns><c>true</c>, if common neighbour was gotten, <c>false</c> otherwise.</returns>
	/// <param name="p_Node">P node.</param>
	public Node getCommonNeighbour(Node p_Node)
	{
		Node toReturn = null;
		bool commonNeighbourFound = false;

		foreach (Node n in _Neighbours) 
		{
			// Find first node that is common neighbour between this instance and parameter
			// then break
			if (this.checkIsNeighbour (n) && p_Node.checkIsNeighbour (n)) 
			{
				toReturn = n;
				commonNeighbourFound = true;
				break;
			}
		}

		// if search successful, return common node
		if (commonNeighbourFound)
			return toReturn;
		// else, return this node
		else 
			return this;
	}
	/// <summary>
	/// Gets the neighbours list of this instance
	/// </summary>
	/// <returns>The neighbours.</returns>
	public List<Node> getNeighbours()
	{
		return _Neighbours;
	}

	public void setParent(Node p_Parent)
	{
		parent = p_Parent;
	}

	public Node getParent()
	{
		return parent;
	}

	public void isOpen()
	{
		SpriteRenderer _sprite = GetComponent<SpriteRenderer> ();
		_sprite.color = Color.green;
	}

	public void isClosed()
	{
		SpriteRenderer _sprite = GetComponent<SpriteRenderer> ();
		_sprite.color = Color.red;
	}

	public void isPathNode()
	{
		SpriteRenderer _sprite = GetComponent<SpriteRenderer> ();
		_sprite.color = Color.yellow;
	}
}

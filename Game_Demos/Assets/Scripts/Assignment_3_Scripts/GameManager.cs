using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	#region Default Node Attributes

	public List<Node> allDefaultNodes;

	[SerializeField]
	private Transform Row_1;
	[SerializeField]
	private Transform Row_2;
	[SerializeField]
	private Transform Row_3;
	[SerializeField]
	private Transform Row_4;
	[SerializeField]
	private Transform Row_5;
	[SerializeField]
	private Transform Row_6;

	#endregion

	#region Plaque Node Attributes
	/// <summary>
	/// List of Nodes that are 'adjacent' to Plaques.
	/// </summary>
	public Node[] PlaqueNodes;

	[SerializeField]
	private Node _Plaque1;
	[SerializeField]
	private Node _Plaque2;
	[SerializeField]
	private Node _Plaque3;
	[SerializeField]
	private Node _Plaque4;
	[SerializeField]
	private Node _Plaque5;
	[SerializeField]
	private Node _Plaque6;
	[SerializeField]
	private Node _Plaque7;
	[SerializeField]
	private Node _Plaque8;

	#endregion

	#region Professor Node Attributes
	[SerializeField]
	private Transform _Room1;
	private Node[] _Room1Nodes;
	[SerializeField]
	private Transform _Room2;
	private Node[] _Room2Nodes;
	[SerializeField]
	private Transform _Room3;
	private Node[] _Room3Nodes;
	[SerializeField]
	private Transform _Room4;
	private Node[] _Room4Nodes;
	[SerializeField]
	private Transform _Room5;
	private Node[] _Room5Nodes;
	[SerializeField]
	private Transform _Room6;
	private Node[] _Room6Nodes;
	#endregion


	// Use this for initialization
	void Awake () {

		instantiatePlaqueNodes();
		instantiateRoomArrays ();


		instantiateProfessorNodes();
		instantiateDefaultNodes ();
	}


	/// <summary>
	/// Gets the prof node array corresponding with parameter
	/// </summary>
	/// <returns>The prof node array.</returns>
	/// <param name="p_ProfNumber">P prof number.</param>
	public Node[] getProfNodeArray(int p_ProfNumber)
	{
		Node[] _toReturn = new Node[3];

		switch (p_ProfNumber) 
		{
		case 1:
			_toReturn = _Room1Nodes;
			break;
		case 2:
			_toReturn = _Room2Nodes;
			break;
		case 3:
			_toReturn = _Room3Nodes;
			break;
		case 4:
			_toReturn = _Room4Nodes;
			break;
		case 5:
			_toReturn = _Room5Nodes;
			break;
		case 6:
			_toReturn = _Room6Nodes;
			break;
		default:
			break;
		}

		return _toReturn;
	}


	private void instantiateDefaultNodes()
	{
		allDefaultNodes = new List<Node> ();

		foreach (Transform child_1 in Row_1) 
		{
			allDefaultNodes.Add (child_1.GetComponent<Node> ());
		}

		foreach (Transform child_2 in Row_2) 
		{
			allDefaultNodes.Add (child_2.GetComponent<Node> ());
		}

		foreach (Transform child_3 in Row_3) 
		{
			allDefaultNodes.Add (child_3.GetComponent<Node> ());
		}
		foreach (Transform child_4 in Row_4) 
		{
			allDefaultNodes.Add (child_4.GetComponent<Node> ());
		}
		foreach (Transform child_5 in Row_5) 
		{
			allDefaultNodes.Add (child_5.GetComponent<Node> ());
		}
		foreach (Transform child_6 in Row_6) 
		{
			allDefaultNodes.Add (child_6.GetComponent<Node> ());
		}
	}

	/// <summary>
	/// Instantiates the plaque nodes.
	/// </summary>
	private void instantiatePlaqueNodes()
	{
		PlaqueNodes = new Node[8];

		PlaqueNodes[0] = _Plaque1;
		PlaqueNodes[1] = _Plaque2;
		PlaqueNodes[2] = _Plaque3;
		PlaqueNodes[3] = _Plaque4;
		PlaqueNodes[4] = _Plaque5;
		PlaqueNodes[5] = _Plaque6;
		PlaqueNodes[6] = _Plaque7;
		PlaqueNodes[7] = _Plaque8;
	}

	/// <summary>
	/// Instantiates the professor nodes.
	/// </summary>
	private void instantiateProfessorNodes()
	{

		Transform temp;
		Node tempNode;

		// Room 1 --------
		temp = _Room1.GetChild (1);
		tempNode = temp.GetComponent<Node> ();
		_Room1Nodes [0] = tempNode;
		temp = _Room1.GetChild (2);
		tempNode = temp.GetComponent<Node> ();
		_Room1Nodes [1] = tempNode;
		temp = _Room1.GetChild (3);
		tempNode = temp.GetComponent<Node> ();
		_Room1Nodes [2] = tempNode;

		// Room 2 --------
		temp = _Room2.GetChild (1);
		tempNode = temp.GetComponent<Node> ();
		_Room2Nodes [0] = tempNode;
		temp = _Room2.GetChild (2);
		tempNode = temp.GetComponent<Node> ();
		_Room2Nodes [1] = tempNode;
		temp = _Room2.GetChild (3);
		tempNode = temp.GetComponent<Node> ();
		_Room2Nodes [2] = tempNode;

		// Room 3 --------
		temp = _Room3.GetChild (1);
		tempNode = temp.GetComponent<Node> ();
		_Room3Nodes [0] = tempNode;
		temp = _Room3.GetChild (2);
		tempNode = temp.GetComponent<Node> ();
		_Room3Nodes [1] = tempNode;
		temp = _Room3.GetChild (3);
		tempNode = temp.GetComponent<Node> ();
		_Room3Nodes [2] = tempNode;

		// Room 4 --------
		temp = _Room4.GetChild (1);
		tempNode = temp.GetComponent<Node> ();
		_Room4Nodes [0] = tempNode;
		temp = _Room4.GetChild (2);
		tempNode = temp.GetComponent<Node> ();
		_Room4Nodes [1] = tempNode;
		temp = _Room4.GetChild (3);
		tempNode = temp.GetComponent<Node> ();
		_Room4Nodes [2] = tempNode;

		// Room 5 --------
		temp = _Room5.GetChild (1);
		tempNode = temp.GetComponent<Node> ();
		_Room5Nodes [0] = tempNode;
		temp = _Room5.GetChild (2);
		tempNode = temp.GetComponent<Node> ();
		_Room5Nodes [1] = tempNode;
		temp = _Room5.GetChild (3);
		tempNode = temp.GetComponent<Node> ();
		_Room5Nodes [2] = tempNode;

		// Room 6 --------
		temp = _Room6.GetChild (1);
		tempNode = temp.GetComponent<Node> ();
		_Room6Nodes [0] = tempNode;
		temp = _Room6.GetChild (2);
		tempNode = temp.GetComponent<Node> ();
		_Room6Nodes [1] = tempNode;
		temp = _Room6.GetChild (3);
		tempNode = temp.GetComponent<Node> ();
		_Room6Nodes [2] = tempNode;

	}

	private void instantiateRoomArrays()
	{

		_Room1Nodes = new Node[3];
		_Room2Nodes = new Node[3];
		_Room3Nodes = new Node[3];
		_Room4Nodes = new Node[3];
		_Room5Nodes = new Node[3];
		_Room6Nodes = new Node[3];

	}
	// Update is called once per frame
	void Update () {
		
	}
}

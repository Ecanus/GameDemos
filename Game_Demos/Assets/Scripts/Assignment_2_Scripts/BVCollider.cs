using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Unused in this Assignment
/// </summary>
public class BVCollider : MonoBehaviour {

	/// <summary>
	/// Node type, modifies collider behaviour accordingly
	/// 
	/// Large BV Nodes encompass up to four pieces of terrain
	/// Medium BV Nodes encompass up to two pieces of terrain
	/// 
	/// </summary>
	public enum NodeType {Large, Medium};
	private NodeType a_BVNodeType;


	/// <summary>
	/// The three Medium BV node siblings
	/// </summary>
	public Transform[] a_NodeSiblings;

	//TODO Make this an array of the Terrain Pieces under the effects of this Node
	public Transform a_TerrainPiece;


	//TODO Make this an array of all projectiles created on screen at any time
	/// <summary>
	/// Projectile whos bounds are checked against
	/// </summary>
	[SerializeField]
	private Transform a_Projectile;

	//private Transform a_CollisionPointPrefab;
	//private Transform a_CollisionPoint;


	private Bounds a_Bounds;
	private Bounds a_ProjectileBounds;

	/// <summary>
	/// Checks the bounds of BVNode to see if a projectile has come into contact with it
	/// </summary>
	public void checkBounds(Transform p_Projectile)
	{
		a_ProjectileBounds = p_Projectile.GetComponent<Renderer> ().bounds;
		if (!a_Bounds.Intersects (a_ProjectileBounds)) { return; }

		switch (a_BVNodeType) 
		{

		// If a Large BV node, render it inactive, and actuate the three specified medium BV nodes
		case (NodeType.Large):
			nodeCollisionLarge();
			break;
		
		// If a Medium BV node, render it and its siblings inactive. Actuate bounds for all encompassed
		// terrain.
		case (NodeType.Medium):
			nodeCollisionMedium();
			break;

		default:
			break;
		}
			
	}

	/// <summary>
	/// Sets all the Medium BV node siblings of this Large BV node to true.
	/// Sets itself to inactive
	/// </summary>
	private void nodeCollisionLarge()
	{
		foreach (Transform b_Sibling in a_NodeSiblings) 
		{
			b_Sibling.gameObject.SetActive (true);
		}

		gameObject.SetActive (false);
	}

	private void nodeCollisionMedium()
	{
		foreach (Transform b_Sibling in a_NodeSiblings) 
		{
			b_Sibling.gameObject.SetActive (false);
		}
	}

	private void instantiateParentNode()
	{
		if (gameObject.CompareTag ("LargeBVNode")) { a_BVNodeType = NodeType.Large; }
		if (gameObject.CompareTag ("MediumBVNode")) { a_BVNodeType = NodeType.Medium; }
	}

	/// <summary>
	/// Populates a_NodeSiblings array, and renders each sibling inactive until called later
	/// </summary>
	private void instantiateSiblings()
	{
		// If a Large BVNode, populate array. 
		if (gameObject.CompareTag("LargeBVNode")) {
			a_NodeSiblings = new Transform[3];
			a_NodeSiblings [0] = GameObject.Find (name + "Sibling_0").GetComponent<Transform>();
			a_NodeSiblings [0].gameObject.SetActive (false);

			a_NodeSiblings [1] = GameObject.Find (name + "Sibling_1").GetComponent<Transform>();
			a_NodeSiblings [1].gameObject.SetActive (false);

			a_NodeSiblings [2] = GameObject.Find (name + "Sibling_2").GetComponent<Transform>();
			a_NodeSiblings [2].gameObject.SetActive (false);

			a_NodeSiblings [0].GetComponent<BVCollider> ().a_NodeSiblings = a_NodeSiblings;
			a_NodeSiblings [1].GetComponent<BVCollider> ().a_NodeSiblings = a_NodeSiblings;
			a_NodeSiblings [2].GetComponent<BVCollider> ().a_NodeSiblings = a_NodeSiblings;
		} 
	}

	/// <summary>
	/// Sets the type of the node so appropriate bounds checking can occur
	/// </summary>
	/// <param name="p_BVNodeType">P BV node type.</param>
	public void setNodeType(NodeType p_BVNodeType)
	{
		a_BVNodeType = p_BVNodeType;
	}

	// Use this for initialization
	void Start () {
		// Get the bounds of this BV node using the Renderer
		a_Bounds = GetComponent<Renderer> ().bounds;


		// Assigns value of a_ParentNode Transform based on the current node's type and BVNodeType
		instantiateParentNode();

		// Populates a_NodeSiblings array, and renders them inactive until called later
		instantiateSiblings();
	}
	
	// Update is called once per frame
	void Update () {



		// BV Node checks its bounds against the potential of a projectile intersecting with it
		// if successful, responds accordingly 
		checkBounds(a_Projectile);
	}
}

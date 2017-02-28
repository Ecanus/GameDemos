using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MazeGenerator : MonoBehaviour {

	/// <summary>
	/// The height of the maze. Derived from plane Z coordinate
	/// </summary>
	private float mazeHeight;


	/// <summary>
	/// The width of the maze. Derived from plane X coordinate
	/// </summary>
	private float mazeWidth;

	/// <summary>
	/// Block prefab used to populate plane
	/// </summary>
	[SerializeField]
	private Transform a_block;

	/// <summary>
	/// Block instance to assign values to each instantiation
	/// </summary>
	[SerializeField]
	private Transform a_blockInstance;

	[SerializeField]
	private Transform a_BlocksObject;

	/// <summary>
	/// Pathfinder of maze, used to create unicursal path
	/// </summary>
	[SerializeField]
	private Pathfinder a_Pathfinder;

	/// <summary>
	/// Sets the position of the plane
	/// </summary>
	private void setPlanePosition()
	{
		Vector3 _position = transform.position;
		_position.x = 4.5f;
		_position.y = 0.0f;
		_position.z = 4.5f;
		transform.position = _position;
	}


	/// <summary>
	/// Populates plane with Block prefabs
	/// </summary>
	private void generateBlocks()
	{
		Vector3 t_Vector = new Vector3();

		for (int rowNum = 0; rowNum < mazeHeight; rowNum++) {
			for (int colNum = 0; colNum < mazeWidth; colNum++) {

				//Instantiate new block using prefab
				t_Vector.x = colNum;
				t_Vector.y = 0;
				t_Vector.z = rowNum;

				a_blockInstance = Instantiate(a_block, t_Vector, Quaternion.identity);
				a_blockInstance.gameObject.name = "(" + rowNum + ", " + colNum + ")";

				// Set mazePosition coords for this Block
				a_blockInstance.gameObject.GetComponent<Block>().setMazePosition(rowNum, colNum);

				// Place this Block into the Pathfinder array
				a_Pathfinder.insertBlock(a_blockInstance.GetComponent<Block>(), rowNum, colNum);

				a_blockInstance.GetComponent<Transform>().SetParent(a_BlocksObject);

			}
		}
	}

	// Use this for initialization
	void Start () {

		// Set maze scale values
		mazeHeight = transform.localScale.z * 10;
		mazeWidth = transform.localScale.x * 10;

		// Instantiate Pathfinder attributes
		a_Pathfinder.setBlocks ((int)mazeHeight, (int)mazeWidth);
	

		// Instantiate default positions and gameobjects
		//setPlanePosition();
		generateBlocks ();
		a_Pathfinder.pickStart();

	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Block class. 
/// 
/// MazePosition of Block uses X and Z coordinates in World Space
/// </summary>
public class Block : MonoBehaviour {

	public enum CornerType {BottomRight, BottomLeft, TopRight, TopLeft, NotCorner};
	public CornerType a_CornerType;

	public Block[] a_BranchPath;

	[SerializeField]
	private float a_MazePositionX;
	[SerializeField]
	private float a_MazePositionY;


	/// <summary>
	/// Sets the values for class attributes, a_MazePosition
	/// </summary>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	public void setMazePosition(float x, float y)
	{
		a_MazePositionX = x;
		a_MazePositionY = y;
	}

	public int getMazePositionX()
	{
		return (int) a_MazePositionX;
	}

	public int getMazePositionY()
	{
		return (int) a_MazePositionY;
	}
		
	public void highlight()
	{
		GetComponent<Renderer> ().sharedMaterial = MaterialManager.gameMaterials [0];
	}

	public void highlightRed()
	{
		GetComponent<Renderer> ().sharedMaterial = MaterialManager.gameMaterials [1];
	}

	public void highlightPurple()
	{
		GetComponent<Renderer> ().sharedMaterial = MaterialManager.gameMaterials [2];
	}

	public bool checkBranchLateral(int mazeWidth)
	{
		bool elligibility = false; 

		switch (a_CornerType) 
		{
		case CornerType.BottomRight:
			if (a_MazePositionY < mazeWidth - 10) { elligibility = true; }
			break;
		
		case CornerType.BottomLeft:
			if (a_MazePositionY > 10) { elligibility =  true; }
			break;

		case CornerType.TopLeft:
			if (a_MazePositionY > 10) { elligibility = true; }
			break;

		case CornerType.TopRight:
			if (a_MazePositionY < mazeWidth - 10) { elligibility =  true; }
			break;

		default:
			elligibility = false;
			break;
		}

		return elligibility;
	}

	public bool checkBranchVertical(int mazeHeight)
	{
		bool elligibility = false; 

		switch (a_CornerType) 
		{
		case CornerType.BottomRight:
			if (a_MazePositionX > 10) { elligibility = true; }
			break;

		case CornerType.BottomLeft:
			if (a_MazePositionY > 10) { elligibility =  true; }
			break;

		case CornerType.TopLeft:
			if (a_MazePositionY < mazeHeight - 10) { elligibility = true; }
			break;

		case CornerType.TopRight:
			if (a_MazePositionY < mazeHeight - 10) { elligibility =  true; }
			break;

		default:
			elligibility = false;
			break;
		}

		return elligibility;
	}


	// Use this for initialization
	void Start () {
		a_CornerType = CornerType.NotCorner;
	}
}

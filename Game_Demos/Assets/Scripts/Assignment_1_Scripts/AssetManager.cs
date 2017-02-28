using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetManager : MonoBehaviour {


	[SerializeField]
	private Transform a_Key;

	[SerializeField]
	private Transform a_Door;

	/// <summary>
	/// Places the Final Door and the BoulderSpawn collider
	/// </summary>
	public void placeDoor(Block p_Block)
	{
		Instantiate (a_Door, p_Block.GetComponent<Transform> ().position, Quaternion.Euler(-90,0,0));
	}

	public void placeKey(Block p_Block, float rotationY)
	{
		Instantiate(a_Key, p_Block.GetComponent<Transform> ().position, Quaternion.Euler(90,rotationY,0));
	}

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneViewManager : MonoBehaviour {


	public static Transform groundObject;
	public static Transform goalFlagObject;

	public static void hideObjects()
	{
		foreach(Transform child in groundObject)
		{
			child.GetComponent<MeshRenderer>().enabled = false;
		}

		goalFlagObject.gameObject.SetActive (false);
	}

	public static void showObjects()
	{
		foreach(Transform child in groundObject)
		{
			child.GetComponent<MeshRenderer>().enabled = true;
		}

		goalFlagObject.gameObject.SetActive (true);
	}

	// Use this for initialization
	void Start () {

		groundObject = GameObject.Find ("GroundLevel").GetComponent<Transform>();
		goalFlagObject = GameObject.Find ("GoalFlag").GetComponent<Transform>();
	}

}

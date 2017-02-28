using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneWayManager : MonoBehaviour {


	[SerializeField]
	private Transform a_EntryBoundary;

	[SerializeField]
	private Transform a_ExitBoundary;

	public void sealEntry()
	{
		a_EntryBoundary.GetComponent<BoxCollider> ().enabled = true;
	}

	public void sealExit()
	{
		a_ExitBoundary.GetComponent<BoxCollider> ().enabled = true;
	}
}

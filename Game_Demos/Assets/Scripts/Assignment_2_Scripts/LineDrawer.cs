using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Draws a line between all points in its points list
/// </summary>
public class LineDrawer : MonoBehaviour {

	// Transforms required for specific backtrackings in order to get GOAT outline
	[SerializeField]
	private Transform a_NoseDown;
	[SerializeField]
	private Transform a_Foreleg;
	[SerializeField]
	private Transform a_Hindleg;
	[SerializeField]
	private Transform a_Backfront;


	/// <summary>
	/// List of all points for LineRenderer to connect
	/// </summary>
	[SerializeField]
	private Vector3[] a_PointsList;

	/// <summary>
	/// The line which will be rendered between all points in list
	/// </summary>
	[SerializeField]
	private LineRenderer a_Line;

	/// <summary>
	/// Bool containing lengthy checks for backtracking
	/// </summary>
	private bool a_JointCheckAll;
	private bool a_JointCheckSome;

	/// <summary>
	/// Populates the list of points for lineRenderer to connect
	/// </summary>
	private void populateList()
	{
		// Instantiate a counter
		int x = 0;

		// Instantiate the list of points to the number of points, plus an additional 7 accounting for 
		// backtrackings
		a_PointsList = new Vector3[transform.childCount+7];

		// Loop through all children and add them to list of points for lineRenderer to connect
		foreach (Transform child in transform) 
		{
			// Get the position of the child, and put it in the Points list
			a_PointsList[x] = child.position;

			// Insert backtrack point if this child's name is one of the backtrack points specified
			// in getFirstJointCheck
			// increment the counter indedx
			if (getFirstJointCheck(child.name)) 
			{
				x++;
				insertFirstJoint (child, x);
			}


			// Insert this instance a second time into points list
			// only if Horn or Beard joint
			// Increment the counter index
			if (getSecondJointCheck(child.name)) 
			{
				x++;
				insertSecondJoint(child, x);
			}

			x++;
		}

		// Set the last element in the list to the first element so that the line completes the cycle
		a_PointsList [transform.childCount+6] = a_PointsList[0];

		// Give the line the values it needs to render itself
		a_Line.numPositions = a_PointsList.Length;
		a_Line.widthMultiplier = 0.03f;
		a_Line.SetPositions (a_PointsList);
	}

	/// <summary>
	/// Inserts the Transform into the list of points at the given index
	/// if it fits certain criteria
	/// </summary>
	/// <param name="p_CurrentPoint">P current point.</param>
	/// <param name="p_ListIndex">P list index.</param>
	private void insertFirstJoint(Transform p_CurrentPoint, int p_ListIndex)
	{
		if (p_CurrentPoint.name == "Neck") { a_PointsList [p_ListIndex] = a_NoseDown.position;}
		if (p_CurrentPoint.name == "Foreleg_Foot") { a_PointsList [p_ListIndex] = a_Foreleg.position;}
		if (p_CurrentPoint.name == "Hindleg_Foot") { a_PointsList [p_ListIndex] = a_Hindleg.position;}
		if (p_CurrentPoint.name == "Forehead") { a_PointsList [p_ListIndex] = a_Backfront.position;}
		
	}

	/// <summary>
	/// Inserts the Transform into the list of points at the given index 
	/// if it fits certain criteria
	/// </summary>
	/// <param name="p_CurrentPoint">P current point.</param>
	/// <param name="p_ListIndex">P list index.</param>
	private void insertSecondJoint(Transform p_CurrentPoint, int p_ListIndex)
	{
		if (p_CurrentPoint.name == "Neck") { a_PointsList [p_ListIndex] = p_CurrentPoint.position;}
		if (p_CurrentPoint.name == "Forehead") { a_PointsList [p_ListIndex] = p_CurrentPoint.position;}

	}

	/// <summary>
	/// Returns the value of the check against the string parameter
	/// </summary>
	/// <returns><c>true</c>, if first joint check was gotten, <c>false</c> otherwise.</returns>
	/// <param name="p_Name">P name.</param>
	private bool getFirstJointCheck(string p_Name)
	{
		return (p_Name == "Neck" || p_Name == "Foreleg_Foot" || p_Name == "Hindleg_Foot" || p_Name == "Forehead");
	}

	/// <summary>
	/// Returns the value of the check against the string parameter
	/// </summary>
	/// <returns><c>true</c>, if second joint check was gotten, <c>false</c> otherwise.</returns>
	/// <param name="p_Name">P name.</param>
	private bool getSecondJointCheck(string p_Name)
	{
		return (p_Name == "Neck" ||  p_Name == "Forehead");
	}
		
	
	// Update is called once per frame
	void Update () {

		// Call in update to constantly re-render after point position changes 
		populateList ();
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Creates the actual terrain in start up frames of the game
/// </summary>
public class TerrainCreator : MonoBehaviour {

	/// <summary>
	/// Object containing created points
	/// </summary>
	[SerializeField]
	private Transform a_PointsObject;

	/// <summary>
	/// A terrain drawer.
	/// </summary>
	[SerializeField]
	private SmartTerrain a_TerrainDrawer;

	/// <summary>
	/// LinkedList for the points to be traversed
	/// </summary>
	private LinkedList<Transform> a_PointsList;

	/// <summary>
	/// LinkedList Nodes needed to keep track of place in the pointsList
	/// </summary>
	private LinkedListNode<Transform> currentPoint;
	private LinkedListNode<Transform> nextPoint;
	private LinkedListNode<Transform> midPoint;

	// Number of times to bisect before incrementing the currentPoint
	[SerializeField]
	private int a_BisectionCount;

	// The initial bounding points
	[SerializeField]
	private Transform a_FirstBound;
	[SerializeField]
	private Transform a_SecondBound;
	[SerializeField]
	private Vector3 a_SpawnPosition;


	/// <summary>
	/// The rotation of the terrain piece
	/// </summary>
	[SerializeField]
	private float a_RotationInDegrees;
	private float a_DegreeSign;

	/// <summary>
	/// The point prefab and the point at which it is called to spawn
	/// </summary>
	[SerializeField]
	private Transform a_PointPrefab;
	private Transform a_SpawnPoint;

	/// <summary>
	/// Vectors holding the line between two points, and the magnitude of that line.
	/// </summary>
	[SerializeField]
	private Vector3 a_LineBetween;
	private Vector3 a_LineMagnitude;

	/// <summary>
	/// Floats keeping track of how heigh the point can be randomly placed
	/// </summary>
	private float a_PointHeight;
	private float a_HeightThreshold;

	// Booleans managing states of the TerrainCreator object
	[SerializeField]
	private bool hasFinishedCreation;
	private bool midPointSet;
	private bool pointsConnected;


	// Insert a new point between the currentPoint and its successor
	private void createPointBetween(Transform p_CPoint, Transform p_NPoint)
	{

		// Modify Vector3.Angle sign based on the height of second point
		updateDegreeSign(p_CPoint, p_NPoint);

		// Choose random height to place new point at
		pickRandomHeight();

		// Get the line between the two points
		a_LineBetween.x = p_NPoint.position.x - p_CPoint.position.x;
		a_LineBetween.y = p_NPoint.position.y - p_CPoint.position.y;
		a_LineBetween.z = p_NPoint.position.z - p_CPoint.position.z;

		// Get the midway point between the two points
		a_SpawnPosition = ((p_NPoint.position - p_CPoint.position) * 0.5f);
		a_SpawnPosition += p_CPoint.position;

		// Get the rotation between the two points
		a_RotationInDegrees = Vector3.Angle (Vector3.right, a_LineBetween);

		// Instantiates the new point at the modified spawn posiiton, with the modified Rotation in Degrees
		a_SpawnPoint = Instantiate (a_PointPrefab, a_SpawnPosition, Quaternion.Euler (0, 0, (a_RotationInDegrees * a_DegreeSign)));
		a_SpawnPoint.parent = a_PointsObject;

		// Move the point up by the random height
		a_SpawnPoint.Translate (0, a_PointHeight, 0);

		// Ads this point into the points list and increments the bisection counter
		// used to know when to place the current pointer where
		a_PointsList.AddAfter (currentPoint, a_SpawnPoint); 
		a_BisectionCount++;

		// Only set on first bisection
		if (!midPointSet) 
		{
			setMidPoint (a_SpawnPoint);
			midPointSet = true;
		}

		// Checks for when and where to modify the next point to.
		// behaves according to the bisectioncount
		if (a_BisectionCount == 3) 
		{
			currentPoint = currentPoint.Next.Next;
		}

		else if (a_BisectionCount == 4) 
		{
			currentPoint = a_PointsList.Find(midPoint.Value);
		}

		else if (a_BisectionCount == 6) 
		{
			currentPoint = currentPoint.Next.Next;
		}

		else if (a_BisectionCount == 7) 
		{
			hasFinishedCreation = true;
			a_TerrainDrawer.createTerrainList(a_PointsList);
			currentPoint = a_PointsList.First;
		}

	}

	// Instantiates Terrain to connect between each point
	private void connectPoints()
	{
		if (currentPoint.Next == null) 
		{
			pointsConnected = true;
			BoundsManager.populateBoundsList();
			a_PointsObject.gameObject.SetActive (false);
			return;
		}

		a_TerrainDrawer.placePlane(currentPoint.Value, currentPoint.Next.Value);
		currentPoint = currentPoint.Next;
	}

	/// <summary>
	/// Sets the middle point between the intial bounds
	/// </summary>
	/// <param name="p_Point">P point.</param>
	private void setMidPoint(Transform p_Point)
	{
		midPoint = a_PointsList.Find(p_Point);
	}

	/// <summary>
	/// Picks the random height for Point to be moved by
	/// </summary>
	private void pickRandomHeight()
	{
		a_PointHeight = Random.Range (-a_HeightThreshold, a_HeightThreshold);
	}

	/// <summary>
	/// Updates the degree sign based on height of nextPoint
	/// </summary>
	/// <param name="p_CPoint">P C point.</param>
	/// <param name="p_NPoint">P N point.</param>
	private void updateDegreeSign(Transform p_CPoint, Transform p_NPoint)
	{
		// If the next point is below the current point, we make the degree have a negative sign
		if (p_NPoint.position.y < p_CPoint.position.y) { a_DegreeSign = -1.0f; }
		// Else the sign is to remain positive
		else { a_DegreeSign = 1.0f;}
	}

	// Use this for initialization
	void Start () {

		a_HeightThreshold = 0.2f;
		hasFinishedCreation = false;

		// Add first and second bound to the LinkedList of points
		a_PointsList = new LinkedList<Transform>();
		currentPoint = new LinkedListNode<Transform>(a_FirstBound);
		nextPoint = new LinkedListNode<Transform>(a_SecondBound);

		// Set the current and next points to the intial bounding points
		currentPoint.Value = a_FirstBound;
		nextPoint.Value = a_SecondBound;

		a_PointsList.AddFirst (currentPoint);
		a_PointsList.AddLast (nextPoint);
		
	}
	
	// Update is called once per frame
	void Update () {

		if (!hasFinishedCreation) 
		{
			createPointBetween (currentPoint.Value, currentPoint.Next.Value);
		}

		if (hasFinishedCreation && !pointsConnected) 
		{
			connectPoints ();
		}
			
		
	}
}

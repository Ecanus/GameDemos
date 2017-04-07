using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages behaviour of smart terrain with regards to creation
/// </summary>
public class SmartTerrain : MonoBehaviour {

	/// <summary>
	/// The object with the terrains
	/// </summary>
	[SerializeField]
	private Transform a_TerrainsObject;

	/// <summary>
	/// The list of all Terrains instantiated and placed
	/// </summary>
	//private LinkedList<Transform> a_TerrainList;

	/// <summary>
	/// The prefab of the Terrain
	/// </summary>
	[SerializeField]
	private Transform a_TerrainPrefab;

	/// <summary>
	/// Variable holding on to the most recently instantiated terrain
	/// </summary>
	private Transform a_SpawnTerrain;

	/// <summary>
	/// List which manages the points that make up the 'joints' between terrain
	/// </summary>
	//private LinkedListNode<Transform> currentPoint;


	/// <summary>
	/// Variables for determining magnitude, rotation and position for each terrain piece
	/// </summary>
	private Vector3 a_LineBetween;
	private Vector3 a_TerrainMagnitude;
	private Vector3 a_TerrainPosition;
	private Quaternion a_TerrainRotation;


	/// <summary>
	/// Variables determining rotation and sign of the degree used.
	/// </summary>
	private float a_RotationInDegrees;
	private float a_DegreeSign;

	/// <summary>
	/// Vector.right
	/// </summary>
	private Vector3 a_GroundVector;


	/// <summary>
	/// Creates the terrain list to be populated and then used to place the terrain
	/// </summary>
	/// <param name="p_PointsList">P points list.</param>
	public void createTerrainList(LinkedList<Transform> p_PointsList)
	{
		//a_TerrainList = new LinkedList<Transform> (p_PointsList);
		//currentPoint = a_TerrainList.First;
	}

	/// <summary>
	/// Draw the out the Transforms specified in p_PointsList.
	/// </summary>
	/// <param name="p_PointsList">P points list.</param>
	public void draw(LinkedList<Transform> p_PointsList)
	{
		
		//currentPoint = new LinkedListNode<Transform>(p_PointsList.First.Value);
		placePlane (p_PointsList.First.Value, p_PointsList.First.Next.Value);
	}

	/// <summary>
	/// Creates the line between p_Point1 and p_Point2.
	/// </summary>
	/// <param name="p_Point1">P point1.</param>
	/// <param name="p_Point2">P point2.</param>
	private void createLineBetween(Transform p_Point1, Transform p_Point2)
	{
		a_LineBetween.x = p_Point2.position.x - p_Point1.position.x;
		a_LineBetween.y = p_Point2.position.y - p_Point1.position.y;
		a_LineBetween.z = p_Point2.position.z - p_Point1.position.z;

		a_TerrainMagnitude = new Vector3(a_LineBetween.magnitude,0.1f,1);
	}

	/// <summary>
	/// Updates the degree sign based on the height of pointTwo with relation to pointOne
	/// </summary>
	/// <param name="p_PointOne">P point one.</param>
	/// <param name="p_PointTwo">P point two.</param>
	private void updateDegreeSign(Transform p_PointOne, Transform p_PointTwo)
	{
		if (p_PointTwo.position.y < p_PointOne.position.y) { a_DegreeSign = -1.0f; }
		else { a_DegreeSign = 1.0f;}
	}

	/// <summary>
	/// Places the plane between the two points
	/// </summary>
	/// <param name="p_FirstPoint">P first point.</param>
	/// <param name="p_SecondPoint">P second point.</param>
	public void placePlane(Transform p_FirstPoint, Transform p_SecondPoint )
	{
		createLineBetween (p_FirstPoint, p_SecondPoint);
		updateDegreeSign (p_FirstPoint, p_SecondPoint);


		// Handle position update
		a_TerrainPosition = ((p_SecondPoint.position - p_FirstPoint.position) * 0.5f);
		a_TerrainPosition += p_FirstPoint.position;

		// Handle rotation update
		a_RotationInDegrees = Vector3.Angle (a_GroundVector, a_LineBetween);
		a_TerrainRotation = Quaternion.Euler (0, 0, (a_RotationInDegrees * a_DegreeSign));

		a_SpawnTerrain = Instantiate (a_TerrainPrefab, a_TerrainPosition, a_TerrainRotation);
		a_SpawnTerrain.parent = a_TerrainsObject;

		a_SpawnTerrain.localScale = a_TerrainMagnitude;

	}

	// Use this for initialization
	void Start () {

		a_GroundVector = Vector3.right;
	}
}

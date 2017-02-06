using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoulderSpawnManager : MonoBehaviour {

	public List<Block> a_Path;

	[SerializeField]
	private Transform a_BoulderSpawnCollider;

	[SerializeField]
	private Transform a_Boulder;

	private bool startedSpawning;

	public void boulderSpawn()
	{
		Instantiate (a_Boulder, Vector3.zero, Quaternion.identity);
	}

	public void setTriggerLocation(List<Block> p_Path)
	{
		a_Path = p_Path;

		int middleBlockIndex = (a_Path.Count) / 4;
		Vector3 colliderPos = a_Path [middleBlockIndex].GetComponent<Transform> ().position;
		a_BoulderSpawnCollider.position = colliderPos;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag ("Player") && !startedSpawning) 
		{
			InvokeRepeating ("boulderSpawn", 0f, 5f);
			startedSpawning = true;
		}
	}

	// Use this for initialization
	void Start () {

		startedSpawning = false;
	}
}

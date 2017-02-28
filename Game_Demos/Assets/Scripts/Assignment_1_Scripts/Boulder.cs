using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boulder : MonoBehaviour {

	private List<Block> a_Path;

	[SerializeField]
	public BoulderSpawnManager a_BoulderSpawnManager;

	public bool isActive;

	private Vector3 directionVector;
	private Vector3 current;
	private Vector3 target;

		

	private IEnumerator moveBoulder()
	{

		Vector3 velocity = Vector3.zero;

		for (int x = 0; x < a_Path.Count-1; x++) 
		{
			current = a_Path [x].GetComponent<Transform> ().position;
			target = a_Path [(x + 1)].GetComponent<Transform> ().position;

			transform.position = Vector3.SmoothDamp (current, target, ref velocity, 0.3f );

			yield return new WaitForSeconds(0.055f);
		}

		destroyBoulder();	

	}

	private IEnumerator delayMove()
	{
		yield return new WaitForSeconds(5f);
	}

	private void destroyBoulder()
	{
		Destroy (gameObject);
	}
			

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag ("Door")) 
		{
			destroyBoulder();
		}
	}

	// Use this for initialization
	void Start () {

		a_BoulderSpawnManager = GameObject.Find ("BoulderSpawnCollider").GetComponent<BoulderSpawnManager>();
		a_Path = a_BoulderSpawnManager.a_Path;
		isActive = true;

		StartCoroutine ("moveBoulder");

	}

}

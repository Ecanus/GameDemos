using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour {

	[SerializeField]
	private PlayerController a_Player;


	// Use this for initialization
	void Start () {
		a_Player = GameObject.Find ("Player").GetComponent<PlayerController> ();
	}


	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag ("Boulder")) 
		{
			a_Player.canFire = true;

			Destroy (gameObject);
			Destroy (other.GetComponent<Transform>().gameObject);
		}

		if (other.CompareTag ("Block")) 
		{
			a_Player.canFire = true;

			Destroy (gameObject);
		}

		if (other.CompareTag ("Boundary")) 
		{
			a_Player.canFire = true;

			Destroy (gameObject);
		}
	}

	// Update is called once per frame
	void Update () {

		transform.Translate(Vector3.up * Time.deltaTime * 25f);
	}
}

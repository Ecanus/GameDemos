using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerController : MonoBehaviour {

	[SerializeField]
	private PlayerController a_Player;

	[SerializeField]
	private Transform loseText;

	[SerializeField]
	private Transform winText;

	[SerializeField]
	private OneWayManager a_BoundaryManager;

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag ("Key")) 
		{
			a_Player.increaseKeyCount ();
			Destroy (other.GetComponent<Transform>().gameObject);
		}

		if (other.CompareTag ("Door")) 
		{
			if (a_Player.getKeyCount () == 3) 
			{
				other.GetComponent<Transform> ().gameObject.SetActive (false);
			}
		}

		if (other.CompareTag ("Boulder")) 
		{
			Time.timeScale = 0;
			loseText.gameObject.SetActive (true);
		}

		if (other.CompareTag ("Goal")) 
		{
			Time.timeScale = 0;
			winText.gameObject.SetActive (true);
		}

		if (other.CompareTag ("Path")) 
		{
			a_BoundaryManager.sealEntry();
		}

		if (other.CompareTag ("MazeExit")) 
		{
			a_BoundaryManager.sealExit();
		}
	}
}

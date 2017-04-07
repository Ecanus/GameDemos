using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}


	private void handleInput()
	{
		float _horizontal = Input.GetAxisRaw ("Horizontal") * 25f;
		float _vertical = Input.GetAxisRaw ("Vertical") * 25f;
		_horizontal *= Time.deltaTime;
		_vertical *= Time.deltaTime;

		transform.Translate(_horizontal, _vertical, 0);
	}
	
	// Update is called once per frame
	void Update () {
		handleInput ();
		
	}
}

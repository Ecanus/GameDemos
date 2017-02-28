using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindVane : MonoBehaviour {


	private SpriteRenderer _SpriteRenderer;
	private Transform _Transform;

	private Vector2 a_TempVector;
	private Vector2 a_TempScale;

	private void updateDirection()
	{
		a_TempScale = transform.localScale;
		a_TempVector.x = Forces.Wind;
		a_TempVector.y = _Transform.localScale.y;

		_Transform.localScale = Vector2.Lerp(a_TempScale, a_TempVector, 3 * Time.deltaTime);
	}

	// Use this for initialization
	void Start () {
		_SpriteRenderer = GetComponent<SpriteRenderer> ();
		_Transform = GetComponent<Transform> ();
	}
	
	// Update is called once per frame
	void Update () {

		updateDirection ();
	}
}

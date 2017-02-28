using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

	[SerializeField]
	private Text keyCountText;

	[SerializeField]
	private float moveSpeed;

	[SerializeField]
	private int a_KeyCount;

	[SerializeField]
	private Transform a_Bullet;

	[SerializeField]
	private Transform a_LockedDoor;

	private Vector3 moveVector;
	public Vector3 fireVector;

	private Quaternion rotationVal;

	[SerializeField]
	private Light a_MainLight;

	public bool canFire;

	private void handleInput()
	{
		moveVector.x = Input.GetAxisRaw ("Horizontal") * moveSpeed;
		moveVector *= Time.deltaTime;

		moveVector.z = Input.GetAxisRaw ("Vertical") * moveSpeed;
		moveVector.z *= Time.deltaTime;


		transform.Translate (moveVector);

		updateFireVector();


		if (Input.GetKeyDown (KeyCode.Space) && canFire) 
		{
			canFire = false;
			Instantiate (a_Bullet, transform.position, rotationVal);
		}

	}

	private void updateFireVector()
	{
		if (Input.GetAxisRaw ("Horizontal") < 0) 
		{ 
			rotationVal = Quaternion.Euler (90, -90, 0);
		}

		if (Input.GetAxisRaw ("Horizontal") > 0) 
		{ 
			rotationVal = Quaternion.Euler (90, 90, 0);
		}

		if (Input.GetAxisRaw ("Vertical") > 0) 
		{  
			rotationVal = Quaternion.Euler (90, 0, 0);
		}

		if (Input.GetAxisRaw ("Vertical") < 0) 
		{ 
			rotationVal = Quaternion.Euler (90, 180, 0);
		}
	}

	private void checkDepth()
	{
		if (transform.position.y <= 1.5f) 
		{ 
			a_MainLight.intensity = 0.25f;
			PlaneViewManager.hideObjects ();
		}

		else if (transform.position.y > 1.5f) 
		{
			a_MainLight.intensity = 1f;
			PlaneViewManager.showObjects();
		}
	}
		
	public void increaseKeyCount()
	{
		a_KeyCount++;
		keyCountText.text = "Keys : " + a_KeyCount;

		if (a_KeyCount == 3) 
		{
			Door.changeMaterial ();
		}
	}

	public int getKeyCount()
	{
		return a_KeyCount;
	}

	// Use this for initialization
	void Start () {
		moveSpeed = 7.7f;
		a_KeyCount = 0;
		canFire = true;
	}
	
	// Update is called once per frame
	void Update () {

		handleInput ();
		checkDepth();
	}
}

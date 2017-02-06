using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour {

	private static Renderer a_Render;

	public static void changeMaterial()
	{
		a_Render.sharedMaterial = MaterialManager.gameMaterials [0];
	}

	// Use this for initialization
	void Start () {
		a_Render = GameObject.Find ("Door(Clone)").GetComponent<Renderer> ();
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialManager : MonoBehaviour {

	public Material highlightedMaterial;
	public Material branchMaterial;
	public Material branchPathMaterial;

	public static Material[] gameMaterials = new Material[3];

	// Use this for initialization
	void Start () {
		gameMaterials [0] = highlightedMaterial;
		gameMaterials [1] = branchMaterial;
		gameMaterials [2] = branchPathMaterial;
	}
}

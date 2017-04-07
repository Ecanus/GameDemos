using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

	[SerializeField]
	private Text _LookbackText;

	void Awake()
	{


	}

	public void increase()
	{
		Pathing._lookBack++;
	}

	public void decrease()
	{
		Pathing._lookBack--;
	}

		

	void Update()
	{
		_LookbackText.text = "" + Pathing._lookBack;
	}


}

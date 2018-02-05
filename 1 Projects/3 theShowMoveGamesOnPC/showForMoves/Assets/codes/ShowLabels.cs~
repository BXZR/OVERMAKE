using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowLabels : MonoBehaviour {

	private Text theShowText ;
	void Start () 
	{
		theShowText = this.GetComponent <Text> ();
		InvokeRepeating ("makeUpdate" , 0.1f,0.2f);
	}

	void makeUpdate()
	{
		theShowText.text = systemValues.linkServerLabel;
	}

	void Update () {
		
	}
}

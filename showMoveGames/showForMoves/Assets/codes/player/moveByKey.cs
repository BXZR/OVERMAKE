using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveByKey : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Input.GetKey (KeyCode.W))
			this.transform.Translate (new Vector3 (0,0,1) * 2.8f *Time .deltaTime);
		if (Input.GetKey (KeyCode.S))
			this.transform.Translate (new Vector3 (0,0,1)* -2.8f *Time .deltaTime);
		if (Input.GetKey (KeyCode.A))
			this.transform.Rotate ( this.transform .up * -65*Time.deltaTime);
		if (Input.GetKey (KeyCode.D))
			this.transform.Rotate ( this.transform .up * 65*Time.deltaTime);
		
	}
}

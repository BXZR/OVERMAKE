using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraChoice : MonoBehaviour {

	public GameObject PCCamera;
	public GameObject AndroidCamera;
	void Start () 
	{
		if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
		{
			Destroy (AndroidCamera.gameObject);
		} 
		else if (Application.platform == RuntimePlatform.Android) 
		{
			Destroy (PCCamera.gameObject);
		}
	}
	

}

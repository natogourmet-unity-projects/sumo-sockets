using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LabelLook : MonoBehaviour
{

	public Camera mc;
	
	// Use this for initialization
	void Start () {
		mc = Camera.main;
	}
	
	// Update is called once per frame
	void LateUpdate () {
		transform.LookAt(transform.position + mc.transform.rotation * Vector3.forward, 
			mc.transform.rotation * Vector3.up);
	}
}

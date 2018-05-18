using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnVertexCollide : MonoBehaviour {

	public bool isInsideTarget;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter (Collider other) {
		if (other.tag == "targetShape") 
			isInsideTarget = true;
	}

	void OnTriggerExit (Collider other) {
		if (other.tag == "targetShape") 
			isInsideTarget = false;
	}
}

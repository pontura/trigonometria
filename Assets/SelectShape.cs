﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectShape : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown (0)) {
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;

			if (Physics.Raycast (ray, out hit)) {
				if (hit.transform.name == "Player") {
					Debug.Log ("This is a Player");
				} else {
					Debug.Log ("This isn't a Player");                
				}
			}
		}
	}
}

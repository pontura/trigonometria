﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour {
	
	public RaycastHit[] hits;
	public bool mousePressed;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
		//Mousse Button
		if (Input.GetMouseButton (0)) {			
			if (!EventSystem.current.IsPointerOverGameObject ()) {
				mousePressed = true;
				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				hits = new RaycastHit[0];
				hits = Physics.RaycastAll (ray);
			}
		} else {
			mousePressed = false;
		}
	}
	

}

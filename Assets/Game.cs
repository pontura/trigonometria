﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {

	static Game mInstance = null;
	public Board board;
	public ShapesManager shapesManager;
	public InputManager inputManager;

	public static Game Instance {
		get
		{
			return mInstance;
		}		
	}

	void Awake () {
		mInstance = this;
		board = GetComponent<Board> ();
		shapesManager = GetComponent<ShapesManager> ();
		inputManager = GetComponent<InputManager> ();
	}
}

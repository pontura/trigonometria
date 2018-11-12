using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LevelManager : MonoBehaviour {

	public List<Level> ejercicios;
	public int currentLevel;

	[Serializable]
	public class Level{
		public string name;
		public string consignaGeneral;
		public GameObject targerShape;
		public Vector3 medidasTarget;
		public string unidad;
		public float escala;
		public string consignaComparar;
		public GameObject levelShape;
		public Vector3 medidasLevelShape;
		public float correctAnswer;
		public GameObject levelSubShape;
		public Vector3 medidasLevelSubShape;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public string GetConsignaComparar(){
		return ejercicios [currentLevel].consignaComparar;
	}

	public GameObject GetLevelShape(){
		return ejercicios [currentLevel].levelShape;
	}
}

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
		public float camSizeExample;
		public GameObject levelShapeExample;
		public ShapeAsset levelShape;
		public Vector3 medidasLevelShape;
		public float correctAnswer;
		public ShapeAsset levelSubShape;
		public Vector3 medidasLevelSubShape;
		public Vector3 snapStep;
	}

	// Use this for initialization
	void Start () {
		ResetShapeManager ();
		Events.Reiniciar += Reiniciar;
	}

	void OnDestroy(){
		Events.Reiniciar -= Reiniciar;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void Reiniciar(){
		LevelShapeShow (false);
		currentLevel = 0;
		ResetShapeManager ();
	}

	public string GetConsignaComparar(){
		return ejercicios [currentLevel].consignaComparar;
	}

	/*public GameObject GetLevelShape(){
		return ejercicios [currentLevel].levelShape;
	}*/

	public float GetCorrectAnswer(){
		return ejercicios [currentLevel].correctAnswer;
	}

	public void LevelShapeShow(bool enable){
		ejercicios [currentLevel].levelShapeExample.SetActive (enable);
	}

	public void NextExcersice(){
		currentLevel++;
		if (currentLevel >= ejercicios.Count)
			currentLevel = 0;

		ResetShapeManager ();
	}

	void ResetShapeManager(){
		Game.Instance.shapesManager.all.Clear ();
		Game.Instance.shapesManager.AddShape (ejercicios [currentLevel].levelShape.GetComponent<ShapeAsset> (), 1);
		if (ejercicios [currentLevel].levelSubShape != null) {
			Game.Instance.shapesManager.AddShape (ejercicios [currentLevel].levelSubShape.GetComponent<ShapeAsset> (), 2);
		}
		Camera.main.orthographicSize = ejercicios [currentLevel].camSizeExample;
	}

	public Vector3 GetSnap(){
		return ejercicios [currentLevel].snapStep;
	}
}

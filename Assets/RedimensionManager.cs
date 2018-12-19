using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RedimensionManager : MonoBehaviour {

	public GameObject mainShape;
	Vector3 originalScale;
	Vector3 originalPos;

	public Slider sliderX,sliderY,sliderZ;
	public float factorX,factorY,factorZ;
	bool doneX,doneY,doneZ;

	public GameObject consigna;
	public GameObject doneSign;

	// Use this for initialization
	void Start () {
		originalScale = mainShape.transform.localScale;
		originalPos = mainShape.transform.localPosition;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void ScaleX(float val, float min, float max){
		float factor = GetFactor (val,min,max);
		doneX = factor == factorX;
		Vector3 scale = mainShape.transform.localScale;
		mainShape.transform.localScale = new Vector3 (originalScale.x * factor, scale.y, scale.z);
		Check ();
	}

	public void ScaleY(float val, float min, float max){		
		float factor = GetFactor (val,min,max);
		doneY = factor == factorY;
		Vector3 scale = mainShape.transform.localScale;
		mainShape.transform.localScale = new Vector3 (scale.x, scale.y, originalScale.z * factor);
		Check ();
	}

	public void ScaleZ(float val, float min, float max){
		float factor = GetFactor (val,min,max);
		Vector3 scale = mainShape.transform.localScale;
		doneZ = factor == factorZ;
		Vector3 pos = mainShape.transform.localPosition;
		Debug.Log (factor);
		pos.y = originalPos.y + ((originalScale.y * factor) - originalScale.y) * 0.5f;
		mainShape.transform.localPosition = pos;
		mainShape.transform.localScale = new Vector3 (scale.x, originalScale.y * factor, scale.z);
		Check ();
	}

	float GetFactor(float val, float min, float max){
		float total = (max - min)*0.5f;
		return (val + total) / total;
	}

	void Check(){
		if (doneX && doneY && doneZ) {
			consigna.SetActive (false);
			Events.OnMessageShow ("Variaste el cuerpo correctamente");
			Invoke ("Reset", 5);
		}
	}

	void Reset(){
		sliderX.value = 0;
		sliderY.value = 0;
		sliderZ.value = 0;
		consigna.SetActive (true);
	}

}

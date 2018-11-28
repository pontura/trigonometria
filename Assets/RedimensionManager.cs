using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedimensionManager : MonoBehaviour {

	public GameObject mainShape;
	Vector3 originalScale;
	Vector3 originalPos;

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
		Vector3 scale = mainShape.transform.localScale;
		mainShape.transform.localScale = new Vector3 (originalScale.x * factor, scale.y, scale.z);	
	}

	public void ScaleY(float val, float min, float max){		
		float factor = GetFactor (val,min,max);
		Vector3 scale = mainShape.transform.localScale;
		mainShape.transform.localScale = new Vector3 (scale.x, scale.y, originalScale.z * factor);
	}

	public void ScaleZ(float val, float min, float max){
		float factor = GetFactor (val,min,max);
		Vector3 scale = mainShape.transform.localScale;
		Vector3 pos = mainShape.transform.localPosition;
		Debug.Log (factor);
		pos.y = originalPos.y + ((originalScale.y * factor) - originalScale.y) * 0.5f;
		mainShape.transform.localPosition = pos;
		mainShape.transform.localScale = new Vector3 (scale.x, originalScale.y * factor, scale.z);
	}

	float GetFactor(float val, float min, float max){
		float total = (max - min)*0.5f;
		return (val + total) / total;
	}

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ShapeAsset : MonoBehaviour {

	public Vector3 size;
	public List<ChildData> childs;
	public Color color;
	public float val;

	[Serializable]
	public class ChildData
	{
		public int id;
		public Vector3 position;
		public Vector3 rotation;
		public GameObject child;
	}

	public void SetColor(Color c){
		MeshRenderer[] mrs = gameObject.GetComponentsInChildren<MeshRenderer> ();
		foreach (MeshRenderer mr in mrs) {
			mr.material.color = c;
		}
		color = c;
	}

}

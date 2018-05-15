using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ShapeAsset : MonoBehaviour {

	public Vector3 size;
	public List<ChildData> childs;

	[Serializable]
	public class ChildData
	{
		public int id;
		public Vector3 position;
		public Vector3 rotation;
	}

}

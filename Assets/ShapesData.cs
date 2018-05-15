using System;
using UnityEngine;

[Serializable]
public class ShapesData {
	public ShapeAsset asset;
	public int id;

	[HideInInspector]
	public Color color;
}

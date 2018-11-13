using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapesManager : MonoBehaviour {

	public List<Color> colors;
	public List<ShapesData> all;

	public ShapesData GetByID(int id)
	{
		foreach (ShapesData data in all)
			if (data.id == id)
				return data;
		return null;
	}

	public Color GetFreeColor(List<ShapeAsset> shapes){
		for (int i = 0; i < colors.Count; i++) {
			bool used=false;
			for (int j = 0; j < shapes.Count; j++) {				
				if (colors [i] == shapes [j].color) {
					j = shapes.Count;
					used = true;
				}
			}
			if (!used)
				return colors [i];
		}
		return colors [shapes.Count % colors.Count];
	}

	public void AddShape(ShapeAsset sa, int id){
		ShapesData sd = new ShapesData ();
		sd.id = id;
		sd.asset = sa;
		all.Add (sd);
	}
}

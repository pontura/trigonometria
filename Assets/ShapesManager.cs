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
}

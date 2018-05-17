using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Events {
	
	public static System.Action<UIButton> OnButtonClickd = delegate { };	
	public static System.Action<GameObject> OnMouseCollide = delegate { };
	public static System.Action<ShapeAsset> OnShapeSelected = delegate { };
	public static System.Action<float> OnZoom = delegate { };
	public static System.Action OnCameraRotate = delegate { };
}

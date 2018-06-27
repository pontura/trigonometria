using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Events {

	//Input Events
	public static System.Action<UIButton> OnButtonClickd = delegate { };	
	public static System.Action<float> OnZoom = delegate { };

	public static System.Action<ShapeAsset> OnShapeSelected = delegate { };
	public static System.Action OnCameraRotate = delegate { };
	public static System.Action<string> OnMessageShow = delegate { };

	public static System.Action<Board.MechanicStates> OnMechanicChange = delegate { };
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraInScene : MonoBehaviour {

	public float defaultZoom;
	public float zoomIn;
	public float zoomOut;

	float zoom;
	private Camera cam;

	void Start () {
		Events.OnZoom += OnZoom;
		zoom = defaultZoom;
		cam = GetComponent<Camera> ();
	}

	void Update () {
		cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, zoom, 0.1f);
	}
	void OnZoom(float value)
	{
		if(value == 1)
			zoom = zoomIn;
		else if(value == 2)
			zoom = defaultZoom;
		else if(value == 3)
			zoom = zoomOut;
	}
}

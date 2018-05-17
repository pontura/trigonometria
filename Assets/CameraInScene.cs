using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraInScene : MonoBehaviour {

	public float defaultZoom;
	public float zoomIn;
	public float zoomOut;
	public GameObject cameraPivot;

	float zoom;
	private Camera cam;

	void Start () {
		Events.OnZoom += OnZoom;
		Events.OnCameraRotate += OnCameraRotate;
		zoom = defaultZoom;
		cam = GetComponent<Camera> ();

		Vector3 rot = cameraPivot.transform.localEulerAngles;
		Game.Instance.board.CameraRot = new Vector3 (-1 * rot.x, -1 * rot.y, -1 * rot.z);
	}

	void Update () {
		cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, zoom, 0.1f);
	}

	void OnCameraRotate(){
		Vector3 rot = cameraPivot.transform.localEulerAngles;
		cameraPivot.transform.localEulerAngles = new Vector3 (rot.x, rot.y + 90, rot.z);
		rot = cameraPivot.transform.localEulerAngles;
		Game.Instance.board.CameraRot = new Vector3 (-1 * rot.x, -1 * rot.y, -1 * rot.z);
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

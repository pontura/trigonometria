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
	public float newRot;

	float timerStart;
	float invDuration;

	public states state;
	public enum states
	{
		ZOOM,
		ROT,
		DONE
	}

	void Start () {
		Events.OnZoom += OnZoom;
		Events.OnCameraRotate += OnCameraRotate;
		zoom = defaultZoom;
		cam = GetComponent<Camera> ();


		Game.Instance.board.CameraRot = cameraPivot.transform.localEulerAngles;
	}

	void Update () {
		if (state == states.ZOOM) 
			cam.orthographicSize = Mathf.Lerp (cam.orthographicSize, zoom, (Time.time - timerStart) * invDuration);
		
		if (state == states.ROT) {
			Vector3 rot = cameraPivot.transform.localEulerAngles;
			float y = Mathf.Lerp (rot.y, newRot, (Time.time-timerStart)*invDuration);
			cameraPivot.transform.localEulerAngles = new Vector3 (rot.x, y, rot.z);
		}
	}

	void OnCameraRotate(){
		Vector3 rot = cameraPivot.transform.localEulerAngles;
		newRot = rot.y + 90;
		Game.Instance.board.CameraRot = new Vector3 (rot.x, rot.y + 90, rot.z);
		state = states.ROT;
		invDuration =1f/0.5f;
		timerStart = Time.time;
		Invoke ("Done", 0.25f);
	}

	void OnZoom(float value)
	{
		if(value == 1)
			zoom = zoomIn;
		else if(value == 2)
			zoom = defaultZoom;
		else if(value == 3)
			zoom = zoomOut;

		state = states.ZOOM;
		invDuration =1f/0.5f;
		timerStart = Time.time;
		Invoke ("Done", 0.5f);
	}

	void Done()
	{
		Vector3 rot = cameraPivot.transform.localEulerAngles;
		cameraPivot.transform.localEulerAngles = new Vector3 (rot.x, newRot, rot.z);
		state = states.DONE;	
	}
}

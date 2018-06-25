﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour {

	public GameObject targetShape;
	public ShapeAsset selectedShape;
	public Transform shapesContainer;
	public List<ShapeAsset> all;
	public GameObject floor;

	public bool total_Vol_done;
	public bool total_Vol_inside;


	public Vector3 lastRotation;
	Vector3 lastPosition;

	public Vector3 newRotation;
	Vector3 newPosition;

	Vector3 mousePos;
	Vector3 offset;
	float offsetFloor;

	bool unmoved;
	bool dragging;
	bool offsetDone;

	int empty_id;
	int empty_size = 3;
	int empty_offset = -2;

	[HideInInspector]
	public Vector3 CameraRot;

	public states state;
	public enum states
	{
		IDLE,
		TRANSFORMING,
		DRAGGING,
		DONE
	}

	// Use this for initialization
	void Start () {
		
	}

	void OnDestroy(){
		
	}
	public void AddNewShape(int id, Vector3 pos, Vector3 rot)
	{		
		SelectNewShape (id);
		selectedShape.transform.localPosition = pos;
		selectedShape.transform.localEulerAngles = rot;
		NewShapeAdded ();
		unmoved = false;
	}
	public void AddNewShape(int id)
	{
		SelectNewShape (id);
		selectedShape.transform.localEulerAngles = new Vector3(0f,Random.Range (0, 3)*90,0f);
		selectedShape.transform.localPosition = GetEmptySpace();		
		NewShapeAdded ();
		unmoved = true;
	}
	void SelectNewShape(int id)
	{
		ShapesData shapeData = Game.Instance.shapesManager.GetByID (id);
		ShapeAsset shapeAsset = Instantiate(shapeData.asset);

		shapeAsset.transform.SetParent (shapesContainer);
		shapeAsset.transform.localScale = Vector3.one;
		shapeAsset.SetColor(Game.Instance.shapesManager.GetFreeColor (all));

		all.Add (shapeAsset);
		selectedShape = shapeAsset;
	}
	void NewShapeAdded()
	{
		newPosition = selectedShape.transform.localPosition;
		newRotation = selectedShape.transform.localEulerAngles;
		Events.OnShapeSelected (selectedShape);
	}
	Vector3 GetEmptySpace()
	{
		if (all.Count == 1)
			return Vector3.zero;
		int z = (int) Mathf.Floor (empty_id / empty_size);
		z = (int)selectedShape.size.z * z;
		z += empty_offset;
		int x =empty_id % empty_size;
		x = (int)selectedShape.size.x * x;
		x += empty_offset;
		empty_id = (empty_id + 1) > (empty_size * empty_size) - 1 ? 0 : empty_id + 1;
		return new Vector3 (x, 0, z);
	}

	public void Rotate(int qty)
	{
		print ("ROTA");
		if (state == states.TRANSFORMING)
			return;

		if (unmoved)
			unmoved = false;
		
		state = states.TRANSFORMING;
		lastRotation = selectedShape.transform.localEulerAngles;

		Vector3 rot = selectedShape.transform.localEulerAngles;
		if (qty < 0 && (lastRotation.y < 1f || lastRotation.y > 359f)) {
			rot = new Vector3 (lastRotation.x, 359f, lastRotation.z);
			selectedShape.transform.localEulerAngles = rot;
			rot.y += qty+1;
		} else {
			rot.y += qty;
		}
		newRotation = rot;
		Invoke ("Done", 0.5f);
	}
	public void Move(Vector3 moveVector)
	{
		
		if (state == states.TRANSFORMING)
			return;

		if (unmoved)
			unmoved = false;

		state = states.TRANSFORMING;
		lastPosition = selectedShape.transform.localPosition;

		Vector3 pos = selectedShape.transform.localPosition;
		pos += moveVector;
		newPosition = pos;
		Invoke ("Done", 0.5f);
	}
	public void UndoLastTransform()
	{
		state = states.DONE;
		CancelInvoke ();
		if(unmoved){
			selectedShape.transform.localPosition = GetEmptySpace();
			lastPosition = selectedShape.transform.localPosition;
		}else{
			selectedShape.transform.localEulerAngles = lastRotation;
			selectedShape.transform.localPosition = lastPosition;
		}

	}
	void Update()
	{
		if (state == states.TRANSFORMING) {			
			selectedShape.transform.localEulerAngles = Vector3.Lerp (selectedShape.transform.localEulerAngles, newRotation, 0.25f);
			selectedShape.transform.localPosition = Vector3.Lerp (selectedShape.transform.localPosition, newPosition, 0.25f);
		}

		if (dragging&&offsetDone) {
			newPosition = mousePos+offset;
			newPosition.y = 0f;
			selectedShape.transform.localPosition = newPosition;
			selectedShape.transform.localEulerAngles = newRotation;
		}

		if (Game.Instance.inputManager.mousePressed && !dragging)
			SelectShape (Game.Instance.inputManager.hits);
		else if (Game.Instance.inputManager.mousePressed && dragging)
			CheckMousePos (Game.Instance.inputManager.hits);
		else if (!Game.Instance.inputManager.mousePressed) {
			Snap ();
			dragging = false;
			offset = Vector3.zero;
			offsetDone = false;
		}


	}
	void Done()
	{
		state = states.DONE;
		selectedShape.transform.localEulerAngles = newRotation;
		selectedShape.transform.localPosition = newPosition;
		CheckIntegration ();
	}

	void SelectShape(RaycastHit[] hits){
		bool selected = false;
		for (int i = 0; i < hits.Length; i++) {	
			GameObject go = hits [i].transform.gameObject;
			ShapeAsset sa = go.GetComponentInParent<ShapeAsset> ();
			if (sa != null && !dragging) {
				Events.OnShapeSelected (sa);
				selectedShape = sa;
				newRotation = sa.transform.localEulerAngles;
				newPosition = sa.transform.localPosition;
				lastRotation = sa.transform.localEulerAngles;
				lastPosition = sa.transform.localPosition;
				dragging = true;
			}
		}			
	}

	void CheckMousePos(RaycastHit[] hits){
		for (int i = 0; i < hits.Length; i++) {	
			GameObject go = hits [i].transform.gameObject;
			if (go.tag == "Floor") {
				if (!offsetDone) {
					offset = newPosition - hits [i].point;
					offsetDone = true;
				}
				mousePos = hits [i].point;
			}
		}			
	}

	void OnMouseRelease(){
		dragging = false;
	}

	void Snap(){
		if (dragging) {
			newPosition = new Vector3 (Mathf.Round (newPosition.x), Mathf.Round (newPosition.y), Mathf.Round (newPosition.z));
			selectedShape.transform.localPosition = newPosition;
			CheckCollision ();
		}
	}

	void CheckCollision(){
		if (selectedShape != null) {
			ShapeCollider[] scs = selectedShape.GetComponentsInChildren<ShapeCollider> ();
			foreach (ShapeCollider sc in scs) {
				if (sc.undoIt) {
					UndoLastTransform ();
					return;
				}
			}
			Invoke ("Done", 0.5f);
		}
	}

	public void DestroyShape(){
		all.Remove (selectedShape);
		Destroy (selectedShape.gameObject);
		Invoke ("CheckIntegration", 0.5f);
	}

	public void BreakShape(){
		List<GameObject> childs = new List<GameObject>();
		int childID = 0;
		foreach (ShapeAsset.ChildData childData in selectedShape.childs) {
			childData.child.transform.SetParent (shapesContainer);
			childs.Add (childData.child);
			childID = childData.id;
		}
		all.Remove (selectedShape);
		Destroy (selectedShape.gameObject);

		foreach (GameObject go in childs) {
			AddNewShape (childID, go.transform.localPosition, go.transform.localEulerAngles);
		}
		foreach (GameObject t in childs)
			Destroy (t);
	}

	void CheckIntegration(){
		//Debug.Log ("Check Integration");
		Mesh targetMesh = targetShape.GetComponent<MeshFilter> ().mesh;
		float totalvol = 0f;
		//bool areInside = true;
		foreach (ShapeAsset sa in all) {
			if (sa.childs.Count > 0) {
				foreach (ShapeAsset.ChildData chd in sa.childs) {
					Mesh mesh = chd.child.GetComponent<MeshFilter> ().mesh;
					/*if(areInside)
						foreach (Vector3 v in mesh.vertices)
							if(areInside)
								areInside = Math3d.IsPointInside (targetMesh, chd.child.transform.TransformPoint(v));*/
					//Debug.Log ("AreInside: " + areInside);
					totalvol += Math3d.VolumeOfMesh (mesh);
				}
			} else {
				Mesh mesh = sa.gameObject.GetComponentInChildren<MeshFilter> ().mesh;
				/*if(areInside)
					foreach (Vector3 v in mesh.vertices)
						if(areInside)
							areInside = Math3d.IsPointInside (targetMesh, targetShape.transform.InverseTransformPoint(v));*/
				//Debug.Log ("AreInside: " + areInside);
				totalvol += Math3d.VolumeOfMesh (mesh);
			}
		}

		float targetVol = Math3d.VolumeOfMesh (targetMesh);

		if (System.Math.Round (targetVol*100, 6) == System.Math.Round (totalvol*100, 6))
			total_Vol_done = true;
		else
			total_Vol_done = false;

		GameObject[] v = GameObject.FindGameObjectsWithTag ("vertexCollider");
		if (v.Length > 0) {
			bool areInside = true;
			for (int i = 0; i < v.Length; i++) {
				areInside = v [i].GetComponent<OnVertexCollide> ().isInsideTarget;
				if (!areInside) {
					i = v.Length;
				}
			}
			total_Vol_inside = areInside;
		}


		if (total_Vol_done && total_Vol_inside)
			Events.OnMessageShow ("¡Buen trabajo!");

		//Debug.Log (targetVol + " = " + totalvol);
		//Debug.Log (System.Math.Round (targetVol*100,6) + " = " + System.Math.Round (totalvol*100,6));
	}
}

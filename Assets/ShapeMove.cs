﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ShapeMove : MonoBehaviour {

	public Vector3 defaultSpace;

	Vector3 lastRotation;
	Vector3 lastPosition;

	Vector3 newRotation;
	Vector3 newPosition;

	Vector3 mousePos;
	public Vector3 offset;
	float offsetFloor;

	bool unmoved;
	bool offsetDone;

	int empty_id_x;
	int empty_id_z;
	int empty_size = 15;
	int empty_offset = -1;

	// Use this for initialization
	void Start () {

	}

	void OnDestroy(){

	}

	public void SetNewShape(Vector3 pos, Vector3 rot, bool isUnmoved){
		newPosition = pos;
		newRotation = rot;
		unmoved = isUnmoved;
	}

	public void ResetEmpty(){
		empty_id_x = 0;
		empty_id_z = 0;
	}

	public Vector3 GetEmptySpace()
	{
		if (Game.Instance.board.compararAll.Count == 1)
			return defaultSpace;
		ShapeAsset selectedShape = Game.Instance.board.selectedShape;
		float z = empty_id_z * selectedShape.size.z;
		empty_id_z++;
		if (z > empty_size) {
			empty_id_z = 0;
			empty_id_x++;
		}
		float x =empty_id_x * selectedShape.size.x;
		if (x > empty_size) {
			empty_id_x = 0;
		}
		Debug.Log (empty_id_x+" : "+empty_id_z);
		return new Vector3 (defaultSpace.x+x, 0,defaultSpace.z+z);
	}

	public void Rotate(int qty)
	{
		//print ("ROTA");
		if (GetState() == Board.ActionStates.TRANSFORMING)
			return;

		if (unmoved)
			unmoved = false;

		ShapeAsset selectedShape = Game.Instance.board.selectedShape;

		SetState(Board.ActionStates.TRANSFORMING);
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

		if (GetState() == Board.ActionStates.TRANSFORMING)
			return;

		if (unmoved)
			unmoved = false;

		ShapeAsset selectedShape = Game.Instance.board.selectedShape;

		SetState(Board.ActionStates.TRANSFORMING);
		lastPosition = selectedShape.transform.localPosition;

		Vector3 pos = selectedShape.transform.localPosition;
		pos += moveVector;
		if (pos.y < 0)
			pos.y = 0;
		newPosition = pos;
		Invoke ("Done", 0.5f);
	}
	public void UndoLastTransform()
	{
		if (GetState() == Board.ActionStates.DRAGGING)
			return;
		SetState(Board.ActionStates.DONE);
		CancelInvoke ();
		ShapeAsset selectedShape = Game.Instance.board.selectedShape;
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
		if (GetState() == Board.ActionStates.TRANSFORMING) {
			ShapeAsset selectedShape = Game.Instance.board.selectedShape;
			selectedShape.transform.localEulerAngles = Vector3.Lerp (selectedShape.transform.localEulerAngles, newRotation, 0.25f);
			selectedShape.transform.localPosition = Vector3.Lerp (selectedShape.transform.localPosition, newPosition, 0.25f);
		}


		if (GetState() == Board.ActionStates.DRAGGING) {
			if (offsetDone) {
				newPosition = mousePos+offset;
				//newPosition.y = 0f;
				ShapeAsset selectedShape = Game.Instance.board.selectedShape;
				selectedShape.transform.localPosition = newPosition;
				selectedShape.transform.localEulerAngles = newRotation;
			}
			
			if (Game.Instance.inputManager.mousePressed)
				CheckMousePos (Game.Instance.inputManager.hits);
			else if (!Game.Instance.inputManager.mousePressed) {
				Snap ();
				SetState (Board.ActionStates.IDLE);
				offsetDone = false;
			}
		}else{			
			if (Game.Instance.inputManager.mousePressed)
				SelectShape (Game.Instance.inputManager.hits);		
		}


	}
	void Done()
	{
		SetState (Board.ActionStates.DONE);
		ShapeAsset selectedShape = Game.Instance.board.selectedShape;
		selectedShape.transform.localEulerAngles = newRotation;
		selectedShape.transform.localPosition = newPosition;
		if(Game.Instance.board.mechanicState == Board.MechanicStates.INTEGRAR)
		Game.Instance.integrationManager.CheckIntegration ();
	}

	void Snap(){
		Vector3 step = Game.Instance.levelManager.GetSnap();
		if (Game.Instance.board.mechanicState == Board.MechanicStates.COMBINAR)
			step = Game.Instance.combinarManager.step;
		newPosition = new Vector3 (Mathf.Round (newPosition.x/step.x)*step.x, Mathf.Round (newPosition.y/step.y)*step.y, Mathf.Round (newPosition.z/step.z)*step.z);
		ShapeAsset selectedShape = Game.Instance.board.selectedShape;
		selectedShape.transform.localPosition = newPosition;
		CheckCollision ();
	}

	void CheckCollision(){
		ShapeAsset selectedShape = Game.Instance.board.selectedShape;
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

	void SelectShape(RaycastHit[] hits){

		Array.Sort(hits,delegate(RaycastHit hit1, RaycastHit hit2){
			return hit1.distance.CompareTo(hit2.distance);			
		});

		
		for (int i = 0; i < hits.Length; i++) {
			GameObject go = hits [i].transform.gameObject;
			ShapeAsset sa = go.GetComponentInParent<ShapeAsset> ();
			if (sa != null) {
				Debug.Log (sa.gameObject.name);
				Events.OnShapeSelected (sa);
				Game.Instance.board.selectedShape = sa;
				newRotation = sa.transform.localEulerAngles;
				newPosition = sa.transform.localPosition;
				lastRotation = sa.transform.localEulerAngles;
				lastPosition = sa.transform.localPosition;
				Events.CloseSubMenu ();
				SetState (Board.ActionStates.DRAGGING);
				break;
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

	void SetState(Board.ActionStates s){
		Game.Instance.board.actionState = s;
	}

	Board.ActionStates GetState(){
		return Game.Instance.board.actionState;
	}
}

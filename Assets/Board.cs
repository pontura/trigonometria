﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour {

	public ShapeAsset selectedShape;
	public Transform compararContainer;
	public GameObject compararCanvas;
	public Transform combinarContainer;
	public GameObject combinarCanvas;
	public Transform redimenContainer;
	public GameObject redimenCanvas;
	public List<ShapeAsset> compararAll;
	public List<ShapeAsset> combinarAll;

	[HideInInspector]
	public Vector3 CameraRot;

	public MechanicStates mechanicState;
	public enum MechanicStates
	{
		INTEGRAR,
		COMBINAR,
		REDIMENSIONAR
	}

	public ActionStates actionState;
	public enum ActionStates
	{
		IDLE,
		TRANSFORMING,
		DRAGGING,
		DONE
	}

	// Use this for initialization
	void Start () {
		Events.OnMechanicChange += OnMechanicsChange;
		Events.Reiniciar += Reiniciar;
	}

	void OnDestroy(){
		Events.OnMechanicChange -= OnMechanicsChange;
		Events.Reiniciar -= Reiniciar;
	}

	void Reiniciar(){
		Clear ();
	}

	public void AddNewShape(int id, Vector3 pos, Vector3 rot)
	{		
		SelectNewShape (id);
		selectedShape.transform.localPosition = pos;
		selectedShape.transform.localEulerAngles = rot;
		NewShapeAdded (false);
	}
	public void AddNewShape(int id)
	{
		
		if (mechanicState == MechanicStates.INTEGRAR) {
			if (compararAll.Count >= Game.Instance.integrationManager.answer) {
				Game.Instance.integrationManager.count.color = Color.red;	
				return;
			}
		}
		SelectNewShape (id);
		//selectedShape.transform.localEulerAngles = new Vector3 (0f, Random.Range (0, 3) * 90, 0f);
		//selectedShape.transform.localPosition = Game.Instance.shapeMove.GetEmptySpace ();	
		Game.Instance.shapeMove.ResetEmpty();
		selectedShape.transform.localPosition = Game.Instance.shapeMove.defaultSpace;		
		NewShapeAdded (true);			

		float c = Game.Instance.integrationManager.answer - compararAll.Count;
		//c = c < 0f ? 0f : c;
		Game.Instance.integrationManager.count.text = "" + c;
	}
	void SelectNewShape(int id)
	{
		

		if (mechanicState == MechanicStates.INTEGRAR) {
			ShapesData shapeData = Game.Instance.shapesManager.GetByID (id);
			ShapeAsset shapeAsset = Instantiate(shapeData.asset);
			shapeAsset.transform.SetParent (compararContainer);
			shapeAsset.transform.localScale = Vector3.one;
			shapeAsset.SetColor(Game.Instance.shapesManager.GetFreeColor (compararAll));
			compararAll.Add (shapeAsset);
			selectedShape = shapeAsset;
		} else if (mechanicState == MechanicStates.COMBINAR) {
			ShapesData shapeData = Game.Instance.shapesManager.GetCombinarByID (id);
			ShapeAsset shapeAsset = Instantiate(shapeData.asset);
			shapeAsset.transform.SetParent (combinarContainer);
			shapeAsset.transform.localScale = Vector3.one;
			shapeAsset.SetColor(Color.white);
			combinarAll.Add (shapeAsset);
			selectedShape = shapeAsset;
		}




	}
	void NewShapeAdded(bool isUnmoved){
		Game.Instance.shapeMove.SetNewShape (selectedShape.transform.localPosition, selectedShape.transform.localEulerAngles, isUnmoved);
		Events.OnShapeSelected (selectedShape);
	}

	public void DestroyShape(){
		if (mechanicState == MechanicStates.INTEGRAR) {
			float c = float.Parse (Game.Instance.integrationManager.count.text);
			c += selectedShape.val;
			c = c > Game.Instance.integrationManager.answer ? Game.Instance.integrationManager.answer : c;
			Game.Instance.integrationManager.count.text = "" + c;
			Game.Instance.integrationManager.count.color = Color.white;	

			compararAll.Remove (selectedShape);
			Destroy (selectedShape.gameObject);
			Invoke ("CheckIntegration", 0.5f);
		} else if (mechanicState == MechanicStates.COMBINAR) {
			combinarAll.Remove (selectedShape);
			Destroy (selectedShape.gameObject);
		}
	}

	public void Clear(){
		if (mechanicState == MechanicStates.INTEGRAR) {
			foreach (ShapeAsset sa in compararAll) {			
				Destroy (sa.gameObject);
			}
			compararAll.Clear ();

			foreach (ShapeAsset sa in combinarAll) {			
				Destroy (sa.gameObject);
			}
			combinarAll.Clear ();

		} else if (mechanicState == MechanicStates.COMBINAR) {
			foreach (ShapeAsset sa in combinarAll) {			
				Destroy (sa.gameObject);
			}
			combinarAll.Clear ();
		}
	}

	public void BreakShape(){
		List<ShapeAsset.ChildData> childs = new List<ShapeAsset.ChildData>();
		if (selectedShape.childs.Count > 0) {
			int childID = 0;
			foreach (ShapeAsset.ChildData childData in selectedShape.childs) {
				if (mechanicState == MechanicStates.INTEGRAR) {
					childData.child.transform.SetParent (compararContainer);
					childData.child.transform.position = selectedShape.transform.position;
					childData.child.transform.rotation = selectedShape.transform.rotation;
				} else if (mechanicState == MechanicStates.COMBINAR) {
					childData.child.transform.SetParent (combinarContainer);
					childData.child.transform.position = selectedShape.transform.position;
					childData.child.transform.rotation = selectedShape.transform.rotation;
				}
				childs.Add (childData);
				//childID = childData.id;
			}
			if (mechanicState == MechanicStates.INTEGRAR) {
				compararAll.Remove (selectedShape);
				Destroy (selectedShape.gameObject);
			} else if (mechanicState == MechanicStates.COMBINAR) {
				combinarAll.Remove (selectedShape);
				Destroy (selectedShape.gameObject);
			}

			foreach (ShapeAsset.ChildData ch in childs) {
				ch.child.transform.position = ch.child.transform.position + (ch.child.transform.rotation * ch.position);
				ch.child.transform.eulerAngles = ch.child.transform.eulerAngles + ch.rotation;
				AddNewShape (ch.id, ch.child.transform.localPosition, ch.child.transform.localEulerAngles);
			}
			foreach (ShapeAsset.ChildData t in childs)
				Destroy (t.child);
		}
	}

	void CheckIntegration(){
		Game.Instance.integrationManager.CheckIntegration ();
	}

	void OnMechanicsChange(MechanicStates state){		
		mechanicState = state;
		if (mechanicState == MechanicStates.INTEGRAR) {
			compararContainer.gameObject.SetActive (true);
			compararCanvas.SetActive (true);
			combinarContainer.gameObject.SetActive (false);
			combinarCanvas.SetActive (false);
			redimenContainer.gameObject.SetActive (false);
			redimenCanvas.SetActive (false);
		} else if (mechanicState == MechanicStates.COMBINAR) {
			compararContainer.gameObject.SetActive (false);
			compararCanvas.SetActive (false);
			combinarContainer.gameObject.SetActive (true);
			combinarCanvas.SetActive (true);
			redimenContainer.gameObject.SetActive (false);
			redimenCanvas.SetActive (false);
		} else if (mechanicState == MechanicStates.REDIMENSIONAR) {
			compararContainer.gameObject.SetActive (false);
			compararCanvas.SetActive (false);
			combinarContainer.gameObject.SetActive (false);
			combinarCanvas.SetActive (false);
			redimenContainer.gameObject.SetActive (true);
			redimenCanvas.SetActive (true);
		}

		Events.OnShapeSelected (null);
	}
}

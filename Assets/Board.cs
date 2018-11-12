using System.Collections;
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
	public List<ShapeAsset> all;

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
	}

	void OnDestroy(){
		Events.OnMechanicChange -= OnMechanicsChange;
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
		SelectNewShape (id);
		selectedShape.transform.localEulerAngles = new Vector3(0f,Random.Range (0, 3)*90,0f);
		selectedShape.transform.localPosition = Game.Instance.shapeMove.GetEmptySpace();		
		NewShapeAdded (true);
	}
	void SelectNewShape(int id)
	{
		ShapesData shapeData = Game.Instance.shapesManager.GetByID (id);
		ShapeAsset shapeAsset = Instantiate(shapeData.asset);

		if(mechanicState==MechanicStates.INTEGRAR)
			shapeAsset.transform.SetParent (compararContainer);
		else if(mechanicState==MechanicStates.COMBINAR)
			shapeAsset.transform.SetParent (combinarContainer);
		shapeAsset.transform.localScale = Vector3.one;
		shapeAsset.SetColor(Game.Instance.shapesManager.GetFreeColor (all));

		all.Add (shapeAsset);
		selectedShape = shapeAsset;
	}
	void NewShapeAdded(bool isUnmoved){
		Game.Instance.shapeMove.SetNewShape (selectedShape.transform.localPosition, selectedShape.transform.localEulerAngles, isUnmoved);
		Events.OnShapeSelected (selectedShape);
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
			if(mechanicState==MechanicStates.INTEGRAR)
				childData.child.transform.SetParent (compararContainer);
			else if(mechanicState==MechanicStates.COMBINAR)
				childData.child.transform.SetParent (combinarContainer);
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
	}
}

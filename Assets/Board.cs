using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour {

	public GameObject targetShape;
	public ShapeAsset selectedShape;
	public Transform shapesContainer;
	public List<ShapeAsset> all;

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

		shapeAsset.transform.SetParent (shapesContainer);
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
		Game.Instance.integrationManager.CheckIntegration ();
	}
}

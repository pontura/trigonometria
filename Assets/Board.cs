using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour {
	
	public ShapeAsset selectedShape;
	public Transform shapesContainer;
	public List<ShapeAsset> all;

	public Vector3 lastRotation;
	public Vector3 lastPosition;

	Vector3 newRotation;
	Vector3 newPosition;

	bool unmoved;

	int empty_id;
	int empty_size = 3;
	int empty_offset = -2;

	public states state;
	public enum states
	{
		IDLE,
		TRANSFORMING,
		DONE
	}

	// Use this for initialization
	void Start () {
		Events.OnMouseCollide += SelectShape;
	}

	void OnDestroy(){
		Events.OnMouseCollide -= SelectShape;
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
	}
	void Done()
	{
		state = states.DONE;
		selectedShape.transform.localEulerAngles = newRotation;
		selectedShape.transform.localPosition = newPosition;
	}

	void SelectShape(GameObject go){
		ShapeAsset sa = go.GetComponentInParent<ShapeAsset> ();
		if (sa != null) {
			Events.OnShapeSelected (sa);
			selectedShape = sa;
			newRotation = sa.transform.localEulerAngles;
			newPosition = sa.transform.localPosition;
		}
	}

	public void DestroyShape(){
		all.Remove (selectedShape);
		Destroy (selectedShape.gameObject);
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
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour {
	
	public ShapeAsset selectedShape;
	public Transform shapesContainer;
	public List<ShapeAsset> all;

	Vector3 lastRotation;
	Vector3 lastPosition;

	Vector3 newRotation;
	Vector3 newPosition;

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

	public void AddNewShape(int id)
	{
		ShapesData shapeData = Game.Instance.shapesManager.GetByID (id);
		ShapeAsset shapeAsset = Instantiate(shapeData.asset);

		shapeAsset.transform.SetParent (shapesContainer);
		shapeAsset.transform.localScale = Vector3.one;
		shapeAsset.transform.localPosition = GetEmptySpace(shapeData);

		all.Add (shapeAsset);
		selectedShape = shapeAsset;
		lastPosition = shapeAsset.transform.localPosition;
		lastRotation = Vector3.zero;
	}
	Vector3 GetEmptySpace(ShapesData shapeData)
	{
		Vector3 emptySpace = Vector3.zero;
		foreach (ShapeAsset asset in all) {
			emptySpace.x += asset.size.x;
		}
		return emptySpace;
	}
	public void Rotate(int qty)
	{
		if (state == states.TRANSFORMING)
			return;
		
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
		selectedShape.transform.localEulerAngles = lastRotation;
		selectedShape.transform.localPosition = lastPosition;

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
		ShapeCollider sc = go.GetComponent<ShapeCollider> ();
		if (sc != null) {
			selectedShape = sc.shapeAsset;
			Debug.Log (selectedShape.transform.name + ": " + selectedShape.transform.GetInstanceID ());
		}

	}
}

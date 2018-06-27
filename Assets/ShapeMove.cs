using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeMove : MonoBehaviour {

	Vector3 lastRotation;
	Vector3 lastPosition;

	Vector3 newRotation;
	Vector3 newPosition;

	Vector3 mousePos;
	public Vector3 offset;
	float offsetFloor;

	bool unmoved;
	bool offsetDone;

	int empty_id;
	int empty_size = 3;
	int empty_offset = -2;

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

	public Vector3 GetEmptySpace()
	{
		if (Game.Instance.board.all.Count == 1)
			return Vector3.zero;
		ShapeAsset selectedShape = Game.Instance.board.selectedShape;
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
		//print ("ROTA");
		if (GetState() == Board.states.TRANSFORMING)
			return;

		if (unmoved)
			unmoved = false;

		ShapeAsset selectedShape = Game.Instance.board.selectedShape;

		SetState(Board.states.TRANSFORMING);
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

		if (GetState() == Board.states.TRANSFORMING)
			return;

		if (unmoved)
			unmoved = false;

		ShapeAsset selectedShape = Game.Instance.board.selectedShape;

		SetState(Board.states.TRANSFORMING);
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
		if (GetState() == Board.states.DRAGGING)
			return;
		SetState(Board.states.DONE);
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
		if (GetState() == Board.states.TRANSFORMING) {
			ShapeAsset selectedShape = Game.Instance.board.selectedShape;
			selectedShape.transform.localEulerAngles = Vector3.Lerp (selectedShape.transform.localEulerAngles, newRotation, 0.25f);
			selectedShape.transform.localPosition = Vector3.Lerp (selectedShape.transform.localPosition, newPosition, 0.25f);
		}


		if (GetState() == Board.states.DRAGGING) {
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
				SetState (Board.states.IDLE);
				offsetDone = false;
			}
		}else{			
			if (Game.Instance.inputManager.mousePressed)
				SelectShape (Game.Instance.inputManager.hits);		
		}


	}
	void Done()
	{
		SetState (Board.states.DONE);
		ShapeAsset selectedShape = Game.Instance.board.selectedShape;
		selectedShape.transform.localEulerAngles = newRotation;
		selectedShape.transform.localPosition = newPosition;
		Game.Instance.integrationManager.CheckIntegration ();
	}

	void Snap(){
		newPosition = new Vector3 (Mathf.Round (newPosition.x), Mathf.Round (newPosition.y), Mathf.Round (newPosition.z));
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
		for (int i = 0; i < hits.Length; i++) {	
			GameObject go = hits [i].transform.gameObject;
			ShapeAsset sa = go.GetComponentInParent<ShapeAsset> ();
			if (sa != null) {
				Events.OnShapeSelected (sa);
				Game.Instance.board.selectedShape = sa;
				newRotation = sa.transform.localEulerAngles;
				newPosition = sa.transform.localPosition;
				lastRotation = sa.transform.localEulerAngles;
				lastPosition = sa.transform.localPosition;
				SetState (Board.states.DRAGGING);
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

	void SetState(Board.states s){
		Game.Instance.board.state = s;
	}

	Board.states GetState(){
		return Game.Instance.board.state;
	}
}

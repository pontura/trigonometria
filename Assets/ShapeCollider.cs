using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeCollider : MonoBehaviour {

	public ShapeAsset shapeAsset;
	public bool undoIt;

	void Awake()
	{
		shapeAsset = GetComponentInParent<ShapeAsset> ();
	}
	void OnTriggerEnter (Collider other) {
		ShapeCollider sc = other.GetComponent<ShapeCollider> ();
		if (sc == null)
			return;
		ShapeAsset otherShapeAsset = sc.shapeAsset;
		if (otherShapeAsset == null)
			return;
		if (shapeAsset == otherShapeAsset)
			return;
		Game.Instance.shapeMove.UndoLastTransform ();
		undoIt = true;
	}

	void OnTriggerExit (Collider other) {
		ShapeCollider sc = other.GetComponent<ShapeCollider> ();
		if (sc == null)
			return;
		ShapeAsset otherShapeAsset = sc.shapeAsset;
		if (otherShapeAsset == null)
			return;
		if (shapeAsset == otherShapeAsset)
			return;
		undoIt = false;
	}

}

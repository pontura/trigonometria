using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour {

	public GameObject targetShape;
	public ShapeAsset selectedShape;
	public Transform shapesContainer;
	public List<ShapeAsset> all;

	bool total_Vol_done;
	bool total_Vol_inside;

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

	public void CheckIntegration(){
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

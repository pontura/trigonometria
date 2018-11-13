using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddShapeColliders : MonoBehaviour {

	public List<GameObject> colliderShapes;

	// Use this for initialization
	void Start () {
		AddCubeColliders ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void AddCubeColliders(){
		for(int i=0;i<colliderShapes.Count;i++){
			GameObject go = colliderShapes[i];
			MeshFilter filter = go.GetComponent< MeshFilter >();
			Mesh mesh = filter.mesh;
			foreach(Vector3 v in mesh.vertices){
				GameObject vBox = GameObject.CreatePrimitive(PrimitiveType.Cube);
				vBox.transform.position = go.transform.TransformPoint(v);
				vBox.transform.localScale = new Vector3(0.15F, 0.15F, 0.15f);
				vBox.transform.parent = go.transform;
				AddColliderComponents (vBox);
			}
		}
	}

	void AddColliderComponents(GameObject go){
		go.tag = "vertexCollider";
		Rigidbody rb =  go.AddComponent<Rigidbody> ();
		rb.isKinematic = true;
		rb.useGravity = false;

		//go.AddComponent<ShapeCollider> ();
		go.AddComponent<OnVertexCollide>();

		go.GetComponent<Collider> ().isTrigger = true;

		go.GetComponent<MeshRenderer> ().enabled = false;

	}

	public bool GetVertexInside(){
		bool areInside = true;
		for(int i=0;i<colliderShapes.Count;i++){
			for(int j=0;j<colliderShapes[i].transform.childCount;j++){
				GameObject child = colliderShapes[i].transform.GetChild(j).gameObject;
				if(child.tag =="vertexCollider"){
					areInside = child.GetComponent<OnVertexCollide> ().isInsideTarget;
					if (!areInside)
						j = colliderShapes[i].transform.childCount;
				}
			}
			if (!areInside) 
				i = colliderShapes.Count;
		}
		return areInside;
	}
}

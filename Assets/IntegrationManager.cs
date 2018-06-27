using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntegrationManager : MonoBehaviour {

	public bool integration_done;

	bool total_Vol_done;
	bool total_Vol_inside;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void CheckIntegration(){
		//Debug.Log ("Check Integration");
		Mesh targetMesh = Game.Instance.board.targetShape.GetComponent<MeshFilter> ().mesh;
		float totalvol = 0f;
		//bool areInside = true;
		foreach (ShapeAsset sa in Game.Instance.board.all) {
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


		if (total_Vol_done && total_Vol_inside) {
			integration_done = true;
			Events.OnMessageShow ("¡Buen trabajo!");
		}

		//Debug.Log (targetVol + " = " + totalvol);
		//Debug.Log (System.Math.Round (targetVol*100,6) + " = " + System.Math.Round (totalvol*100,6));
	}
}

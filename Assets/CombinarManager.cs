using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombinarManager : MonoBehaviour {

	public GameObject question;
	public GameObject combinando;
	public GameObject retry;

	public Text medidas;

	public InputField input;

	int tries;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Terminar(){
		tries = 0;
		combinando.SetActive (false);
		question.SetActive (true);
		retry.SetActive (false);
		LevelManager.Level l = Game.Instance.levelManager.GetLevel ();
		medidas.text = "El cuerpo transparente mide " + l.medidasTarget.x + "x" + l.medidasTarget.y + "x" + l.medidasTarget.z + " cm y el otro mide " +
		l.medidasLevelShape.x + "x" + l.medidasLevelShape.y + "x" + l.medidasLevelShape.z+" cm";
	}

	void Retry(){
		tries++;
		if (tries < 3) {
			combinando.SetActive (false);
			question.SetActive (true);
			retry.SetActive (true);
			input.text = "";
		} else {
			Reset ();
		}
	}

	public void SetAnswer(string s){
		CheckVolume(float.Parse(input.text));
	}

	public void CheckVolume(float val){
		Debug.Log ("CheckVolume");
		Transform targetShape = Game.Instance.integrationManager.targetShape.transform;
		Mesh targetMesh = Game.Instance.integrationManager.targetShape.GetComponent<MeshFilter> ().mesh;
		float totalvol = 0f;
		float insideVol = 0f;
		//bool areInside = true;
		foreach (ShapeAsset sa in Game.Instance.board.combinarAll) {
			bool isInside = sa.gameObject.GetComponent<AddShapeColliders> ().GetVertexInside ();
			if (sa.childs.Count > 0) {
				foreach (ShapeAsset.ChildData chd in sa.childs) {
					Mesh mesh = chd.child.GetComponent<MeshFilter> ().mesh;
					//Debug.Log (chd.child.name);
					/*if(areInside)
						foreach (Vector3 v in mesh.vertices)
							if(areInside)
								areInside = Math3d.IsPointInside (targetMesh, chd.child.transform.TransformPoint(v));*/
					//Debug.Log ("AreInside: " + areInside);
					float vol = Math3d.VolumeOfMesh (mesh,chd.child.transform);
					totalvol += vol;
					if (isInside)
						insideVol += vol;
				}
			} else {
				Mesh mesh = sa.gameObject.GetComponentInChildren<MeshFilter> ().mesh;
				/*if(areInside)
					foreach (Vector3 v in mesh.vertices)
						if(areInside)
							areInside = Math3d.IsPointInside (targetMesh, targetShape.transform.InverseTransformPoint(v));*/
				//Debug.Log ("AreInside: " + areInside);

				float vol = Math3d.VolumeOfMesh (mesh,sa.transform.GetChild(0));
				totalvol += vol;
				if (isInside)
					insideVol += vol;
				//Debug.Log (sa.gameObject.name+": "+totalvol+" / "+mesh.bounds + " / "+ sa.gameObject.GetComponentInChildren<Renderer> ().bounds);
			}
		}

		float targetVol = Math3d.VolumeOfMesh (targetMesh,targetShape);

		Debug.Log (targetVol + " = " + totalvol + " = "+insideVol) ;

		totalvol += targetVol;

		totalvol *= 1000;

		if (System.Math.Round (val * 100, 6) == System.Math.Round (totalvol * 100, 6)) {
			question.SetActive (false);
			Events.OnMessageShow ("Ese es el volumen de la escultura");
			Invoke ("Reset", 5);
		} else {
			question.SetActive (false);
			Events.OnMessageShow ("Ese no es el volumen de la escultura");
			Invoke ("Retry", 5);
		}


	}

	public void Reset(){
		input.text = "";
		Game.Instance.board.Clear ();
		combinando.SetActive (true);
		question.SetActive (false);
		retry.SetActive (false);
	}
}

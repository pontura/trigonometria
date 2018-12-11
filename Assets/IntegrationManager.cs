using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IntegrationManager : MonoBehaviour {

	public GameObject targetShape;
	public bool integration_done;

	public GameObject question;
	public GameObject integrating;
	public GameObject confirmation;

	public Text consigna;
	public InputField respuesta;
	public Text count;
	public GameObject badConfirmationTxt;

	public float answer;

	Material targetMaterial;
	Color originalTargetColor;

	public IntegrationStates integrationState;
	public enum IntegrationStates{
		question,
		integrating,
		confirmation
	}

	bool total_Vol_done;
	bool total_Vol_inside;
	bool vol_inside;

	// Use this for initialization
	void Start () {
		targetMaterial = targetShape.GetComponent<Renderer> ().material;
		originalTargetColor = targetMaterial.color;
		Init ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Init(){		
		integrationState = IntegrationStates.question;
		targetMaterial.color = originalTargetColor;
		SetStateScreen ();
	}

	public void CheckIntegration(){
		Debug.Log ("Check Integration");
		Mesh targetMesh = targetShape.GetComponent<MeshFilter> ().mesh;
		float totalvol = 0f;
		float insideVol = 0f;
		//bool areInside = true;
		foreach (ShapeAsset sa in Game.Instance.board.compararAll) {
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
					float vol = Math3d.VolumeOfMesh (mesh);
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

		float targetVol = Math3d.VolumeOfMesh (targetMesh,targetShape.transform);

		if (System.Math.Round (targetVol * 100, 6) == System.Math.Round (totalvol * 100, 6))
			total_Vol_done = true;
		else {
			total_Vol_done = false;
			if (System.Math.Round (targetVol * 100, 6) == System.Math.Round (insideVol * 100, 6))
				vol_inside = true;
			else
				vol_inside = false;
		}

		GameObject[] vc = GameObject.FindGameObjectsWithTag ("vertexCollider");
		if (vc.Length > 0) {
			bool areInside = true;
			for (int i = 0; i < vc.Length; i++) {
				areInside = vc [i].GetComponent<OnVertexCollide> ().isInsideTarget;
				if (!areInside) {
					i = vc.Length;
				}
			}
			total_Vol_inside = areInside;
		}


		if (total_Vol_done && total_Vol_inside) {
			Game.Instance.board.Clear ();
			integration_done = true;
			Game.Instance.combinarManager.Reset ();
			float h, s, v;
			Color.RGBToHSV (originalTargetColor, out h, out s, out v);
			targetMaterial.color = Color.HSVToRGB (h, 1f, v);
			Events.OnMessageShow ("Completaste el cuerpo principal");
			Invoke ("SetConfirmation", 5);
		} else if (total_Vol_inside) {
			if (Game.Instance.board.compararAll.Count >= answer) {
				count.color = Color.red;
				targetMaterial.color = new Color (255, 0, 0, originalTargetColor.a);
				Events.OnMessageShow ("Ya usaste todos los cuerpos disponibles");
				Invoke ("SetConfirmation", 5);
			}
		} else if (vol_inside) {
			if (answer > Game.Instance.levelManager.GetCorrectAnswer ()) {
				integration_done = true;
				Game.Instance.combinarManager.Reset ();
				float h, s, v;
				Color.RGBToHSV (originalTargetColor, out h, out s, out v);
				targetMaterial.color = Color.HSVToRGB (h, 1f, v);
				foreach (ShapeAsset sa in Game.Instance.board.compararAll) {
					MeshRenderer[] mrs = sa.gameObject.GetComponentsInChildren<MeshRenderer> ();
					foreach (MeshRenderer mr in mrs) {
						mr.material.color = Color.red;
					}
				}
				Events.OnMessageShow ("Completaste el cuerpo pero te quedaron prismas afuera");
				Invoke ("SetConfirmation", 5);
			}
		}

		Debug.Log (targetVol + " = " + totalvol + " = "+insideVol) ;
		//Debug.Log (targetVol + " = " + totalvol);
		//Debug.Log (System.Math.Round (targetVol*100,6) + " = " + System.Math.Round (totalvol*100,6));
	}

	void SetStateScreen(){
		if (integrationState == IntegrationStates.question) {
			Events.OnMessageShow ("");
			SetConsigna ();
			question.SetActive(true);
			integrating.SetActive(false);
			confirmation.SetActive(false);
		} else if (integrationState == IntegrationStates.integrating) {
			question.SetActive(false);
			integrating.SetActive(true);
			confirmation.SetActive(false);
		} else if (integrationState == IntegrationStates.confirmation) {
			question.SetActive(false);
			integrating.SetActive(false);
			confirmation.SetActive(true);
		}
	}

	void SetConfirmation(){	
		if (integrationState == IntegrationStates.integrating) {
			integrationState = IntegrationStates.confirmation;
			Events.CloseSubMenu ();
			SetStateScreen ();
		}
	}

	void SetConsigna(){		
		consigna.text = Game.Instance.levelManager.GetConsignaComparar ();
		targetMaterial.color = originalTargetColor;
		Game.Instance.levelManager.LevelShapeShow (true);
	}

	public void SetAnswer(string s){
		count.color = Color.white;
		count.text = respuesta.text;
		answer = float.Parse(respuesta.text);
		Debug.Log(respuesta.text);
		integrationState = IntegrationStates.integrating;
		respuesta.text = "";
		Game.Instance.levelManager.LevelShapeShow (false);
		SetStateScreen ();
	}


	public void Confirmation(bool confirm){		
		if (!confirm) {
			badConfirmationTxt.SetActive (true);
			Invoke ("NextExcersice", 5);
		} else {
			Game.Instance.levelManager.NextExcersice ();
			NextExcersice ();
		}
	}

	public void NextExcersice(){
		Game.Instance.board.Clear ();
		integrationState = IntegrationStates.question;
		SetStateScreen ();
	}
}